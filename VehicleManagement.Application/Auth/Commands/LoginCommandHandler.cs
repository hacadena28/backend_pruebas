using MediatR;
using VehicleManagement.Application.Auth.DTOs;
using VehicleManagement.Domain.Common.Exceptions;
using VehicleManagement.Domain.Entities;
using VehicleManagement.Domain.Ports;
using VehicleManagement.Domain.Ports.Configuration;

namespace VehicleManagement.Application.Auth.Commands;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginDto>
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IGenericRepository<User> userRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginDto> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetEntityAsync(x=> x.Email == request.Email);

        if (user is null)
            throw new UnauthorizedException(
                "Credenciales inválidas");

        var validPassword = user.PasswordHash == request.Password;

        if (!validPassword)
            throw new UnauthorizedException(
                "Credenciales inválidas");

        var token = _jwtTokenGenerator.Generate(user);

        return new LoginDto
        {
            Token = token,
        };
    }
}