using VehicleManagement.Application.UserCase.BaseCommands.Commands.UpdateBase;
using VehicleManagement.Application.Vehiculos.Dtos;
using EVehiculo = VehicleManagement.Domain.Entities.Vehiculo;

namespace VehicleManagement.Application.Vehiculos.Commands;

public class UpdateVehiculoCommand : UpdateCommand<VehicleManagement.Domain.Entities.Vehiculo, VehiculoDto>
{
    public string Placa { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Anio { get; set; }
    public string Color { get; set; } = string.Empty;
}

public class UpdateVehiculoHandler : UpdateHandler<UpdateVehiculoCommand, VehicleManagement.Domain.Entities.Vehiculo, VehiculoDto>
{
    public UpdateVehiculoHandler(VehicleManagement.Domain.Ports.IGenericRepository<EVehiculo> repository) : base(repository) { }

    protected override void UpdateEntity(UpdateVehiculoCommand request, VehicleManagement.Domain.Entities.Vehiculo entity)
    {
        entity.Placa = request.Placa;
        entity.Marca = request.Marca;
        entity.Modelo = request.Modelo;
        entity.Anio = request.Anio;
        entity.Color = request.Color;
        entity.MarkAsUpdated();
    }

    protected override VehiculoDto MapToResponse(VehicleManagement.Domain.Entities.Vehiculo entity)
        => new VehiculoDto(entity.Id, entity.Placa, entity.Marca, entity.Modelo, entity.Anio, entity.Color, entity.FechaRegistro);
}
