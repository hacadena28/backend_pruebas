using MediatR;
using VehicleManagement.Application.UserCase.BaseCommands.Commands.CreateBase;
using VehicleManagement.Domain.Common.Exceptions;
using VehicleManagement.Domain.Common.Wrappers;
using VehicleManagement.Domain.Entities.Base;
using VehicleManagement.Domain.Ports;

namespace VehicleManagement.Application.UserCase.BaseCommands.Commands.DaleteBase;

public class DeleteCommand<TEntity> : IRequest<Response<bool>>
{
    public string Id { get; set; }
    public DeleteCommand(string id) => Id = id;
}

public class DeleteHandler<TQuery, TEntity>
    : IRequestHandler<TQuery, Response<bool>>
    where TQuery : DeleteCommand<TEntity>
    where TEntity : BaseEntity<string>
{
    protected readonly IGenericRepository<TEntity> _repository;

    public DeleteHandler(IGenericRepository<TEntity> repository)
    {
        _repository = repository;
    }

    public async Task<Response<bool>> Handle(TQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        if (entity == null)
            throw new CustomException($"No se encontro ningun registo con {request.Id}");

        await _repository.DeleteAsync(entity);
        return new Response<bool>(true) { Message = "Eliminado exitosamente" };
    }
}
