using Moq;
using System.Linq.Expressions;
using VehicleManagement.Application.Auth.Commands;
using VehicleManagement.Domain.Common.Exceptions;
using VehicleManagement.Domain.Entities;
using VehicleManagement.Domain.Ports;
using VehicleManagement.Domain.Ports.Configuration;
using Xunit;

namespace VehicleManagement.Test.VehicleManagement.Application.Auth.Commands;

public class LoginCommandHandlerTests
{
    private readonly Mock<IGenericRepository<User>> _userRepositoryMock;
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IGenericRepository<User>>();
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        _handler = new LoginCommandHandler(_userRepositoryMock.Object, _jwtTokenGeneratorMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "password123");
        var user = new User
        {
            Id = "123",
            Email = "test@example.com",
            PasswordHash = "password123",
            FullName = "Test User"
        };

        _userRepositoryMock.Setup(x => x.GetEntityAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<bool>()))
            .ReturnsAsync(user);

        _jwtTokenGeneratorMock.Setup(x => x.Generate(user))
            .Returns("valid-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("valid-token", result.Token);
        _jwtTokenGeneratorMock.Verify(x => x.Generate(user), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsUnauthorizedException()
    {
        // Arrange
        var command = new LoginCommand("notfound@example.com", "password123");

        _userRepositoryMock.Setup(x => x.GetEntityAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<bool>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedException>(() => 
            _handler.Handle(command, CancellationToken.None));
        
        Assert.Equal("Credenciales inválidas", exception.Message);
    }

    [Fact]
    public async Task Handle_InvalidPassword_ThrowsUnauthorizedException()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "wrongpassword");
        var user = new User
        {
            Id = "123",
            Email = "test@example.com",
            PasswordHash = "password123"
        };

        _userRepositoryMock.Setup(x => x.GetEntityAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<bool>()))
            .ReturnsAsync(user);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedException>(() => 
            _handler.Handle(command, CancellationToken.None));
        
        Assert.Equal("Credenciales inválidas", exception.Message);
    }
}
