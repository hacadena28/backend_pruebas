using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VehicleManagement.Infrastructure.Adapters;
using VehicleManagement.Infrastructure.Extensions.Cors;
using VehicleManagement.Infrastructure.Extensions.Mediator;
using VehicleManagement.Infrastructure.Extensions.Services;
using VehicleManagement.Infrastructure.Extensions.Validation;

namespace VehicleManagement.Test.VehicleManagement.Infraestructure;

public sealed class ExtensionRegistrationTests
{
    [Fact]
    public void AddValidation_ConfiguresSpanishValidationAndPipelineBehavior()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddValidation();
        var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<ApiBehaviorOptions>>().Value;
        Assert.True(options.SuppressModelStateInvalidFilter);
        Assert.Equal("es", ValidatorOptions.Global.LanguageManager.Culture.TwoLetterISOLanguageName);
        Assert.Contains(services, descriptor =>
            descriptor.ServiceType == typeof(IPipelineBehavior<,>) &&
            descriptor.ImplementationType == typeof(ValidationBehaviour<,>));
    }

    [Fact]
    public void AddMediator_RegistersMediatorServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddMediator();
        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<IMediator>());
    }


    [Fact]
    public void AddCorsPolicy_RegistersCorsOptions_WhenReactOriginsAreConfigured()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CorsSettings:React"] = "https://app.example.com;https://admin.example.com"
            })
            .Build();
        var services = new ServiceCollection();

        services.AddCorsPolicy(configuration);

        Assert.Contains(services, descriptor => descriptor.ServiceType.FullName!.Contains("Cors"));
    }

    [Fact]
    public void UseCorsPolicy_ReturnsSameApplicationBuilder()
    {
        var services = new ServiceCollection();
        services.AddCors();
        var app = new ApplicationBuilder(services.BuildServiceProvider());

        var result = app.UseCorsPolicy();

        Assert.Same(app, result);
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
