using REPM.Domain.DomainEvents;
using REPM.Domain.DomainExceptions;
using REPM.Domain.Enums;
using REPM.Domain.ValueObjects;

namespace REPM.Domain.Entities;

public class Payment: BaseEntity
{
    public Guid LeaseId { get; private set; }
    public Lease Lease { get; private set; }
    public Money Amount { get; private set; }
    public DateTime Date { get; private set; }
    public PaymentStatus Status { get; private set; }

    private Payment() { } // Required for EF Core

    public Payment(Guid leaseId, Money amount, DateTime date)
    {
        if (date > DateTime.UtcNow)
            throw new OverduePaymentException("Payment date cannot be in the future.");

        Id = Guid.NewGuid();
        LeaseId = leaseId;
        Amount = amount;
        Date = date;
        Status = PaymentStatus.Completed;

        DomainEventDispatcher.Raise(new PaymentReceived(Id, LeaseId, Amount));
    }
    
    public void Cancel()
    {
        if (Status == PaymentStatus.Canceled)
            throw new PaymentAlreadyCancelledException("Payment is already cancelled.");

        Status = PaymentStatus.Canceled;
    }
    
    public void MarkAsCompleted()
    {
        if (Status == PaymentStatus.Completed)
            throw new PaymentAlreadyCompletedException("Payment is already completed.");

        Status = PaymentStatus.Completed;
    }
    
}