using REPM.Domain.DomainExceptions;

namespace REPM.Domain.ValueObjects;

public record DateRange(DateTime Start, DateTime End)
{
    public bool Overlaps(DateRange other) =>
        Start < other.End && End > other.Start;

    public void Validate()
    {
        if (Start >= End)
            throw new InvalidLeasePeriodException("Start date must be before end date.");
    }

    public bool IsValid()
    {
        return Start < End;
    }
}