using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleManagement.Api.Common;

namespace VehicleManagement.Api.Controllers;

/// <summary>
/// Base Controller 
/// </summary>

[Route(BaseRoute.BaseRouteUrl)]
[ApiController]
[Authorize]
public class BaseController : ControllerBase
{
    private IMediator _mediator = null!;
    /// <summary>
    ///  Mediator 
    /// </summary>
    public IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>()!;
}
