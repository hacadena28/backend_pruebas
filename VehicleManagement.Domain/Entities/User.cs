using VehicleManagement.Domain.Entities.Base;

namespace VehicleManagement.Domain.Entities;

public class User: BaseEntity<string>
{
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string FullName { get; set; } = default!;
}
