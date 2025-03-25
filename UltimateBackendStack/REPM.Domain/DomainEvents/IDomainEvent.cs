using MediatR;

namespace REPM.Domain.DomainEvents;

public interface IDomainEvent:  INotification
{
    DateTime OccurredOn { get; }
}