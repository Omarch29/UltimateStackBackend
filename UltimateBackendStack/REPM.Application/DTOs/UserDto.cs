namespace REPM.Application.DTOs;

public record UserDto(Guid Id, string Name, string Email)
{
    // Parameterless constructor for AutoMapper
    public UserDto() : this(Guid.Empty, string.Empty, string.Empty)
    {
    }
}

public record UserforUpdateDto(Guid Id, string Name, string Email);