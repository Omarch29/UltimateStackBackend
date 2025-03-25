namespace REPM.Domain.ValueObjects;

public record Address(string Street, string City, string State, string ZipCode, string Country)
{
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Street) || string.IsNullOrWhiteSpace(City) ||
            string.IsNullOrWhiteSpace(State) || string.IsNullOrWhiteSpace(ZipCode) || 
            string.IsNullOrWhiteSpace(Country))
        {
            throw new ArgumentException("All address fields must be provided.");
        }

        if (ZipCode.Length < 3)
        {
            throw new ArgumentException("Zip code must be at least 3 characters.");
        }
    }
}