using MediatR;

namespace REPM.Domain.DomainEvents;

public static class DomainEventDispatcher
{
    private static IMediator? _mediator;

    /// <summary>
    /// Initializes the MediatR instance.
    /// Should be set at application startup.
    /// </summary>
    public static void Initialize(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Raises a domain event using MediatR.
    /// </summary>
    public static async Task Raise(IDomainEvent domainEvent)
    {
        if (_mediator == null)
        {
            throw new InvalidOperationException("DomainEvents mediator is not initialized.");
        }

        await _mediator.Publish(domainEvent);
    }
}