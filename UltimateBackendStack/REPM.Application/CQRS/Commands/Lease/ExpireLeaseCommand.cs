using Ardalis.GuardClauses;
using MediatR;
using REPM.Domain.Entities;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Commands;

public record ExpireLeaseCommand(Guid Id) : IRequest<bool>;

public class ExpireLeaseCommandHandler : IRequestHandler<ExpireLeaseCommand, bool>
{
    private readonly IRepository<Lease> _leaseRepository;

    public ExpireLeaseCommandHandler(IRepository<Lease> leaseRepository)
    {
        _leaseRepository = leaseRepository;
    }
    public async Task<bool> Handle(ExpireLeaseCommand request, CancellationToken cancellationToken)
    {
       Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.Id, nameof(request.Id));

        var lease = await _leaseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (lease == null) throw new NotFoundException(nameof(Lease), request.Id.ToString());

        lease.Expire();
        return await _leaseRepository.SaveChangesAsync(cancellationToken);
    }
}