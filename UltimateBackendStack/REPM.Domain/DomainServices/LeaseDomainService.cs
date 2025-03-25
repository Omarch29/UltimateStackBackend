using REPM.Domain.DomainExceptions;
using REPM.Domain.Entities;
using REPM.Domain.Enums;
using REPM.Domain.ValueObjects;

namespace REPM.Domain.DomainServices;

public static class LeaseDomainService
{
    public static Lease CreateLease(User renter, Property property, DateRange leasePeriod, Money rentAmount, List<Lease> activeLeases)
    {
        // 1️⃣ Check if the property is available
        if (!property.IsAvailable)
        {
            throw new PropertyNotAvailableException(property.Id);
        }

        // 2️⃣ Validate lease dates
        if (!leasePeriod.IsValid())
        {
            throw new InvalidLeasePeriodException(leasePeriod);
        }

        // 3️⃣ Check if the renter has unpaid active leases
        if (activeLeases.Any(l => l.Payments.Any(p => p.Status == PaymentStatus.Pending)))
        {
            throw new OverduePaymentException(renter.Id);
        }

        // 4️⃣ Create and return the lease
        return new Lease(property.Id, renter.Id, leasePeriod, rentAmount);
    }
}