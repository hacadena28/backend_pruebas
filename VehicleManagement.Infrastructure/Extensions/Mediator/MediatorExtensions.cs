using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace VehicleManagement.Infrastructure.Extensions.Mediator;

public static class MediatorExtensions
{
    public static IServiceCollection AddMediator(this IServiceCollection svc)
    {
        var assembly = Assembly.Load(ApiConstants.ApplicationProject);
        svc.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        return svc;
    }
}
