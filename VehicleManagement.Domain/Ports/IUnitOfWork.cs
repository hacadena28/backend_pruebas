using VehicleManagement.Domain.Entities.Base;

namespace VehicleManagement.Domain.Ports;

public interface IUnitOfWork
{
    IGenericRepository<E> Repository<E>() where E : BaseEntity<string>;
}
