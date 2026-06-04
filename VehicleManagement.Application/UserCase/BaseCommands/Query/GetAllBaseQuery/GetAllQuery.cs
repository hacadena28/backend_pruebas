using MediatR;
using Newtonsoft.Json;
using System.Linq.Expressions;
using VehicleManagement.Domain.Common.Wrappers;
using VehicleManagement.Domain.Entities.Base;
using VehicleManagement.Domain.Ports;

namespace VehicleManagement.Application.UserCase.BaseCommands.Query.GetAllBaseQuery;

public class GetAllQuery<TEntity, TResponse> : IRequest<Response<IEnumerable<TResponse>>>
{
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public Expression<Func<TEntity, bool>>? Filter { get; set; }
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy { get; set; }
    public string IncludeProperties { get; set; } = string.Empty;
    public bool IsTracking { get; set; } = false;
}



public abstract class GetAllHandler<TQuery, TEntity, TResponse>
    : IRequestHandler<TQuery, Response<IEnumerable<TResponse>>>
    where TQuery : GetAllQuery<TEntity, TResponse>
    where TEntity : BaseEntity<string>
{
    private readonly IGenericRepository<TEntity> _repository;

    protected GetAllHandler(
        IGenericRepository<TEntity> repository)
    {
        _repository = repository;
    }

    public async Task<Response<IEnumerable<TResponse>>> Handle(
        TQuery request,
        CancellationToken cancellationToken)
    {
        await BeforeHandleAsync(request);

        var result = await _repository.GetAsync(
            request.Filter,
            request.OrderBy,
            request.IncludeProperties,
            request.IsTracking
        );

        await AfterHandleAsync(result);

        var mapped = result.Select(MapToResponse);

        return new Response<IEnumerable<TResponse>>(mapped) { Message = "Consulta exitosa" };
    }

    protected abstract TResponse MapToResponse(TEntity entity);

    protected virtual Task BeforeHandleAsync(TQuery query)
        => Task.CompletedTask;

    protected virtual Task AfterHandleAsync(
        IEnumerable<TEntity> entities)
        => Task.CompletedTask;
}


