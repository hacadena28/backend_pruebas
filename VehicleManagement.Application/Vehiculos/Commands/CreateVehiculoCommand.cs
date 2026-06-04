using VehicleManagement.Application.UserCase.BaseCommands.Commands.CreateBase;
using VehicleManagement.Application.Vehiculos.Dtos;
using EVehiculo =  VehicleManagement.Domain.Entities.Vehiculo;

namespace VehicleManagement.Application.Vehiculos.Commands;

public class CreateVehiculoCommand : CreateCommand<EVehiculo, VehiculoDto>
{
    public string Placa { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Anio { get; set; }
    public string Color { get; set; } = string.Empty;
}

public class CreateVehiculoHandler : CreateHandler<CreateVehiculoCommand, EVehiculo, VehiculoDto>
{
    public CreateVehiculoHandler(VehicleManagement.Domain.Ports.IGenericRepository<EVehiculo> repository) : base(repository) { }

    protected override VehicleManagement.Domain.Entities.Vehiculo MapToEntity(CreateVehiculoCommand request)
        => new VehicleManagement.Domain.Entities.Vehiculo
        {
            Placa = request.Placa,
            Marca = request.Marca,
            Modelo = request.Modelo,
            Anio = request.Anio,
            Color = request.Color,
            FechaRegistro = DateTime.UtcNow
        };

    protected override VehiculoDto MapToResponse(EVehiculo entity)
        => new VehiculoDto(entity.Id, entity.Placa, entity.Marca, entity.Modelo, entity.Anio, entity.Color, entity.FechaRegistro);
}
