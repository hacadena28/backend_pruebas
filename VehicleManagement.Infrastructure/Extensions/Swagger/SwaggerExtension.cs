using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

namespace VehicleManagement.Infrastructure.Extensions.Swagger;

public static class SwaggerExtension
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            // XML comments
            var xmlFile = $"{Assembly.GetEntryAssembly()!.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

            // Server URL desde variable de entorno
            var serverUrl = Environment.GetEnvironmentVariable("SWAGGER_HOST") ?? "http://localhost";
            options.AddServer(new OpenApiServer { Url = serverUrl });

            // Definición del esquema JWT Bearer
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter JWT Bearer token *only*",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
  
            };

            options.AddSecurityDefinition("bearer", securityScheme);

            // Requerimiento de seguridad global — aplica Bearer a todos los endpoints
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("bearer", document)] = []
            });

            options.ExampleFilters();
            options.SchemaFilter<IgnoreSwaggerPropertiesSchemaFilter>();
        });

        return services;
    }

    public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsProduction()) return app;

        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.DefaultModelExpandDepth(2);
            options.DefaultModelsExpandDepth(-1);
            options.DisplayOperationId();
            options.DisplayRequestDuration();
            options.EnableDeepLinking();
            options.ShowExtensions();
            options.EnableValidator(null);
            options.DocExpansion(DocExpansion.None);
        });

        return app;
    }
}