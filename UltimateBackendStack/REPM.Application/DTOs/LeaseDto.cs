namespace REPM.Application.DTOs;

public record LeaseDto(Guid Id, Guid PropertyId, Guid TenantId, DateTime StartDate, DateTime EndDate, MoneyDto Rent, PaymentDto[] Payments, bool IsActive)
{
}