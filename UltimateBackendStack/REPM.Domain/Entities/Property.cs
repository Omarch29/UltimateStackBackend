using REPM.Domain.DomainEvents;
using REPM.Domain.DomainExceptions;
using REPM.Domain.Enums;
using REPM.Domain.ValueObjects;

namespace REPM.Domain.Entities;

public class Property: BaseEntity
{
    public string Name { get; set; }
    public Address Address { get; set; }
    public Guid OwnerId { get; private set; }
    public bool IsListedForRent { get; private set; }
    
    // Properties of the property
    
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Beds { get; set; }
    public int Baths { get; set; }
    public int SquareFeet { get; set; }
    
    
    private List<Lease> _leases = new();
    public IReadOnlyCollection<Lease> Leases => _leases.AsReadOnly();

    public bool IsAvailable => !IsListedForRent || _leases.All(lease => lease.Status == LeaseStatus.Expired);


    private Property() { } // Required for EF Core

    public Property(string name, Address address, Guid ownerId, string description, decimal price, int beds, int baths, int squareFeet)
    {
        Id = Guid.NewGuid();
        Name = name;
        Address = address ?? throw new ArgumentException("Address is required.");
        OwnerId = ownerId;
        Description = description;
        Price = price;
        Beds = beds;
        Baths = baths;
        SquareFeet = squareFeet;
        IsListedForRent = false;
    }

    public void ChangeAddress(Address newAddress)
    {
        if (newAddress is null)
            throw new ArgumentException("New address cannot be null.");

        Address = newAddress;
    }
    
    public void ListForRent()
    {
        if (IsListedForRent)
            throw new PropertyNotAvailableException("Property is already listed for rent.");
        
        // Check if there are active leases
        if (_leases.Any(lease => lease.Status == LeaseStatus.Active))
            throw new PropertyNotAvailableException("Property has active leases.");
        
        IsListedForRent = true;
        DomainEventDispatcher.Raise(new PropertyListedForRent(Id));
    }

    public void UnlistForRent() => IsListedForRent = false;

    public bool IsAvailableForRent(DateRange period) =>
        _leases.All(lease => !lease.IsOverlapping(period));
}