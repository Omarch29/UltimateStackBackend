using REPM.Domain.DomainEvents;
using REPM.Domain.DomainExceptions;
using REPM.Domain.Enums;
using REPM.Domain.ValueObjects;

namespace REPM.Domain.Entities;
public class Lease
{
    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public Guid TenantId { get; private set; }
    public DateRange LeasePeriod { get; private set; }
    public Money RentAmount { get; private set; }
    public LeaseStatus Status { get; private set; }

    private List<Payment> _payments = new();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    private Lease() { } // Required for EF Core

    public Lease(Guid propertyId, Guid tenantId, DateRange leasePeriod, Money rentAmount)
    {
        if (leasePeriod.Start >= leasePeriod.End)
            throw new InvalidLeasePeriodException("Lease start date must be before end date.");

        Id = Guid.NewGuid();
        PropertyId = propertyId;
        TenantId = tenantId;
        LeasePeriod = leasePeriod;
        RentAmount = rentAmount;
        Status = LeaseStatus.Active;

        DomainEventDispatcher.Raise(new LeaseCreated(Id));
    }

    public bool IsOverlapping(DateRange other) => LeasePeriod.Overlaps(other);
    
    public void Activate()
    {
        if (Status != LeaseStatus.Pending)
            throw new InvalidOperationException("Only pending leases can be activated.");

        Status = LeaseStatus.Active;
    }

    public void Expire()
    {
        if (Status != LeaseStatus.Active)
            throw new InvalidOperationException("Only active leases can expire.");

        Status = LeaseStatus.Expired;
    }

    public void Cancel()
    {
        if (Status == LeaseStatus.Expired)
            throw new InvalidOperationException("Cannot cancel an already expired lease.");

        Status = LeaseStatus.Canceled;
    }
}
