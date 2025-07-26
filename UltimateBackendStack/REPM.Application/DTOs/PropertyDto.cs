namespace REPM.Application.DTOs;

public record PropertyDto(Guid Id, string Name, AddressDto Address, string Description, decimal Price, int Beds, int Baths, int SquareFeet, bool IsActive, Guid OwnerId, UserDto? Owner)
{
    // Parameterless constructor for AutoMapper
    public PropertyDto() : this(Guid.Empty, string.Empty, new AddressDto(), string.Empty, 0m, 0, 0, 0, false, Guid.Empty, null)
    {
    }
}

public record PropertyForUpdateDto(Guid Id, string Name, AddressDto Address, string Description, decimal Price, int Beds, int Baths, int SquareFeet, bool IsActive, Guid OwnerId);