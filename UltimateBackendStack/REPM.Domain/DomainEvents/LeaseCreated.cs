using REPM.Domain.ValueObjects;

namespace REPM.Domain.DomainEvents;

public class LeaseCreated : IDomainEvent
{
    public Guid LeaseId { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public LeaseCreated(Guid leaseId)
    {
        LeaseId = leaseId;
    }
}