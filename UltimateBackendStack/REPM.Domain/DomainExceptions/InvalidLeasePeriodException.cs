using REPM.Domain.ValueObjects;

namespace REPM.Domain.DomainExceptions;

public class InvalidLeasePeriodException : Exception
{
    private DateRange LeasePeriod { get; }
    private static string GenerateMessage(DateRange leasePeriod) => $"Lease period from {leasePeriod.Start} to {leasePeriod.End} is invalid.";

    public InvalidLeasePeriodException(DateRange leasePeriod) : base(GenerateMessage(leasePeriod))
    {
        LeasePeriod = leasePeriod;
    }
    
    public InvalidLeasePeriodException(string message) : base(message) { }
}