using Moq;
using VehicleManagement.Application.Vehiculos.Dtos;
using VehicleManagement.Application.Vehiculos.Queries;
using VehicleManagement.Application.Vehiculos.Commands;
using VehicleManagement.Application.UserCase.BaseCommands.Commands.DaleteBase;
using VehicleManagement.Domain.Common.Wrappers;
using VehicleManagement.Domain.Entities;
using VehicleManagement.Domain.Ports;
using EVehiculo = VehicleManagement.Domain.Entities.Vehiculo;

namespace VehicleManagement.Test.VehicleManagement.Application.VehiculoTests;

public class VehiculoHandlersTests
{
    [Fact]
    public async Task CreateVehiculoHandler_Should_Call_Repository_AddAsync()
    {
        var repoMock = new Mock<IGenericRepository<EVehiculo>>();
        repoMock.Setup(r => r.AddAsync(It.IsAny<EVehiculo>(), It.IsAny<System.Linq.Expressions.Expression<System.Func<EVehiculo, object?>>[]>()))
            .ReturnsAsync((EVehiculo v, System.Linq.Expressions.Expression<System.Func<EVehiculo, object?>>[] inc) => v);

        var handler = new CreateVehiculoHandler(repoMock.Object);

        var cmd = new CreateVehiculoCommand { Placa = "ABC123", Marca = "Toyota", Modelo = "Corolla", Anio = 2020, Color = "Rojo" };

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<Response<VehiculoDto>>(result);
        repoMock.Verify(r => r.AddAsync(It.IsAny<Vehiculo>(), It.IsAny<System.Linq.Expressions.Expression<System.Func<Vehiculo, object?>>[]>()), Times.Once);
    }

    [Fact]
    public async Task GetVehiculoByIdHandler_Should_Return_Response()
    {
        var veh = new EVehiculo { Id = "1", Placa = "ABC123", Marca = "Toyota", Modelo = "Corolla", Anio = 2020, Color = "Rojo", FechaRegistro = System.DateTime.UtcNow };

        var repoMock = new Mock<IGenericRepository<EVehiculo>>();
        repoMock.Setup(r => r.GetByIdAsync("1", It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(veh);

        var handler = new GetVehiculoByIdHandler(repoMock.Object);

        var query = new GetVehiculoByIdQuery { Id = "1" };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("ABC123", result.Data.Placa);
    }

    [Fact]
    public async Task GetAllVehiculosHandler_Should_Return_List()
    {
        var veh2 = new EVehiculo { Id = "1", Placa = "ABC123", Marca = "Toyota", Modelo = "Corolla", Anio = 2020, Color = "Rojo", FechaRegistro = System.DateTime.UtcNow };
        var list = new List<EVehiculo> { veh2 };

        var repoMock = new Mock<IGenericRepository<EVehiculo>>();
        repoMock.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<EVehiculo, bool>>?>(), It.IsAny<System.Func<System.Linq.IQueryable<EVehiculo>, System.Linq.IOrderedQueryable<EVehiculo>>>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(list as IEnumerable<EVehiculo>);

        var handler = new GetAllVehiculosHandler(repoMock.Object);

        var query = new GetAllVehiculosQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task UpdateVehiculoHandler_Should_Update_Entity_And_Return_Response()
    {
        // Arrange
        var id = "1";
        var originalVehiculo = new EVehiculo 
        { 
            Id = id, 
            Placa = "OLD-123", 
            Marca = "Toyota", 
            Modelo = "Corolla", 
            Anio = 2010, 
            Color = "Blanco", 
            FechaRegistro = DateTime.UtcNow 
        };
        
        var repoMock = new Mock<IGenericRepository<EVehiculo>>();
        repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(originalVehiculo);
        repoMock.Setup(r => r.UpdateAsync(It.IsAny<EVehiculo>())).Returns(Task.CompletedTask);

        var handler = new UpdateVehiculoHandler(repoMock.Object);
        var cmd = new UpdateVehiculoCommand 
        { 
            Id = id, 
            Placa = "NEW-789", 
            Marca = "Honda", 
            Modelo = "Civic", 
            Anio = 2022, 
            Color = "Negro" 
        };

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("NEW-789", result.Data!.Placa);
        Assert.Equal("Honda", result.Data.Marca);
        Assert.Equal("Civic", result.Data.Modelo);
        Assert.Equal(2022, result.Data.Anio);
        Assert.Equal("Negro", result.Data.Color);
        
        repoMock.Verify(r => r.UpdateAsync(It.Is<EVehiculo>(v => 
            v.Placa == "NEW-789" && 
            v.Marca == "Honda" && 
            v.Modelo == "Civic" && 
            v.Anio == 2022 && 
            v.Color == "Negro")), Times.Once);
    }

    [Fact]
    public async Task DeleteVehiculoHandler_Should_Delete_And_Return_Success()
    {
        // Arrange
        var id = "1";
        var vehiculo = new EVehiculo { Id = id };
        var repoMock = new Mock<IGenericRepository<EVehiculo>>();
        repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(vehiculo);
        repoMock.Setup(r => r.DeleteAsync(vehiculo)).Returns(Task.CompletedTask);

        var handler = new DeleteHandler<DeleteVehiculoCommand, EVehiculo>(repoMock.Object);
        var cmd = new DeleteVehiculoCommand(id);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Eliminado exitosamente", result.Message);
        repoMock.Verify(r => r.DeleteAsync(vehiculo), Times.Once);
    }
}
