using MediatR;
using VehicleManagement.Domain.Common.Wrappers;
using VehicleManagement.Domain.Entities.Base;
using VehicleManagement.Domain.Ports;
using System.Linq.Expressions;

namespace VehicleManagement.Application.UserCase.BaseCommands.Query.GetBasePaginatedQuery;

public class GetPaginatedQuery<TEntity, TResponse>
    : IRequest<PaginatedResponse<TResponse>>
{
    public int? NumeroPagina { get; set; } = 1;

    public int? TamanoPagina { get; set; } = 10;
}

public abstract class GetPaginatedHandler<TQuery, TEntity, TResponse>
    : IRequestHandler<TQuery, PaginatedResponse<TResponse>>
    where TQuery : GetPaginatedQuery<TEntity, TResponse>
    where TEntity : BaseEntity<string>
{
    private readonly IGenericRepository<TEntity> _repository;

    protected GetPaginatedHandler(
        IGenericRepository<TEntity> repository)
    {
        _repository = repository;
    }

    public async Task<PaginatedResponse<TResponse>> Handle(
        TQuery request,
        CancellationToken cancellationToken)
    {
        var options = await BuildQueryAsync(request);

        var result = await _repository.GetPaginatedAsync(
            options.Filter,
            options.OrderBy,
            request.NumeroPagina,
            request.TamanoPagina,
            options.IsTracking,
            options.IncludeProperties
        );

        if (result == null || result.Data == null || !result.Data.Any())
        {
            return new PaginatedResponse<TResponse>(
                404,
                "Recursos no encontrados.",
                false
            );
        }

        await AfterPaginedAsync(result.Data);

        var mapped = result.Data
            .Select(MapToResponse)
            .ToList();

        return new PaginatedResponse<TResponse>(
            mapped,
            result.PageNumber,
            result.PageSize,
            result.TotalRecords,
            result.TotalCountRecords
        )
        { Message = "Consulta exitosa", Success = true };
    }

    protected abstract TResponse MapToResponse(
        TEntity entity
    );

    protected virtual Task<PaginatedOptions<TEntity>> BuildQueryAsync(
        TQuery request)
        => Task.FromResult(new PaginatedOptions<TEntity>());

    protected virtual Task AfterPaginedAsync(
        IEnumerable<TEntity> entities)
        => Task.CompletedTask;
}

public class PaginatedOptions<TEntity>
{
    public Expression<Func<TEntity, bool>>? Filter { get; set; }

    public Func<IQueryable<TEntity>,
        IOrderedQueryable<TEntity>>? OrderBy
    { get; set; }

    public string IncludeProperties { get; set; } = string.Empty;

    public bool IsTracking { get; set; } = false;
}