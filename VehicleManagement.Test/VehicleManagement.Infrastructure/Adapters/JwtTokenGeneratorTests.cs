using Microsoft.Extensions.Configuration;
using Moq;
using VehicleManagement.Domain.Entities;
using VehicleManagement.Infrastructure.Adapters;
using Xunit;

namespace VehicleManagement.Test.VehicleManagement.Infrastructure.Adapters;

public class JwtTokenGeneratorTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly JwtTokenGenerator _generator;

    public JwtTokenGeneratorTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _generator = new JwtTokenGenerator();
        
        // Set environment variables needed for the generator
        Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", "this_is_a_very_long_secret_key_at_least_32_chars");
        Environment.SetEnvironmentVariable("JWT_ISSUER", "test-issuer");
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", "test-audience");
    }

    [Fact]
    public void Generate_ValidUser_ReturnsToken()
    {
        // Arrange
        var user = new User
        {
            Id = "123",
            Email = "test@example.com",
            FullName = "Test User"
        };

        // Act
        var token = _generator.Generate(user);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
        
        // Optionally verify it's a valid JWT format (header.payload.signature)
        var parts = token.Split('.');
        Assert.Equal(3, parts.Length);
    }

    [Fact]
    public void Generate_MissingEnvVars_ThrowsException()
    {
        // Arrange
        Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", null);
        var user = new User { Id = "123", Email = "test@example.com", FullName = "Test" };

        // Act & Assert
        // Depending on how SymmetricSecurityKey handles empty/null key, it might throw.
        // In JwtTokenGenerator: Environment.GetEnvironmentVariable("JWT_SIGNING_KEY") ?? string.Empty
        // Empty key will throw ArgumentException in SymmetricSecurityKey constructor
        Assert.ThrowsAny<Exception>(() => _generator.Generate(user));
        
        // Restore for other tests if they run in same process (though xunit isolation usually helps)
        Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", "this_is_a_very_long_secret_key_at_least_32_chars");
    }
}
