using VehicleManagement.Domain.Entities.Base;
using VehicleManagement.Domain.Ports;
using VehicleManagement.Infrastructure.Contexts;
using System.Collections.Concurrent;

namespace VehicleManagement.Infrastructure.Adapters;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly MongoDbContext _context;
    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    public UnitOfWork(MongoDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<E> Repository<E>() where E : BaseEntity<string>
    {
        return (IGenericRepository<E>)_repositories.GetOrAdd(
            typeof(E),
            _ => new GenericRepository<E>(_context));
    }
}
