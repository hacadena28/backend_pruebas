using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VehicleManagement.Domain.Ports.Configuration;

namespace VehicleManagement.Infrastructure.Extensions.Services;

public static class ServiceExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection svc, IConfiguration config)
    {
        svc.AddHttpClient();
        svc.AddJwtAuthentication(config);
        return svc;
    }

    private static IServiceCollection AddJwtAuthentication(this IServiceCollection svc, IConfiguration config)
    {
        var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? config["JWT_ISSUER"] ?? string.Empty;
        var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? config["JWT_AUDIENCE"] ?? string.Empty;
        var signingKey = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY") ?? config["JWT_SIGNING_KEY"] ?? string.Empty;

        var keyBytes = Encoding.UTF8.GetBytes(signingKey ?? string.Empty);

        svc.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
        
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
        
                ValidateAudience = true,
                ValidAudience = audience,
        
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"JWT Error: {context.Exception}");
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    Console.WriteLine($"Challenge Error: {context.Error}");
                    Console.WriteLine($"Description: {context.ErrorDescription}");
                    return Task.CompletedTask;
                }
            };
        });
       
        svc.AddSingleton<IJwtTokenGenerator, Adapters.JwtTokenGenerator>();
        return svc;
    }

    private static int? GetIntegerEnvironmentVariable(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return int.TryParse(value, out var parsedValue) ? parsedValue : null;
    }

    private static bool? GetBooleanEnvironmentVariable(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return bool.TryParse(value, out var parsedValue) ? parsedValue : null;
    }
}
