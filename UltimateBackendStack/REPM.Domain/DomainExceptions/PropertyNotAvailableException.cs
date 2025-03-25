namespace REPM.Domain.DomainExceptions;

public class PropertyNotAvailableException : Exception
{
    private Guid PropertyId { get; }
    private static string GenerateMessage(Guid propertyId) => $"Property with ID {propertyId} is not available for rent.";

    public PropertyNotAvailableException(Guid propertyId) : base(GenerateMessage(propertyId))
    {
        PropertyId = propertyId;
    }
    
    public PropertyNotAvailableException(string message) : base(message) { }
}