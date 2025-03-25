namespace REPM.Application.DTOs;

public record PaymentForCreateDto(Guid leaseId, MoneyDto amount, DateTime date);