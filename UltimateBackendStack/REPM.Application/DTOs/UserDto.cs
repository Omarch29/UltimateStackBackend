namespace REPM.Application.DTOs;

public record UserDto(Guid Id, string Name, string Email);

public record UserforUpdateDto(Guid Id, string Name, string Email);