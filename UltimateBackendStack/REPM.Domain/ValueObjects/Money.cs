namespace REPM.Domain.ValueObjects;

public record Money(decimal Amount, string Currency)
{
    public Money EnsureValid()
    {
        if (Amount < 0)
            throw new ArgumentException("Money amount cannot be negative.");
        if (string.IsNullOrWhiteSpace(Currency))
            throw new ArgumentException("Currency cannot be empty.");
        return this;
    }
}