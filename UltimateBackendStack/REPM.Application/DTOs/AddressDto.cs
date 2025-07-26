namespace REPM.Application.DTOs;

public record AddressDto(string Street, string City, string State, string ZipCode) 
{
    // Parameterless constructor for AutoMapper
    public AddressDto() : this(string.Empty, string.Empty, string.Empty, string.Empty)
    {
    }
}