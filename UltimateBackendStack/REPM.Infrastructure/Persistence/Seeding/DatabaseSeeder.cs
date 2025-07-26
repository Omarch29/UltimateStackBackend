using REPM.Domain.Entities;
using REPM.Domain.ValueObjects;
using REPM.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace REPM.Infrastructure.Persistence.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(REPMContext context, ILogger logger)
    {
        try
        {
            // Check if database has any users
            if (await context.Users.AnyAsync())
            {
                logger.LogInformation("Database already contains data. Skipping seeding.");
                return;
            }

            logger.LogInformation("Starting database seeding...");

            // Seed Users
            var users = SeedUsers();
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            // Seed Properties
            var properties = SeedProperties(users);
            
            // List all properties for rent
            foreach (var property in properties)
            {
                property.ListForRent();
            }
            
            await context.Properties.AddRangeAsync(properties);
            await context.SaveChangesAsync();

            logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private static List<User> SeedUsers()
    {
        return new List<User>
        {
            new User("John Smith", "john.smith@example.com"),
            new User("Jane Doe", "jane.doe@example.com"),
            new User("Michael Johnson", "michael.johnson@example.com"),
            new User("Sarah Wilson", "sarah.wilson@example.com"),
            new User("David Brown", "david.brown@example.com"),
            new User("Emily Davis", "emily.davis@example.com"),
            new User("Robert Miller", "robert.miller@example.com"),
            new User("Lisa Anderson", "lisa.anderson@example.com"),
            new User("Christopher Taylor", "christopher.taylor@example.com"),
            new User("Amanda Martinez", "amanda.martinez@example.com")
        };
    }

    private static List<Property> SeedProperties(List<User> users)
    {
        return new List<Property>
        {
            new Property(
                "Modern Downtown Apartment",
                new Address("123 Main St", "New York", "NY", "10001", "USA"),
                users[0].Id,
                "Beautiful 2-bedroom apartment in the heart of downtown with city views and modern amenities.",
                2500.00m,
                2,
                2,
                1200
            ),
            new Property(
                "Cozy Suburban House",
                new Address("456 Oak Avenue", "Los Angeles", "CA", "90210", "USA"),
                users[1].Id,
                "Charming 3-bedroom house with a backyard, perfect for families.",
                3200.00m,
                3,
                2,
                1800
            ),
            new Property(
                "Luxury Penthouse",
                new Address("789 Park Place", "Miami", "FL", "33101", "USA"),
                users[0].Id,
                "Stunning penthouse with panoramic ocean views and premium finishes.",
                5500.00m,
                4,
                3,
                2800
            ),
            new Property(
                "Student-Friendly Studio",
                new Address("321 University Blvd", "Austin", "TX", "73301", "USA"),
                users[2].Id,
                "Compact studio apartment near the university campus, perfect for students.",
                900.00m,
                1,
                1,
                450
            ),
            new Property(
                "Family Home with Garden",
                new Address("654 Maple Street", "Seattle", "WA", "98101", "USA"),
                users[3].Id,
                "Spacious 4-bedroom family home with a large garden and garage.",
                4200.00m,
                4,
                3,
                2400
            ),
            new Property(
                "Loft in Arts District",
                new Address("987 Industrial Way", "Portland", "OR", "97201", "USA"),
                users[4].Id,
                "Trendy loft apartment in the arts district with exposed brick and high ceilings.",
                2800.00m,
                2,
                1,
                1600
            ),
            new Property(
                "Beachfront Condo",
                new Address("147 Ocean Drive", "San Diego", "CA", "92101", "USA"),
                users[1].Id,
                "Beautiful beachfront condominium with direct beach access and ocean views.",
                4800.00m,
                3,
                2,
                1900
            ),
            new Property(
                "Historic Brownstone",
                new Address("258 Heritage Lane", "Boston", "MA", "02101", "USA"),
                users[5].Id,
                "Charming historic brownstone with original details and modern updates.",
                3800.00m,
                3,
                2,
                2100
            ),
            new Property(
                "Mountain View Cabin",
                new Address("369 Pine Ridge Road", "Denver", "CO", "80201", "USA"),
                users[6].Id,
                "Rustic cabin with stunning mountain views, perfect for nature lovers.",
                2200.00m,
                2,
                1,
                1100
            ),
            new Property(
                "Urban High-Rise",
                new Address("741 Skyline Tower", "Chicago", "IL", "60601", "USA"),
                users[7].Id,
                "Modern high-rise apartment with city skyline views and luxury amenities.",
                3600.00m,
                2,
                2,
                1400
            ),
            new Property(
                "Countryside Cottage",
                new Address("852 Rural Route 5", "Nashville", "TN", "37201", "USA"),
                users[8].Id,
                "Quaint countryside cottage surrounded by nature, perfect for peaceful living.",
                1800.00m,
                2,
                1,
                1000
            ),
            new Property(
                "Tech Hub Apartment",
                new Address("963 Innovation Drive", "San Francisco", "CA", "94101", "USA"),
                users[9].Id,
                "Modern apartment in the heart of the tech district with smart home features.",
                4500.00m,
                3,
                2,
                1700
            )
        };
    }
}
