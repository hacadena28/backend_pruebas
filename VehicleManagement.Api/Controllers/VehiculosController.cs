using Microsoft.AspNetCore.Mvc;
using VehicleManagement.Api.Common;
using VehicleManagement.Application.Vehiculos.Commands;
using VehicleManagement.Application.Vehiculos.Dtos;
using VehicleManagement.Application.Vehiculos.Queries;

namespace VehicleManagement.Api.Controllers;

/// <summary>
/// Controlador para la gestión de vehículos. Proporciona endpoints para crear, obtener, actualizar y eliminar vehículos.
/// </summary>
public class VehiculosController : BaseController
{
    /// <summary>
    /// Obtiene una lista de todos los vehículos registrados en el sistema.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<VehicleManagement.Domain.Common.Wrappers.Response<IEnumerable<VehiculoDto>>>> GetAll()
    {
        var query = new GetAllVehiculosQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }
    /// <summary>
    /// Obtiene un vehículo por su ID. Devuelve los detalles del vehículo solicitado.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleManagement.Domain.Common.Wrappers.Response<VehiculoDto>>> GetById(string id)
    {
        var query = new GetVehiculoByIdQuery { Id = id };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Crea un nuevo vehículo en el sistema. Recibe los detalles del vehículo a través del cuerpo
    /// de la solicitud y devuelve el vehículo creado con su ID asignado.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<VehicleManagement.Domain.Common.Wrappers.Response<VehiculoDto>>> Create([FromBody] CreateVehiculoCommand command)
    {
        var result = await Mediator.Send(command);
        if (result == null || result.Data == null)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
    }
    /// <summary>
    /// Actualiza un vehículo existente en el sistema. Recibe el ID del vehículo a actualizar y los nuevos detalles
    /// </summary>
    /// <param name="id"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<VehicleManagement.Domain.Common.Wrappers.Response<VehiculoDto>>> Update(string id, [FromBody] UpdateVehiculoCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return Ok(result);
    }
    /// <summary>
    /// Elimina un vehículo del sistema. Recibe el ID del vehículo a eliminar y devuelve un resultado indicando si la eliminación fue exitosa.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<VehicleManagement.Domain.Common.Wrappers.Response<bool>>> Delete(string id)
    {
        var command = new DeleteVehiculoCommand(id);
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}
