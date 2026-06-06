using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VehicleManagement.Api.Controllers;
using VehicleManagement.Application.Vehiculos.Commands;
using VehicleManagement.Application.Vehiculos.Dtos;
using VehicleManagement.Application.Vehiculos.Queries;
using VehicleManagement.Domain.Common.Wrappers;
using Xunit;

namespace VehicleManagement.Test.VehicleManagement.Api.Controllers;

public class VehiculosControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly VehiculosController _controller;

    public VehiculosControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new VehiculosController();

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(x => x.GetService(typeof(IMediator))).Returns(_mediatorMock.Object);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider.Object
            }
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithData()
    {
        // Arrange
        var expectedVehiculos = new List<VehiculoDto>
        {
            new VehiculoDto("1", "ABC-123", "Toyota", "Corolla", 2020, "Azul", DateTime.UtcNow)
        };
        var response = new Response<IEnumerable<VehiculoDto>>(expectedVehiculos);
        
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllVehiculosQuery>(), default))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualResponse = Assert.IsType<Response<IEnumerable<VehiculoDto>>>(okResult.Value);
        Assert.Equal(expectedVehiculos, actualResponse.Data);
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllVehiculosQuery>(), default), Times.Once);
    }

    [Fact]
    public async Task GetById_ReturnsOkWithData()
    {
        // Arrange
        var id = "1";
        var expectedVehiculo = new VehiculoDto(id, "ABC-123", "Toyota", "Corolla", 2020, "Azul", DateTime.UtcNow);
        var response = new Response<VehiculoDto>(expectedVehiculo);

        _mediatorMock.Setup(m => m.Send(It.Is<GetVehiculoByIdQuery>(q => q.Id == id), default))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetById(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualResponse = Assert.IsType<Response<VehiculoDto>>(okResult.Value);
        Assert.Equal(expectedVehiculo, actualResponse.Data);
    }

    [Fact]
    public async Task Create_ValidCommand_ReturnsCreatedAtAction()
    {
        // Arrange
        var command = new CreateVehiculoCommand { Placa = "ABC-123" };
        var createdVehiculo = new VehiculoDto("1", "ABC-123", "Toyota", "Corolla", 2020, "Azul", DateTime.UtcNow);
        var response = new Response<VehiculoDto>(createdVehiculo);

        _mediatorMock.Setup(m => m.Send(command, default))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Create(command);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
        Assert.Equal("1", createdResult.RouteValues?["id"]);
        Assert.Equal(response, createdResult.Value);
    }

    [Fact]
    public async Task Create_MediatorReturnsNull_ReturnsBadRequest()
    {
        // Arrange
        var command = new CreateVehiculoCommand();
        _mediatorMock.Setup(m => m.Send(command, default))
            .ReturnsAsync((Response<VehiculoDto>)null!);

        // Act
        var result = await _controller.Create(command);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ResponseDataNull_ReturnsBadRequest()
    {
        // Arrange
        var command = new CreateVehiculoCommand();
        var response = new Response<VehiculoDto>(400, "Error");
        
        _mediatorMock.Setup(m => m.Send(command, default))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Create(command);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(response, badRequestResult.Value);
    }

    [Fact]
    public async Task Update_ValidCommand_ReturnsOk()
    {
        // Arrange
        var id = "1";
        var command = new UpdateVehiculoCommand { Placa = "XYZ-789" };
        var updatedVehiculo = new VehiculoDto(id, "XYZ-789", "Toyota", "Corolla", 2021, "Rojo", DateTime.UtcNow);
        var response = new Response<VehiculoDto>(updatedVehiculo);

        _mediatorMock.Setup(m => m.Send(command, default))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Update(id, command);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(response, okResult.Value);
        Assert.Equal(id, command.Id);
    }

    [Fact]
    public async Task Delete_ValidId_ReturnsOk()
    {
        // Arrange
        var id = "1";
        var response = new Response<bool>(true);

        _mediatorMock.Setup(m => m.Send(It.Is<DeleteVehiculoCommand>(c => c.Id == id), default))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(response, okResult.Value);
    }
}
