using Ardalis.GuardClauses;
using MediatR;
using REPM.Domain.Entities;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Commands;

public record DeletePropertyCommand(Guid Id) : IRequest<bool>;

public class DeletePropertyCommandHandler : IRequestHandler<DeletePropertyCommand, bool>
{
    private readonly IRepository<Property> _propertyRepository;

    public DeletePropertyCommandHandler(IRepository<Property> propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<bool> Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.Id, nameof(request.Id));

        var property = await _propertyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (property == null) throw new NotFoundException(nameof(Property), request.Id.ToString());

        _propertyRepository.Delete(property);
        await _propertyRepository.SaveChangesAsync(cancellationToken);
        return true;

    }
}