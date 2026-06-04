using VehicleManagement.Domain.Entities.Base;
using VehicleManagement.Domain.Ports;
using VehicleManagement.Infrastructure.Adapters;
using VehicleManagement.Infrastructure.Contexts;
using VehicleManagement.Infrastructure.Extensions.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using VehicleManagement.Domain.Entities;

namespace VehicleManagement.Test.VehicleManagement.Infraestructure;

public sealed class MongoInfrastructureTests
{
    [Fact]
    public void MongoDbContext_ThrowsWhenClientOrOptionsAreNull()
    {
        var client = new Mock<IMongoClient>().Object;

        Assert.Throws<ArgumentNullException>(() => new MongoDbContext(null!, Options.Create(new MongoDbSettings { DatabaseName = "VehicleManagement" })));
        Assert.Throws<ArgumentNullException>(() => new MongoDbContext(client, null!));
    }

    [Fact]
    public void MongoDbContext_ThrowsWhenDatabaseNameIsMissing()
    {
        var client = new Mock<IMongoClient>().Object;

        var exception = Assert.Throws<InvalidOperationException>(() =>
            new MongoDbContext(client, Options.Create(new MongoDbSettings())));

        Assert.Equal("MongoDb:DatabaseName is required.", exception.Message);
    }

    [Fact]
    public void MongoDbContext_CollectionUsesMongoCollectionAttributeName()
    {
        var collection = new Mock<IMongoCollection<Vehiculo>>();
        var database = new Mock<IMongoDatabase>();
        database.Setup(db => db.GetCollection<Vehiculo>("Vehiculo", null)).Returns(collection.Object);
        var client = new Mock<IMongoClient>();
        client.Setup(value => value.GetDatabase("VehicleManagement", null)).Returns(database.Object);
        var sut = new MongoDbContext(client.Object, Options.Create(new MongoDbSettings { DatabaseName = "VehicleManagement" }));

        var result = sut.Collection<Vehiculo>();

        Assert.Same(collection.Object, result);
        database.Verify(db => db.GetCollection<Vehiculo>("Vehiculo", null), Times.Once);
    }

    [Fact]
    public void MongoDbContext_CollectionFallsBackToTypeName_WhenAttributeIsMissing()
    {
        var collection = new Mock<IMongoCollection<TestEntity>>();
        var database = new Mock<IMongoDatabase>();
        database.Setup(db => db.GetCollection<TestEntity>("TestEntity", null)).Returns(collection.Object);
        var client = new Mock<IMongoClient>();
        client.Setup(value => value.GetDatabase("VehicleManagement", null)).Returns(database.Object);
        var sut = new MongoDbContext(client.Object, Options.Create(new MongoDbSettings { DatabaseName = "VehicleManagement" }));

        var result = sut.Collection<TestEntity>();

        Assert.Same(collection.Object, result);
    }

    [Fact]
    public void UnitOfWork_ReturnsCachedGenericRepositoryForEntityType()
    {
        var context = CreateContext();
        var sut = new UnitOfWork(context);

        var first = sut.Repository<Vehiculo>();
        var second = sut.Repository<Vehiculo>();

        Assert.Same(first, second);
        Assert.IsAssignableFrom<IGenericRepository<Vehiculo>>(first);
    }

    [Fact]
    public void AddContextDatabase_RegistersMongoServicesAndBindsConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MongoDb:ConnectionString"] = "mongodb://localhost:27017",
                ["MongoDb:DatabaseName"] = "VehicleManagement"
            })
            .Build();

        var services = new ServiceCollection();

        services.AddContextDatabase(configuration);
        var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
        Assert.Equal("mongodb://localhost:27017", options.ConnectionString);
        Assert.Equal("VehicleManagement", options.DatabaseName);
        Assert.IsType<MongoClient>(provider.GetRequiredService<IMongoClient>());
        Assert.NotNull(provider.GetRequiredService<MongoDbContext>());
    }

    [Fact]
    public void AddContextDatabase_ThrowsWhenConnectionStringIsMissingAndClientIsResolved()
    {
        var previous = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");
        Environment.SetEnvironmentVariable("MONGODB_CONNECTION_STRING", null);
        try
        {
            var services = new ServiceCollection();
            services.AddContextDatabase(new ConfigurationBuilder().Build());
            var provider = services.BuildServiceProvider();

            var exception = Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<IMongoClient>());

            Assert.Equal("MongoDb:ConnectionString or MONGODB_CONNECTION_STRING is required.", exception.Message);
        }
        finally
        {
            Environment.SetEnvironmentVariable("MONGODB_CONNECTION_STRING", previous);
        }
    }

    private static MongoDbContext CreateContext()
    {
        var database = new Mock<IMongoDatabase>();
        var client = new Mock<IMongoClient>();
        client.Setup(value => value.GetDatabase("VehicleManagement", null)).Returns(database.Object);
        return new MongoDbContext(client.Object, Options.Create(new MongoDbSettings { DatabaseName = "VehicleManagement" }));
    }

    public sealed class TestEntity : BaseEntity<string>
    {
    }
}
