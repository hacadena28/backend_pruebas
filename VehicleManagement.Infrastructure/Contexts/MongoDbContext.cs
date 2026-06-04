using VehicleManagement.Domain.Attributes;
using VehicleManagement.Domain.Entities.Base;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace VehicleManagement.Infrastructure.Contexts;

public sealed class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoClient client, IOptions<MongoDbSettings> options)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(options);

        var settings = options.Value;
        if (string.IsNullOrWhiteSpace(settings.DatabaseName))
        {
            throw new InvalidOperationException("MongoDb:DatabaseName is required.");
        }

        _database = client.GetDatabase(settings.DatabaseName);
    }

    public IMongoCollection<E> Collection<E>() where E : BaseEntity<string>
    {
        return _database.GetCollection<E>(GetCollectionName<E>());
    }

    private static string GetCollectionName<E>()
    {
        var attribute = typeof(E)
            .GetCustomAttributes(typeof(MongoCollectionAttribute), inherit: false)
            .OfType<MongoCollectionAttribute>()
            .FirstOrDefault();

        return attribute?.Name ?? typeof(E).Name;
    }
}
