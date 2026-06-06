using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleManagement.Api.Common;
using VehicleManagement.Application.Auth.Commands;

namespace VehicleManagement.Api.Controllers;

/// <summary>
/// Controlador para la autenticación de usuarios.
/// </summary>
public class AuthController : BaseController
{
    /// <summary>
    /// Endpoint para iniciar sesión. Recibe un comando de inicio de sesión con el correo electrónico 
    /// y la contraseña del usuario, y devuelve un token JWT si las credenciales son válidas.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
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