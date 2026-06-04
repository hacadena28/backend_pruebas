using VehicleManagement.Domain.Entities;

namespace VehicleManagement.Domain.Ports.Configuration;

public interface IJwtTokenGenerator
{
    string Generate(User user);
}
