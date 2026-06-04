using VehicleManagement.Application.UserCase.BaseCommands.Commands.DaleteBase;

namespace VehicleManagement.Application.Vehiculos.Commands;

public class DeleteVehiculoCommand : DeleteCommand<VehicleManagement.Domain.Entities.Vehiculo>
{
    public DeleteVehiculoCommand(string id) : base(id) { }
}
