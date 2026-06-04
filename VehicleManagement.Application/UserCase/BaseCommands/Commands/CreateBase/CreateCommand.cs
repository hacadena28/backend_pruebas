using MediatR;
using VehicleManagement.Domain.Common.Wrappers;
using VehicleManagement.Domain.Entities.Base;
using VehicleManagement.Domain.Ports;

namespace VehicleManagement.Application.UserCase.BaseCommands.Commands.CreateBase;

public abstract class CreateCommand<TEntity, TResponse> : IRequest<Response<TResponse>>
{

}


public abstract class CreateHandler<TQuery, TEntity, TResponse>
    : IRequestHandler<TQuery, Response<TResponse>>
    where TQuery : CreateCommand<TEntity, TResponse>
    where TEntity : BaseEntity<string>
{
    protected readonly IGenericRepository<TEntity> _repository;

    protected CreateHandler(IGenericRepository<TEntity> repository)
    {
        _repository = repository;
    }

    public async Task<Response<TResponse>> Handle(TQuery request, CancellationToken cancellationToken)
    {
        await BeforeCreateAsync(request);

        var entity = MapToEntity(request);

        var created = await _repository.AddAsync(entity);

        await AfterCreateAsync(created);

        var response = MapToResponse(created);

        return new Response<TResponse>(response);
    }
    protected abstract TEntity MapToEntity(TQuery request);
    protected abstract TResponse MapToResponse(TEntity entity);
    protected virtual Task BeforeCreateAsync(TQuery request) => Task.CompletedTask;
    protected virtual Task AfterCreateAsync(TEntity entity) => Task.CompletedTask;
}

