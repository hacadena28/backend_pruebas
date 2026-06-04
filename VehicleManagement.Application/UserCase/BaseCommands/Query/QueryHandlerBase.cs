using VehicleManagement.Domain.Entities.Base;
using VehicleManagement.Domain.Ports;

namespace VehicleManagement.Application.UserCase.BaseCommands.Query;

public abstract class QueryHandlerBase<TEntity, TResponse>
    where TEntity : BaseEntity<string>
{
    protected readonly IGenericRepository<TEntity> Repository;

    protected QueryHandlerBase(IGenericRepository<TEntity> repository)
    {
        Repository = repository;
    }

    /// <summary>
    /// Hook que permite modificar la consulta antes de aplicar filtros.
    /// </summary>
    protected virtual Task<IQueryable<TEntity>> BeforeQueryAsync(IQueryable<TEntity> query, CancellationToken cancellationToken)
    {
        return Task.FromResult(query);
    }

    /// <summary>
    /// Hook que permite modificar la consulta justo antes de ejecutarla.
    /// </summary>
    protected virtual Task<IQueryable<TEntity>> AfterQueryAsync(IQueryable<TEntity> query, CancellationToken cancellationToken)
    {
        return Task.FromResult(query);
    }
}

