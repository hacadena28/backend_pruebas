using VehicleManagement.Application.UserCase.BaseCommands.Query.GetAllBaseQuery;
using VehicleManagement.Application.Vehiculos.Dtos;
using VehicleManagement.Domain.Entities;
using EVehiculo = VehicleManagement.Domain.Entities.Vehiculo;

namespace VehicleManagement.Application.Vehiculos.Queries;

public class GetAllVehiculosQuery : GetAllQuery<VehicleManagement.Domain.Entities.Vehiculo, VehiculoDto>
{
}

public class GetAllVehiculosHandler : GetAllHandler<GetAllVehiculosQuery, VehicleManagement.Domain.Entities.Vehiculo, VehiculoDto>
{
    public GetAllVehiculosHandler(VehicleManagement.Domain.Ports.IGenericRepository<EVehiculo> repository) : base(repository) { }

    protected override VehiculoDto MapToResponse(VehicleManagement.Domain.Entities.Vehiculo entity)
        => new VehiculoDto(entity.Id, entity.Placa, entity.Marca, entity.Modelo, entity.Anio, entity.Color, entity.FechaRegistro);
}
