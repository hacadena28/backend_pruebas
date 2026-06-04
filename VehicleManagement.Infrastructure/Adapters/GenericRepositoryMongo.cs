using VehicleManagement.Domain.Entities.Base;
using VehicleManagement.Infrastructure.Contexts;

namespace VehicleManagement.Infrastructure.Adapters;

[Obsolete("Use GenericRepository<E>. This type remains only for backwards compatibility.")]
public sealed class GenericRepositoryMongo<E> : GenericRepository<E> where E : BaseEntity<string>
{
    public GenericRepositoryMongo(MongoDbContext context) : base(context)
    {
    }
}
