namespace REPM.Application.DTOs;

public record PropertyDto(Guid Id, string Name, AddressDto Address, string Description, decimal Price, int Beds, int Baths, int SquareFeet, bool IsActive, Guid OwnerId, UserDto? Owner)
{
}

public record PropertyForUpdateDto(Guid Id, string Name, AddressDto Address, string Description, decimal Price, int Beds, int Baths, int SquareFeet, bool IsActive, Guid OwnerId);