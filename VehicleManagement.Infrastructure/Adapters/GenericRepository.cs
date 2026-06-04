using VehicleManagement.Domain.Common.Exceptions;
using VehicleManagement.Domain.Common.Wrappers;
using VehicleManagement.Domain.Entities.Base;
using VehicleManagement.Domain.Ports;
using VehicleManagement.Infrastructure.Contexts;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;

namespace VehicleManagement.Infrastructure.Adapters;

public class GenericRepository<E> : IGenericRepository<E> where E : BaseEntity<string>
{
    private readonly IMongoCollection<E> _collection;

    public GenericRepository(MongoDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _collection = context.Collection<E>();
    }

    public async Task<E> AddAsync(E entity, params Expression<Func<E, object?>>[] includes)
    {
        ArgumentNullException.ThrowIfNull(entity);

        EnsureId(entity);
        entity.MarkAsCreated();

        await _collection.InsertOneAsync(entity).ConfigureAwait(false);
        return entity;
    }

    public async Task<IEnumerable<E>> AddAsync(IEnumerable<E> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        var documents = entities.ToList();
        foreach (var entity in documents)
        {
            EnsureId(entity);
            entity.MarkAsCreated();
        }

        if (documents.Count > 0)
        {
            await _collection.InsertManyAsync(documents).ConfigureAwait(false);
        }

        return documents;
    }

    public async Task DeleteAsync(IEnumerable<E> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        foreach (var entity in entities)
        {
            await DeleteAsync(entity).ConfigureAwait(false);
        }
    }

    public async Task DeleteAsync(E entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        entity.MarkAsDeleted();
        entity.SetDelete();
        entity.MarkAsUpdated();

        var result = await _collection.ReplaceOneAsync(
            document => document.Id == entity.Id,
            entity).ConfigureAwait(false);

        if (result.MatchedCount == 0)
        {
            throw new NotFoundException($"No se encontro ningun registro con id {entity.Id}.");
        }
    }

    public async Task<IEnumerable<E>> GetAsync(
        Expression<Func<E, bool>>? filter = null,
        Func<IQueryable<E>, IOrderedQueryable<E>>? orderBy = null,
        string includeStringProperties = "",
        bool isTracking = false)
    {
        var query = CreateQueryable(filter);

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return await query.ToListAsync().ConfigureAwait(false);
    }

    public async Task<IEnumerable<E>> GetAsync(
        Expression<Func<E, bool>>? filter = null,
        Func<IQueryable<E>, IOrderedQueryable<E>>? orderBy = null,
        bool isTracking = false,
        params Expression<Func<E, object>>[] includeObjectProperties)
    {
        return await GetAsync(filter, orderBy, string.Empty, isTracking).ConfigureAwait(false);
    }

    public async Task<E?> GetByIdAsync(object id)
    {
        var normalizedId = NormalizeId(id);
        if (normalizedId is null)
        {
            return null;
        }

        return await _collection
            .Find(document => document.Id == normalizedId && !document.IsDeleted)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
    }

    public async Task<E?> GetByIdAsync(object id, string includeStringProperties = "", bool isTracking = false)
    {
        return await GetByIdAsync(id).ConfigureAwait(false);
    }

    public async Task<bool> Exist(Expression<Func<E, bool>> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return await CreateQueryable(filter).AnyAsync().ConfigureAwait(false);
    }

    public async Task UpdateAsync(E entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        entity.MarkAsUpdated();

        var result = await _collection.ReplaceOneAsync(
            document => document.Id == entity.Id && !document.IsDeleted,
            entity).ConfigureAwait(false);

        if (result.MatchedCount == 0)
        {
            throw new NotFoundException($"No se encontro ningun registro con id {entity.Id}.");
        }
    }

    public async Task<PaginatedResponse<E>> GetPaginatedAsync(
        Expression<Func<E, bool>>? filter = null,
        Func<IQueryable<E>, IOrderedQueryable<E>>? orderBy = null,
        int? pageNumber = null,
        int? pageSize = null,
        bool isTracking = false,
        params Expression<Func<E, object>>[] includeObjectProperties)
    {
        return await GetPaginatedAsync(filter, orderBy, pageNumber, pageSize, isTracking, string.Empty)
            .ConfigureAwait(false);
    }

    public async Task<PaginatedResponse<E>> GetPaginatedAsync(
        Expression<Func<E, bool>>? filter = null,
        Func<IQueryable<E>, IOrderedQueryable<E>>? orderBy = null,
        int? pageNumber = null,
        int? pageSize = null,
        bool isTracking = false,
        string includeStringProperties = "")
    {
        var allActiveRecords = await _collection
            .CountDocumentsAsync(document => !document.IsDeleted)
            .ConfigureAwait(false);

        var query = CreateQueryable(filter);
        if (orderBy != null)
        {
            query = orderBy(query);
        }

        var totalRecords = await query.CountAsync().ConfigureAwait(false);
        var normalizedPageNumber = Math.Max(pageNumber ?? 1, 1);
        var normalizedPageSize = Math.Max(pageSize ?? (int)Math.Max(totalRecords, 1), 1);

        var data = await query
            .Skip((normalizedPageNumber - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .ToListAsync()
            .ConfigureAwait(false);

        return new PaginatedResponse<E>(
            data,
            normalizedPageNumber,
            normalizedPageSize,
            (int)totalRecords,
            (int)allActiveRecords);
    }

    public async Task<List<IGrouping<string, E>>> GetGroupedAsync(Expression<Func<E, string>> keySelector)
    {
        ArgumentNullException.ThrowIfNull(keySelector);

        var data = await CreateQueryable().ToListAsync().ConfigureAwait(false);
        return data.GroupBy(keySelector.Compile()).ToList();
    }

    public async Task<E?> GetEntityAsync(Expression<Func<E, bool>> filter, bool isTracking = false)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return await CreateQueryable(filter).FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public async IAsyncEnumerable<E> GetAsyncStream(
        Expression<Func<E, bool>>? filter = null,
        Func<IQueryable<E>, IOrderedQueryable<E>>? orderBy = null,
        string includeStringProperties = "",
        bool isTracking = false)
    {
        var results = await GetAsync(filter, orderBy, includeStringProperties, isTracking).ConfigureAwait(false);

        foreach (var entity in results)
        {
            yield return entity;
        }
    }

    public string GetUnaccent(string field)
    {
        if (string.IsNullOrWhiteSpace(field))
        {
            throw new CustomException("Campo no puede ser null o vacio.");
        }

        var normalized = field.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private IQueryable<E> CreateQueryable(Expression<Func<E, bool>>? filter = null)
    {
        IQueryable<E> query = _collection.AsQueryable().Where(document => !document.IsDeleted);

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return query;
    }

    private static void EnsureId(E entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Id))
        {
            entity.Id = ObjectId.GenerateNewId().ToString();
        }
    }

    private static string? NormalizeId(object id)
    {
        var value = id.ToString();
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return ObjectId.TryParse(value, out _) ? value : null;
    }
}
