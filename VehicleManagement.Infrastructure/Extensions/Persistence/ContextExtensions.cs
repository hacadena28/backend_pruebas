using VehicleManagement.Infrastructure.Contexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace VehicleManagement.Infrastructure.Extensions.Persistence;

public static class ContextExtensions
{
    public static IServiceCollection AddContextDatabase(this IServiceCollection svc, IConfiguration config)
    {
        svc.Configure<MongoDbSettings>(options =>
        {
            config.GetSection(MongoDbSettings.SectionName).Bind(options);
            options.ConnectionString = GetMongoConnectionString(config);
            options.DatabaseName = GetMongoDatabaseName(config);
        });

        svc.AddSingleton<IMongoClient>(_ =>
        {
            var connectionString = GetMongoConnectionString(config);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("MongoDb:ConnectionString or MONGODB_CONNECTION_STRING is required.");
            }

            return new MongoClient(connectionString);
        });

        svc.AddSingleton<MongoDbContext>();

        return svc;
    }

    private static string GetMongoConnectionString(IConfiguration config)
    {
        return Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING")
            ?? config.GetSection(MongoDbSettings.SectionName).GetValue<string>("ConnectionString")
            ?? string.Empty;
    }

    private static string GetMongoDatabaseName(IConfiguration config)
    {
        return Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME")
            ?? config.GetSection(MongoDbSettings.SectionName).GetValue<string>("DatabaseName")
            ?? string.Empty;
    }
}
