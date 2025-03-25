namespace REPM.Domain.Entities;

public class User: BaseEntity
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    
    private User() { } // Required for EF Core

    public User(string name, string email)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
    }
}