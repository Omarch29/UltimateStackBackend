namespace REPM.Domain.Entities;

public class User: BaseEntity
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    
    // Navigation properties
    private List<Lease> _leases = new();
    public IReadOnlyCollection<Lease> Leases => _leases.AsReadOnly();
    
    private List<Property> _properties = new();
    public IReadOnlyCollection<Property> Properties => _properties.AsReadOnly();
    
    private User() { } // Required for EF Core

    public User(string name, string email)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
    }
}