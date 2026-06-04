using Microsoft.AspNetCore.Mvc;
using VehicleManagement.Api.Common;
using VehicleManagement.Application.Vehiculos.Commands;
using VehicleManagement.Application.Vehiculos.Dtos;
using VehicleManagement.Application.Vehiculos.Queries;

namespace VehicleManagement.Api.Controllers;

[Route(BaseRoute.BaseRouteUrl)]
[ApiController]
public class VehiculosController : BaseController
{
    [HttpGet]
    public async Task<ActionResult<VehicleManagement.Domain.Common.Wrappers.Response<IEnumerable<VehiculoDto>>>> GetAll()
    {
        var query = new GetAllVehiculosQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleManagement.Domain.Common.Wrappers.Response<VehiculoDto>>> GetById(string id)
    {
        var query = new GetVehiculoByIdQuery { Id = id };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<VehicleManagement.Domain.Common.Wrappers.Response<VehiculoDto>>> Create([FromBody] CreateVehiculoCommand command)
    {
        var result = await Mediator.Send(command);
        if (result == null || result.Data == null)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<VehicleManagement.Domain.Common.Wrappers.Response<VehiculoDto>>> Update(string id, [FromBody] UpdateVehiculoCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<VehicleManagement.Domain.Common.Wrappers.Response<bool>>> Delete(string id)
    {
        var command = new DeleteVehiculoCommand(id);
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}
