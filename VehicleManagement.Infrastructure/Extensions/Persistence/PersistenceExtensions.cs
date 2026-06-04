using VehicleManagement.Domain.Ports;
using VehicleManagement.Infrastructure.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VehicleManagement.Infrastructure.Extensions.Persistence;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection svc, IConfiguration config)
    {
        svc.AddContextDatabase(config);
        svc.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        svc.AddScoped<IUnitOfWork, UnitOfWork>();

        return svc;
    }
}
