namespace REPM.Application.DTOs;

public record LeaseDto(Guid Id, Guid PropertyId, PropertyDto Property, Guid TenantId, UserDto Tenant,
    DateTime StartDate, DateTime EndDate, MoneyDto Rent, PaymentDto[] Payments, bool IsActive)
{
}

public record LeaseToUpdateDto(Guid Id, Guid PropertyId, Guid TenantId, DateTime StartDate, DateTime EndDate, MoneyDto Rent, bool IsActive);