using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleManagement.Api.Common;
using VehicleManagement.Application.Auth.Commands;

namespace VehicleManagement.Api.Controllers;

[Route(BaseRoute.BaseRouteUrl)]
[ApiController]
public class AuthController : BaseController
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var response = await Mediator.Send(command);

        if (response == null)
        {
            return Unauthorized(new { Message = "Usuario o contraseña incorrectos." });
        }

        return Ok(response);
    }
}