using MediatR;
using Newtonsoft.Json;
using VehicleManagement.Domain.Common.Exceptions;
using VehicleManagement.Domain.Common.Wrappers;
using VehicleManagement.Domain.Entities.Base;
using VehicleManagement.Domain.Ports;

namespace VehicleManagement.Application.UserCase.BaseCommands.Query.GetByIdBase;

public class GetByIdBaseQuery<TEntity, TResponse> : IRequest<Response<TResponse>>
{
    public string Id { get; set; } = string.Empty;
    [JsonIgnore]
    public string IncludeProperties { get; set; } = string.Empty;
    [JsonIgnore]
    public bool IsTracking { get; set; } = false;
}


public abstract class GetByIdBaseHandler<TQuery, TEntity, TResponse>
    : IRequestHandler<TQuery, Response<TResponse>>
    where TQuery : GetByIdBaseQuery<TEntity, TResponse>
    where TEntity : BaseEntity<string>
{
    private readonly IGenericRepository<TEntity> _repository;

    protected GetByIdBaseHandler(
        IGenericRepository<TEntity> repository)
    {
        _repository = repository;
    }

    public async Task<Response<TResponse>> Handle(
        TQuery request,
        CancellationToken cancellationToken)
    {
        await BeforeHandleAsync(request);

        var entity = await _repository.GetByIdAsync(
            request.Id,
            request.IncludeProperties,
            request.IsTracking
        );

        if (entity is null)
            throw new CustomException(
                $"Entity with id {request.Id} not found"
            );

        await AfterHandleAsync(entity);

        var response = MapToResponse(entity);

        return new Response<TResponse>(response) { Message = "Consulta exitosa", Success = true, StatusCode = 200 };
    }

    protected abstract TResponse MapToResponse(
        TEntity entity
    );

    protected virtual Task BeforeHandleAsync(
        TQuery query)
        => Task.CompletedTask;

    protected virtual Task AfterHandleAsync(
        TEntity entity)
        => Task.CompletedTask;
}

