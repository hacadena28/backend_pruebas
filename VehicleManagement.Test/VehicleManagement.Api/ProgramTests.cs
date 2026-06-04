using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace VehicleManagement.Test.VehicleManagement.Api;

public sealed class ProgramTests
{
    [Fact]
    public async Task Program_StartsApplicationInDevelopmentAndMapsOpenApi()
    {
        using var currentDirectory = new CurrentDirectoryScope();
        Directory.CreateDirectory("VehicleManagement.Api");
        await File.WriteAllTextAsync(".env", "PROGRAM_TEST_ENV=from-root");
        await File.WriteAllTextAsync(Path.Combine("VehicleManagement.Api", ".env"), "PROGRAM_TEST_API_ENV=from-api");
        await File.WriteAllTextAsync(Path.Combine(AppContext.BaseDirectory, "testhost.xml"), "<doc><members /></doc>");

        await using var factory = CreateFactory("Development");
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });

        using var response = await client.GetAsync("/openapi/v1.json");

        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, $"Expected OpenAPI endpoint to succeed, got {(int)response.StatusCode} {response.StatusCode}: {body}");
        Assert.Equal("from-root", Environment.GetEnvironmentVariable("PROGRAM_TEST_ENV"));
        Assert.Equal("from-api", Environment.GetEnvironmentVariable("PROGRAM_TEST_API_ENV"));
    }

    [Fact]
    public async Task Program_StartsApplicationOutsideDevelopmentWithoutOpenApiEndpoint()
    {
        using var currentDirectory = new CurrentDirectoryScope();

        await using var factory = CreateFactory("Production");
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });

        using var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Program_MapsHealthEndpointWithoutHttpsRedirect()
    {
        using var currentDirectory = new CurrentDirectoryScope();

        await using var factory = CreateFactory("Production");
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("http://localhost")
        });

        using var response = await client.GetAsync("/health");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", body);
    }

    private static WebApplicationFactory<Program> CreateFactory(string environment)
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment(environment);
                builder.ConfigureAppConfiguration((_, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["CorsSettings:React"] = "https://app.example.com",
                        ["MongoDb:ConnectionString"] = "mongodb://localhost:27017",
                        ["MongoDb:DatabaseName"] = "VehicleManagement",
                        ["AzureEntraId:TenantId"] = "tenant-id",
                        ["AzureEntraId:ClientId"] = "client-id",
                        ["Jwt:Issuer"] = "issuer",
                        ["Jwt:Audience"] = "audience",
                        ["Jwt:SigningKey"] = "01234567890123456789012345678901",
                        ["RefreshToken:ExpirationDays"] = "7"
                    });
                });
            });
    }

    private sealed class CurrentDirectoryScope : IDisposable
    {
        private readonly string _originalDirectory;
        private readonly string _temporaryDirectory;
        private readonly Dictionary<string, string?> _environmentValues;

        public CurrentDirectoryScope()
        {
            _originalDirectory = Directory.GetCurrentDirectory();
            _temporaryDirectory = Path.Combine(Path.GetTempPath(), $"VehicleManagement-api-tests-{Guid.NewGuid():N}");
            Directory.CreateDirectory(_temporaryDirectory);
            Directory.SetCurrentDirectory(_temporaryDirectory);
            _environmentValues = ClearEnvironment(
                "PROGRAM_TEST_ENV",
                "PROGRAM_TEST_API_ENV",
                "CorsSettings__React",
                "MONGODB_CONNECTION_STRING",
                "MONGODB_DATABASE_NAME",
                "AZURE_ENTRA_TENANT_ID",
                "AZURE_ENTRA_CLIENT_ID",
                "JWT_ISSUER",
                "JWT_AUDIENCE",
                "JWT_SIGNING_KEY");
            Environment.SetEnvironmentVariable("CorsSettings__React", "https://app.example.com");
            Environment.SetEnvironmentVariable("MONGODB_CONNECTION_STRING", "mongodb://localhost:27017");
            Environment.SetEnvironmentVariable("MONGODB_DATABASE_NAME", "VehicleManagement");
            Environment.SetEnvironmentVariable("AZURE_ENTRA_TENANT_ID", "tenant-id");
            Environment.SetEnvironmentVariable("AZURE_ENTRA_CLIENT_ID", "client-id");
            Environment.SetEnvironmentVariable("JWT_ISSUER", "issuer");
            Environment.SetEnvironmentVariable("JWT_AUDIENCE", "audience");
            Environment.SetEnvironmentVariable("JWT_SIGNING_KEY", "01234567890123456789012345678901");
        }

        public void Dispose()
        {
            Directory.SetCurrentDirectory(_originalDirectory);
            RestoreEnvironment(_environmentValues);

            if (Directory.Exists(_temporaryDirectory))
            {
                Directory.Delete(_temporaryDirectory, recursive: true);
            }
        }

        private static Dictionary<string, string?> ClearEnvironment(params string[] names)
        {
            var values = names.ToDictionary(name => name, Environment.GetEnvironmentVariable);
            foreach (var name in names)
            {
                Environment.SetEnvironmentVariable(name, null);
            }

            return values;
        }

        private static void RestoreEnvironment(Dictionary<string, string?> values)
        {
            foreach (var pair in values)
            {
                Environment.SetEnvironmentVariable(pair.Key, pair.Value);
            }
        }
    }
}
