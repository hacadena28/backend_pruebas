using VehicleManagement.Domain.Common.Wrappers;
using VehicleManagement.Domain.Entities.Base;
using System.Linq.Expressions;

namespace VehicleManagement.Domain.Ports;

public interface IGenericRepository<E> : IDisposable where E : BaseEntity<string>
{
    Task<E> AddAsync(E entity, params Expression<Func<E, object?>>[] includes);

    Task<IEnumerable<E>> AddAsync(IEnumerable<E> entities);

    Task DeleteAsync(E entity);

    Task DeleteAsync(IEnumerable<E> entities);

    Task<bool> Exist(Expression<Func<E, bool>> filter);

    Task<IEnumerable<E>> GetAsync(
        Expression<Func<E, bool>>? filter = null,
        Func<IQueryable<E>, IOrderedQueryable<E>>? orderBy = null,
        string includeStringProperties = "",
        bool isTracking = false);

    Task<IEnumerable<E>> GetAsync(
        Expression<Func<E, bool>>? filter = null,
        Func<IQueryable<E>, IOrderedQueryable<E>>? orderBy = null,
        bool isTracking = false,
        params Expression<Func<E, object>>[] includeObjectProperties);

    IAsyncEnumerable<E> GetAsyncStream(
        Expression<Func<E, bool>>? filter = null,
        Func<IQueryable<E>, IOrderedQueryable<E>>? orderBy = null,
        string includeStringProperties = "",
        bool isTracking = false);

    Task<E?> GetByIdAsync(object id);

    Task<E?> GetByIdAsync(object id, string includeStringProperties = "", bool isTracking = false);

    Task<E?> GetEntityAsync(Expression<Func<E, bool>> filter, bool isTracking = false);

    Task<List<IGrouping<string, E>>> GetGroupedAsync(Expression<Func<E, string>> keySelector);

    Task<PaginatedResponse<E>> GetPaginatedAsync(
        Expression<Func<E, bool>>? filter = null,
        Func<IQueryable<E>, IOrderedQueryable<E>>? orderBy = null,
        int? pageNumber = null,
        int? pageSize = null,
        bool isTracking = false,
        params Expression<Func<E, object>>[] includeObjectProperties);

    Task<PaginatedResponse<E>> GetPaginatedAsync(
        Expression<Func<E, bool>>? filter = null,
        Func<IQueryable<E>, IOrderedQueryable<E>>? orderBy = null,
        int? pageNumber = null,
        int? pageSize = null,
        bool isTracking = false,
        string includeStringProperties = "");

    string GetUnaccent(string field);

    Task UpdateAsync(E entity);
}
