using Ardalis.GuardClauses;
using MediatR;
using REPM.Domain.Entities;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Commands;

public record CancelLeaseCommand(Guid LeaseId) : IRequest<bool>;

public class CancelLeaseCommandHandler : IRequestHandler<CancelLeaseCommand, bool>
{
    private readonly IRepository<Lease> _leaseRepository;

    public CancelLeaseCommandHandler(IRepository<Lease> leaseRepository)
    {
        _leaseRepository = leaseRepository;
    }

    public async Task<bool> Handle(CancelLeaseCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.LeaseId, nameof(request.LeaseId));

        var lease = await _leaseRepository.GetByIdAsync(request.LeaseId, cancellationToken);
        if (lease == null) throw new NotFoundException(nameof(Lease), request.LeaseId.ToString());

        lease.Cancel();
        return await _leaseRepository.SaveChangesAsync(cancellationToken);
    }
}