using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using System.Linq.Expressions;
using VehicleManagement.Domain.Common.Exceptions;
using VehicleManagement.Domain.Entities.Base;
using VehicleManagement.Infrastructure.Adapters;
using VehicleManagement.Infrastructure.Contexts;
using Xunit;

namespace VehicleManagement.Test.VehicleManagement.Infrastructure.Adapters;

public class GenericRepositoryTests
{
    public class TestEntity : BaseEntity<string>
    {
        public string Name { get; set; } = string.Empty;
    }

    private readonly Mock<IMongoCollection<TestEntity>> _collectionMock;
    private readonly GenericRepository<TestEntity> _repository;

    public GenericRepositoryTests()
    {
        var clientMock = new Mock<IMongoClient>();
        var optionsMock = new Microsoft.Extensions.Options.OptionsWrapper<MongoDbSettings>(new MongoDbSettings { DatabaseName = "TestDb" });
        var databaseMock = new Mock<IMongoDatabase>();
        _collectionMock = new Mock<IMongoCollection<TestEntity>>();

        clientMock.Setup(c => c.GetDatabase("TestDb", null)).Returns(databaseMock.Object);
        databaseMock.Setup(d => d.GetCollection<TestEntity>("TestEntity", null)).Returns(_collectionMock.Object);
        
        var context = new MongoDbContext(clientMock.Object, optionsMock);
        _repository = new GenericRepository<TestEntity>(context);
    }

    [Fact]
    public void Constructor_NullContext_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new GenericRepository<TestEntity>(null!));
    }

    [Fact]
    public async Task AddAsync_ValidEntity_SetsMetadataAndInserts()
    {
        var entity = new TestEntity { Name = "Test" };

        await _repository.AddAsync(entity, Array.Empty<Expression<Func<TestEntity, object?>>>());

        Assert.False(string.IsNullOrWhiteSpace(entity.Id));
        Assert.NotEqual(default, entity.CreatedAt);
        _collectionMock.Verify(c => c.InsertOneAsync(entity, null, default), Times.Once);
    }

    [Fact]
    public async Task AddAsync_NullEntity_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AddAsync((TestEntity)null!, Array.Empty<Expression<Func<TestEntity, object?>>>()));
    }

    [Fact]
    public async Task AddAsync_MultipleEntities_InsertsMany()
    {
        var entities = new List<TestEntity> 
        { 
            new TestEntity { Name = "E1" },
            new TestEntity { Name = "E2", Id = "already-has-id" }
        };

        await _repository.AddAsync((IEnumerable<TestEntity>)entities);

        Assert.All(entities, e => Assert.False(string.IsNullOrWhiteSpace(e.Id)));
        _collectionMock.Verify(c => c.InsertManyAsync(It.Is<IEnumerable<TestEntity>>(en => en.Count() == 2), null, default), Times.Once);
    }

    [Fact]
    public async Task AddAsync_EmptyList_DoesNotCallInsertMany()
    {
        await _repository.AddAsync((IEnumerable<TestEntity>)new List<TestEntity>());
        _collectionMock.Verify(c => c.InsertManyAsync(It.IsAny<IEnumerable<TestEntity>>(), null, default), Times.Never);
    }

    [Fact]
    public async Task AddAsync_NullEntities_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AddAsync((IEnumerable<TestEntity>)null!));
    }

    [Fact]
    public async Task DeleteAsync_ValidEntity_UpdatesAndReplaces()
    {
        var id = ObjectId.GenerateNewId().ToString();
        var entity = new TestEntity { Id = id };
        var replaceResult = new ReplaceOneResult.Acknowledged(1, 1, null);

        _collectionMock.Setup(c => c.ReplaceOneAsync(It.IsAny<FilterDefinition<TestEntity>>(), entity, It.IsAny<ReplaceOptions>(), default))
            .ReturnsAsync(replaceResult);

        await _repository.DeleteAsync(entity);

        Assert.True(entity.IsDeleted);
        Assert.NotNull(entity.DeletedOn);
        _collectionMock.Verify(c => c.ReplaceOneAsync(It.IsAny<FilterDefinition<TestEntity>>(), entity, It.IsAny<ReplaceOptions>(), default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_EntityNotFound_ThrowsNotFoundException()
    {
        var entity = new TestEntity { Id = ObjectId.GenerateNewId().ToString() };
        var replaceResult = new ReplaceOneResult.Acknowledged(0, 0, null);

        _collectionMock.Setup(c => c.ReplaceOneAsync(It.IsAny<FilterDefinition<TestEntity>>(), entity, It.IsAny<ReplaceOptions>(), default))
            .ReturnsAsync(replaceResult);

        await Assert.ThrowsAsync<NotFoundException>(() => _repository.DeleteAsync(entity));
    }

    [Fact]
    public async Task DeleteAsync_NullEntity_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.DeleteAsync((TestEntity)null!));
    }

    [Fact]
    public async Task DeleteAsync_MultipleEntities_CallsDeleteForEach()
    {
        var entities = new List<TestEntity> { new TestEntity { Id = ObjectId.GenerateNewId().ToString() } };
        var replaceResult = new ReplaceOneResult.Acknowledged(1, 1, null);

        _collectionMock.Setup(c => c.ReplaceOneAsync(It.IsAny<FilterDefinition<TestEntity>>(), It.IsAny<TestEntity>(), It.IsAny<ReplaceOptions>(), default))
            .ReturnsAsync(replaceResult);

        await _repository.DeleteAsync((IEnumerable<TestEntity>)entities);

        _collectionMock.Verify(c => c.ReplaceOneAsync(It.IsAny<FilterDefinition<TestEntity>>(), It.IsAny<TestEntity>(), It.IsAny<ReplaceOptions>(), default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NullEntitiesList_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.DeleteAsync((IEnumerable<TestEntity>)null!));
    }

    [Fact]
    public async Task UpdateAsync_ValidEntity_UpdatesMetadataAndReplaces()
    {
        var id = ObjectId.GenerateNewId().ToString();
        var entity = new TestEntity { Id = id };
        var replaceResult = new ReplaceOneResult.Acknowledged(1, 1, null);

        _collectionMock.Setup(c => c.ReplaceOneAsync(It.IsAny<FilterDefinition<TestEntity>>(), entity, It.IsAny<ReplaceOptions>(), default))
            .ReturnsAsync(replaceResult);

        await _repository.UpdateAsync(entity);

        Assert.NotEqual(default, entity.UpdatedAt);
    }

    [Fact]
    public async Task UpdateAsync_NullEntity_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.UpdateAsync(null!));
    }

    [Fact]
    public async Task UpdateAsync_NotFound_ThrowsNotFoundException()
    {
        var entity = new TestEntity { Id = ObjectId.GenerateNewId().ToString() };
        var replaceResult = new ReplaceOneResult.Acknowledged(0, 0, null);

        _collectionMock.Setup(c => c.ReplaceOneAsync(It.IsAny<FilterDefinition<TestEntity>>(), entity, It.IsAny<ReplaceOptions>(), default))
            .ReturnsAsync(replaceResult);

        await Assert.ThrowsAsync<NotFoundException>(() => _repository.UpdateAsync(entity));
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ReturnsNull()
    {
        Assert.Null(await _repository.GetByIdAsync(""));
        Assert.Null(await _repository.GetByIdAsync("not-an-object-id"));
    }

    [Fact]
    public async Task GetByIdAsync_Overload_CallsBase()
    {
        var id = ObjectId.GenerateNewId().ToString();
        // Since we can't easily mock FindAsync extension, we just ensure it doesn't crash if we mock the basic Find
        var cursorMock = new Mock<IAsyncCursor<TestEntity>>();
        _collectionMock.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<TestEntity>>(), It.IsAny<FindOptions<TestEntity, TestEntity>>(), default))
            .ReturnsAsync(cursorMock.Object);

        await _repository.GetByIdAsync(id, "includes", true);
        
        _collectionMock.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<TestEntity>>(), It.IsAny<FindOptions<TestEntity, TestEntity>>(), default), Times.Once);
    }


    [Fact]
    public async Task Exist_NullFilter_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.Exist(null!));
    }

    [Fact]
    public async Task GetEntityAsync_NullFilter_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.GetEntityAsync(null!));
    }

    [Fact]
    public async Task GetGroupedAsync_NullKeySelector_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.GetGroupedAsync(null!));
    }

    [Fact]
    public void GetUnaccent_ValidString_RemovesAccents()
    {
        var input = "áéíóú ñ";
        var expected = "aeiou n";
        var result = _repository.GetUnaccent(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetUnaccent_NullOrEmpty_ThrowsCustomException()
    {
        Assert.Throws<CustomException>(() => _repository.GetUnaccent(null!));
        Assert.Throws<CustomException>(() => _repository.GetUnaccent(""));
        Assert.Throws<CustomException>(() => _repository.GetUnaccent(" "));
    }

    [Fact]
    public void Dispose_Success()
    {
        _repository.Dispose();
    }
}
