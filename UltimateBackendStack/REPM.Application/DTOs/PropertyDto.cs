namespace REPM.Application.DTOs;

public record PropertyDto(Guid Id, string Name, AddressDto address, string Description, decimal Price, int Beds, int Baths, int SquareFeet, bool IsActive, Guid OwnerId)
{
}