namespace REPM.Application.DTOs;

public record PropertyWithLeasesDto(Guid Id, string Name, AddressDto Address, List<LeaseDto> Leases, string Description, decimal Price, int Beds, int Baths, int SquareFeet, bool IsActive, Guid OwnerId)
{
    
}