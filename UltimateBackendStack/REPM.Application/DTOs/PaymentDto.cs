namespace REPM.Application.DTOs;

public record PaymentDto(Guid Id, Guid LeaseId, DateTime PaymentDate, MoneyDto Amount, string Description)
{
    
}

public record PaymentForUpdateDto(Guid Id, Guid LeaseId, DateTime PaymentDate, MoneyDto Amount, string Description);