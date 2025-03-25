using Ardalis.GuardClauses;
using MediatR;
using REPM.Domain.Entities;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Commands;

public record DeleteLeaseCommand(Guid Id) : IRequest<bool>;

public class DeleteLeaseCommandHandler : IRequestHandler<DeleteLeaseCommand, bool>
{
    private readonly IRepository<Lease> _leaseRepository;

    public DeleteLeaseCommandHandler(IRepository<Lease> leaseRepository)
    {
        _leaseRepository = leaseRepository;
    }

    public async Task<bool> Handle(DeleteLeaseCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.Id, nameof(request.Id));

        var lease = await _leaseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (lease == null) throw new NotFoundException(nameof(Lease), request.Id.ToString());

        _leaseRepository.Delete(lease);
        await _leaseRepository.SaveChangesAsync(cancellationToken);
        return true;

    }
}