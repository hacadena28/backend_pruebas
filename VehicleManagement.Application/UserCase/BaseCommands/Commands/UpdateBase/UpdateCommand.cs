using MediatR;
using VehicleManagement.Domain.Common.Exceptions;
using VehicleManagement.Domain.Common.Wrappers;
using VehicleManagement.Domain.Entities.Base;
using VehicleManagement.Domain.Ports;

namespace VehicleManagement.Application.UserCase.BaseCommands.Commands.UpdateBase;

public abstract class UpdateCommand<TEntity, TResponse>
    : IRequest<Response<TResponse>>
{
    public string Id { get; set; } = string.Empty;
}

public abstract class UpdateHandler<TQuery, TEntity, TResponse>
    : IRequestHandler<TQuery, Response<TResponse>>
    where TQuery : UpdateCommand<TEntity, TResponse>
    where TEntity : BaseEntity<string>
{
    protected readonly IGenericRepository<TEntity> _repository;

    protected UpdateHandler(IGenericRepository<TEntity> repository)
    {
        _repository = repository;
    }

    public async Task<Response<TResponse>> Handle(
        TQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);

        if (entity == null)
            throw new CustomException(
                $"Entity with id {request.Id} not found"
            );

        await BeforeUpdateAsync(request, entity);

        UpdateEntity(request, entity);

        await _repository.UpdateAsync(entity);

        await AfterUpdateAsync(request, entity);

        var response = MapToResponse(entity);

        return new Response<TResponse>(response);
    }

    protected abstract void UpdateEntity(
        TQuery request,
        TEntity entity
    );

    protected abstract TResponse MapToResponse(
        TEntity entity
    );

    protected virtual Task BeforeUpdateAsync(
        TQuery request,
        TEntity entity
    ) => Task.CompletedTask;

    protected virtual Task AfterUpdateAsync(
        TQuery request,
        TEntity entity
    ) => Task.CompletedTask;
}