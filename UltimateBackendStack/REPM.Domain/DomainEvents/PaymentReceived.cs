using REPM.Domain.ValueObjects;

namespace REPM.Domain.DomainEvents;

public record PaymentReceived(Guid PaymentId, Guid LeaseId, Money Amount) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

}