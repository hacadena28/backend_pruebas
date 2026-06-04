using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleManagement.Infrastructure.Extensions.Cors;
using VehicleManagement.Infrastructure.Extensions.Feature;
using VehicleManagement.Infrastructure.Extensions.Mediator;
using VehicleManagement.Infrastructure.Extensions.Persistence;
using VehicleManagement.Infrastructure.Extensions.Services;
using VehicleManagement.Infrastructure.Extensions.Swagger;
using VehicleManagement.Infrastructure.Extensions.Validation;

namespace VehicleManagement.Infrastructure.Extensions;

public static class Startup
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddSwagger()
            .AddDomainServices(config)
            .AddValidation()
            .AddMediator()
            .AddCorsPolicy(config)
            .AddPersistence(config)
            .AddHttpContextAccessor()
            .AddFeature(config);
    }

    public static void UseInfrastructure(this IApplicationBuilder builder, IWebHostEnvironment env)
    {
        builder
            .UseSwagger(env)
            .UseCorsPolicy();
    }
}
