using MediatR;
using VehicleManagement.Application.Vehiculos.Dtos;
using VehicleManagement.Application.UserCase.BaseCommands.Query.GetByIdBase;
using EVehiculo = VehicleManagement.Domain.Entities.Vehiculo;

namespace VehicleManagement.Application.Vehiculos.Queries;

public class GetVehiculoByIdQuery : GetByIdBaseQuery<VehicleManagement.Domain.Entities.Vehiculo, VehiculoDto>, IRequest<VehicleManagement.Domain.Common.Wrappers.Response<VehiculoDto>>
{
}

public class GetVehiculoByIdHandler : GetByIdBaseHandler<GetVehiculoByIdQuery, VehicleManagement.Domain.Entities.Vehiculo, VehiculoDto>
{
    public GetVehiculoByIdHandler(VehicleManagement.Domain.Ports.IGenericRepository<EVehiculo> repository) : base(repository) { }

    protected override VehiculoDto MapToResponse(VehicleManagement.Domain.Entities.Vehiculo entity)
        => new VehiculoDto(entity.Id, entity.Placa, entity.Marca, entity.Modelo, entity.Anio, entity.Color, entity.FechaRegistro);
}