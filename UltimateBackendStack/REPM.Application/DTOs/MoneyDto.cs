namespace REPM.Application.DTOs;

public class MoneyDto
{
    public MoneyDto()
    {
    }

    public MoneyDto(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
}