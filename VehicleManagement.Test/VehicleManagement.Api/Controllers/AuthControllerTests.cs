using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VehicleManagement.Api.Controllers;
using VehicleManagement.Application.Auth.Commands;
using VehicleManagement.Application.Auth.DTOs;
using Xunit;

namespace VehicleManagement.Test.VehicleManagement.Api.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new AuthController();

        // Setup ControllerContext to provide mocked Mediator via RequestServices
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(x => x.GetService(typeof(IMediator))).Returns(_mediatorMock.Object);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider.Object
            }
        };
    }

    [Fact]
    public async Task Login_ValidCommand_ReturnsOk()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "password123");
        var expectedResponse = new LoginDto { Token = "valid-token" };

        _mediatorMock.Setup(x => x.Send(command, default))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Login(command);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    [Fact]
    public async Task Login_MediatorReturnsNull_ReturnsUnauthorized()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "password123");

        _mediatorMock.Setup(x => x.Send(command, default))
            .ReturnsAsync((LoginDto?)null);

        // Act
        var result = await _controller.Login(command);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        // The controller returns: Unauthorized(new { Message = "Usuario o contraseña incorrectos." })
        // We can check the message if needed, but the type is the main thing here.
    }
}
