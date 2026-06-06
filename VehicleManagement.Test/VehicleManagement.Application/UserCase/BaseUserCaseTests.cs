using System.Linq.Expressions;
using Moq;
using VehicleManagement.Application.UserCase.BaseCommands.Commands.CreateBase;
using VehicleManagement.Application.UserCase.BaseCommands.Commands.DaleteBase;
using VehicleManagement.Application.UserCase.BaseCommands.Commands.UpdateBase;
using VehicleManagement.Application.UserCase.BaseCommands.Query;
using VehicleManagement.Application.UserCase.BaseCommands.Query.GetAllBaseQuery;
using VehicleManagement.Application.UserCase.BaseCommands.Query.GetBasePaginatedQuery;
using VehicleManagement.Application.UserCase.BaseCommands.Query.GetByIdBase;
using VehicleManagement.Domain.Common.Exceptions;
using VehicleManagement.Domain.Common.Wrappers;
using VehicleManagement.Domain.Entities.Base;
using VehicleManagement.Domain.Ports;
using Xunit;

namespace VehicleManagement.Test.UserCase;

public class BaseUserCaseTests
{
    // --- Setup Helpers ---

    public class TestEntity : BaseEntity<string>
    {
        public string Name { get; set; } = string.Empty;
    }

    public record TestResponse(string Id, string Name);

    // --- Create Handler Tests ---

    public class TestCreateCommand : CreateCommand<TestEntity, TestResponse>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class TestCreateHandler : CreateHandler<TestCreateCommand, TestEntity, TestResponse>
    {
        public bool BeforeCalled { get; private set; }
        public bool AfterCalled { get; private set; }

        public TestCreateHandler(IGenericRepository<TestEntity> repository) : base(repository) { }

        protected override TestEntity MapToEntity(TestCreateCommand request) => new TestEntity { Name = request.Name };
        protected override TestResponse MapToResponse(TestEntity entity) => new TestResponse(entity.Id, entity.Name);

        protected override Task BeforeCreateAsync(TestCreateCommand request)
        {
            BeforeCalled = true;
            return base.BeforeCreateAsync(request);
        }

        protected override Task AfterCreateAsync(TestEntity entity)
        {
            AfterCalled = true;
            return base.AfterCreateAsync(entity);
        }
    }

    [Fact]
    public async Task CreateHandler_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var repositoryMock = new Mock<IGenericRepository<TestEntity>>();
        var handler = new TestCreateHandler(repositoryMock.Object);
        var command = new TestCreateCommand { Name = "Test" };
        var entity = new TestEntity { Id = "1", Name = "Test" };

        repositoryMock.Setup(r => r.AddAsync(It.IsAny<TestEntity>(), It.IsAny<Expression<Func<TestEntity, object?>>[]>()))
            .ReturnsAsync(entity);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Test", result.Data!.Name);
        Assert.True(handler.BeforeCalled);
        Assert.True(handler.AfterCalled);
    }

    // --- Update Handler Tests ---

    public class TestUpdateCommand : UpdateCommand<TestEntity, TestResponse>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class TestUpdateHandler : UpdateHandler<TestUpdateCommand, TestEntity, TestResponse>
    {
        public bool BeforeCalled { get; private set; }
        public bool AfterCalled { get; private set; }

        public TestUpdateHandler(IGenericRepository<TestEntity> repository) : base(repository) { }

        protected override void UpdateEntity(TestUpdateCommand request, TestEntity entity) => entity.Name = request.Name;
        protected override TestResponse MapToResponse(TestEntity entity) => new TestResponse(entity.Id, entity.Name);

        protected override Task BeforeUpdateAsync(TestUpdateCommand request, TestEntity entity)
        {
            BeforeCalled = true;
            return base.BeforeUpdateAsync(request, entity);
        }

        protected override Task AfterUpdateAsync(TestUpdateCommand request, TestEntity entity)
        {
            AfterCalled = true;
            return base.AfterUpdateAsync(request, entity);
        }
    }

    [Fact]
    public async Task UpdateHandler_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var repositoryMock = new Mock<IGenericRepository<TestEntity>>();
        var handler = new TestUpdateHandler(repositoryMock.Object);
        var command = new TestUpdateCommand { Id = "1", Name = "Updated" };
        var entity = new TestEntity { Id = "1", Name = "Original" };

        repositoryMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(entity);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Updated", result.Data!.Name);
        Assert.True(handler.BeforeCalled);
        Assert.True(handler.AfterCalled);
        repositoryMock.Verify(r => r.UpdateAsync(entity), Times.Once);
    }

    [Fact]
    public async Task UpdateHandler_EntityNotFound_ThrowsCustomException()
    {
        // Arrange
        var repositoryMock = new Mock<IGenericRepository<TestEntity>>();
        var handler = new TestUpdateHandler(repositoryMock.Object);
        var command = new TestUpdateCommand { Id = "99" };

        repositoryMock.Setup(r => r.GetByIdAsync("99")).ReturnsAsync((TestEntity?)null);

        // Act & Assert
        await Assert.ThrowsAsync<CustomException>(() => handler.Handle(command, CancellationToken.None));
    }

    // --- Delete Handler Tests ---

    [Fact]
    public async Task DeleteHandler_ValidId_ReturnsSuccess()
    {
        // Arrange
        var repositoryMock = new Mock<IGenericRepository<TestEntity>>();
        var handler = new DeleteHandler<DeleteCommand<TestEntity>, TestEntity>(repositoryMock.Object);
        var command = new DeleteCommand<TestEntity>("1");
        var entity = new TestEntity { Id = "1" };

        repositoryMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(entity);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Eliminado exitosamente", result.Message);
        repositoryMock.Verify(r => r.DeleteAsync(entity), Times.Once);
    }

    [Fact]
    public async Task DeleteHandler_EntityNotFound_ThrowsCustomException()
    {
        // Arrange
        var repositoryMock = new Mock<IGenericRepository<TestEntity>>();
        var handler = new DeleteHandler<DeleteCommand<TestEntity>, TestEntity>(repositoryMock.Object);
        var command = new DeleteCommand<TestEntity>("99");

        repositoryMock.Setup(r => r.GetByIdAsync("99")).ReturnsAsync((TestEntity?)null);

        // Act & Assert
        await Assert.ThrowsAsync<CustomException>(() => handler.Handle(command, CancellationToken.None));
    }

    // --- GetAll Handler Tests ---

    public class TestGetAllQuery : GetAllQuery<TestEntity, TestResponse> { }

    public class TestGetAllHandler : GetAllHandler<TestGetAllQuery, TestEntity, TestResponse>
    {
        public bool BeforeCalled { get; private set; }
        public bool AfterCalled { get; private set; }

        public TestGetAllHandler(IGenericRepository<TestEntity> repository) : base(repository) { }

        protected override TestResponse MapToResponse(TestEntity entity) => new TestResponse(entity.Id, entity.Name);

        protected override Task BeforeHandleAsync(TestGetAllQuery query)
        {
            BeforeCalled = true;
            return base.BeforeHandleAsync(query);
        }

        protected override Task AfterHandleAsync(IEnumerable<TestEntity> entities)
        {
            AfterCalled = true;
            return base.AfterHandleAsync(entities);
        }
    }

    [Fact]
    public async Task GetAllHandler_ReturnsList()
    {
        // Arrange
        var repositoryMock = new Mock<IGenericRepository<TestEntity>>();
        var handler = new TestGetAllHandler(repositoryMock.Object);
        var query = new TestGetAllQuery();
        var entities = new List<TestEntity> { new TestEntity { Id = "1", Name = "E1" } };

        repositoryMock.Setup(r => r.GetAsync(null, null, "", false)).ReturnsAsync(entities);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Data!);
        Assert.True(handler.BeforeCalled);
        Assert.True(handler.AfterCalled);
    }

    // --- GetById Handler Tests ---

    public class TestGetByIdQuery : GetByIdBaseQuery<TestEntity, TestResponse> { }

    public class TestGetByIdHandler : GetByIdBaseHandler<TestGetByIdQuery, TestEntity, TestResponse>
    {
        public bool BeforeCalled { get; private set; }
        public bool AfterCalled { get; private set; }

        public TestGetByIdHandler(IGenericRepository<TestEntity> repository) : base(repository) { }

        protected override TestResponse MapToResponse(TestEntity entity) => new TestResponse(entity.Id, entity.Name);

        protected override Task BeforeHandleAsync(TestGetByIdQuery query)
        {
            BeforeCalled = true;
            return base.BeforeHandleAsync(query);
        }

        protected override Task AfterHandleAsync(TestEntity entity)
        {
            AfterCalled = true;
            return base.AfterHandleAsync(entity);
        }
    }

    [Fact]
    public async Task GetByIdHandler_ValidId_ReturnsEntity()
    {
        // Arrange
        var repositoryMock = new Mock<IGenericRepository<TestEntity>>();
        var handler = new TestGetByIdHandler(repositoryMock.Object);
        var query = new TestGetByIdQuery { Id = "1" };
        var entity = new TestEntity { Id = "1", Name = "E1" };

        repositoryMock.Setup(r => r.GetByIdAsync("1", "", false)).ReturnsAsync(entity);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("E1", result.Data!.Name);
        Assert.True(handler.BeforeCalled);
        Assert.True(handler.AfterCalled);
    }

    [Fact]
    public async Task GetByIdHandler_NotFound_ThrowsCustomException()
    {
        // Arrange
        var repositoryMock = new Mock<IGenericRepository<TestEntity>>();
        var handler = new TestGetByIdHandler(repositoryMock.Object);
        var query = new TestGetByIdQuery { Id = "99" };

        repositoryMock.Setup(r => r.GetByIdAsync("99", "", false)).ReturnsAsync((TestEntity?)null);

        // Act & Assert
        await Assert.ThrowsAsync<CustomException>(() => handler.Handle(query, CancellationToken.None));
    }

    // --- GetPaginated Handler Tests ---

    public class TestGetPaginatedQuery : GetPaginatedQuery<TestEntity, TestResponse> { }

    public class TestGetPaginatedHandler : GetPaginatedHandler<TestGetPaginatedQuery, TestEntity, TestResponse>
    {
        public bool AfterCalled { get; private set; }
        public bool BuildQueryCalled { get; private set; }

        public TestGetPaginatedHandler(IGenericRepository<TestEntity> repository) : base(repository) { }

        protected override TestResponse MapToResponse(TestEntity entity) => new TestResponse(entity.Id, entity.Name);

        protected override Task AfterPaginedAsync(IEnumerable<TestEntity> entities)
        {
            AfterCalled = true;
            return base.AfterPaginedAsync(entities);
        }

        protected override async Task<PaginatedOptions<TestEntity>> BuildQueryAsync(TestGetPaginatedQuery request)
        {
            BuildQueryCalled = true;
            return await base.BuildQueryAsync(request);
        }
    }

    [Fact]
    public async Task GetPaginatedHandler_ReturnsPaginatedData()
    {
        // Arrange
        var repositoryMock = new Mock<IGenericRepository<TestEntity>>();
        var handler = new TestGetPaginatedHandler(repositoryMock.Object);
        var query = new TestGetPaginatedQuery { NumeroPagina = 1, TamanoPagina = 10 };
        var entities = new List<TestEntity> { new TestEntity { Id = "1", Name = "E1" } };
        var paginatedResponse = new PaginatedResponse<TestEntity>(entities, 1, 10, 1, 1);

        repositoryMock.Setup(r => r.GetPaginatedAsync(null, null, 1, 10, false, ""))
            .ReturnsAsync(paginatedResponse);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Data!);
        Assert.True(handler.AfterCalled);
        Assert.True(handler.BuildQueryCalled);
    }

    [Fact]
    public async Task GetPaginatedHandler_NoData_Returns404()
    {
        // Arrange
        var repositoryMock = new Mock<IGenericRepository<TestEntity>>();
        var handler = new TestGetPaginatedHandler(repositoryMock.Object);
        var query = new TestGetPaginatedQuery();

        repositoryMock.Setup(r => r.GetPaginatedAsync(null, null, 1, 10, false, ""))
            .ReturnsAsync((PaginatedResponse<TestEntity>?)null);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
    }

    // --- QueryHandlerBase Tests ---

    public class TestQueryHandler : QueryHandlerBase<TestEntity, TestResponse>
    {
        public TestQueryHandler(IGenericRepository<TestEntity> repository) : base(repository) { }

        public async Task<IQueryable<TestEntity>> TestBefore(IQueryable<TestEntity> query) => await BeforeQueryAsync(query, CancellationToken.None);
        public async Task<IQueryable<TestEntity>> TestAfter(IQueryable<TestEntity> query) => await AfterQueryAsync(query, CancellationToken.None);
    }

    [Fact]
    public async Task QueryHandlerBase_Hooks_ReturnQuery()
    {
        // Arrange
        var repositoryMock = new Mock<IGenericRepository<TestEntity>>();
        var handler = new TestQueryHandler(repositoryMock.Object);
        var query = new List<TestEntity>().AsQueryable();

        // Act
        var beforeResult = await handler.TestBefore(query);
        var afterResult = await handler.TestAfter(query);

        // Assert
        Assert.Equal(query, beforeResult);
        Assert.Equal(query, afterResult);
    }
}
