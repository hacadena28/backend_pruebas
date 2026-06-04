using MediatR;
using VehicleManagement.Application.Auth.DTOs;

namespace VehicleManagement.Application.Auth.Commands;

using MediatR;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<LoginDto>;