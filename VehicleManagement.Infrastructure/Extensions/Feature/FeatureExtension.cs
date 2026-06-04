using VehicleManagement.Infrastructure.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace VehicleManagement.Infrastructure.Extensions.Feature;

public static class FeatureExtension
{
    public const string PolicyCors = "policyCors";

    public static IServiceCollection AddFeature(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddLocalization();
        services.AddMvc();
        services.AddControllers(options =>
        {
            options.Filters.Add(typeof(AppExceptionFilterAttribute));
        }).AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            options.SerializerSettings.DateFormatString = "dd/MM/yyyy HH:mm:ss";
            options.SerializerSettings.FloatParseHandling = Newtonsoft.Json.FloatParseHandling.Double;
        });

        return services;
    }

    public static IApplicationBuilder UseFeature(this IApplicationBuilder app,
                                                      IWebHostEnvironment environment,
                                                      IConfigurationBuilder configuration)
    {
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors(PolicyCors);


        if (!environment.IsProduction())
        {
            configuration.AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: false,
                reloadOnChange: true);
        }



        return app;
    }
}
