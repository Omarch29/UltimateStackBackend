namespace REPM.Domain.DomainEvents;

public record PropertyListedForRent(Guid PropertyId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}