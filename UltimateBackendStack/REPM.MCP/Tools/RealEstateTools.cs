using REPM.MCP.Models;

namespace REPM.MCP.Tools;

public static class RealEstateTools
{
    public static List<Tool> GetAllTools()
    {
        return new List<Tool>
        {
            GetListPropertiesForRentTool(),
            GetPropertyDetailsTool(),
            GetUsersTool(),
            GetUserPropertiesTool(),
            GetLeasesByPropertyTool(),
            CreateLeaseTool(),
            CreatePropertyTool(),
            MakePaymentTool(),
            SearchPropertiesTool()
        };
    }

    private static Tool GetListPropertiesForRentTool()
    {
        return new Tool
        {
            Name = "list_properties_for_rent",
            Description = "List all properties available for rent with optional filtering",
            InputSchema = new
            {
                type = "object",
                properties = new
                {
                    city = new { type = "string", description = "Filter by city" },
                    minPrice = new { type = "number", description = "Minimum price filter" },
                    maxPrice = new { type = "number", description = "Maximum price filter" },
                    bedrooms = new { type = "integer", description = "Number of bedrooms" },
                    propertyType = new { type = "string", description = "Type of property (House, Apartment, Condo, etc.)" }
                },
                additionalProperties = false
            }
        };
    }

    private static Tool GetPropertyDetailsTool()
    {
        return new Tool
        {
            Name = "get_property_details",
            Description = "Get detailed information about a specific property",
            InputSchema = new
            {
                type = "object",
                properties = new
                {
                    propertyId = new { type = "string", description = "The ID of the property to get details for" }
                },
                required = new[] { "propertyId" },
                additionalProperties = false
            }
        };
    }

    private static Tool GetUsersTool()
    {
        return new Tool
        {
            Name = "get_users",
            Description = "Get a list of all users in the system",
            InputSchema = new
            {
                type = "object",
                properties = new { },
                additionalProperties = false
            }
        };
    }

    private static Tool GetUserPropertiesTool()
    {
        return new Tool
        {
            Name = "get_user_properties",
            Description = "Get properties owned by a specific user",
            InputSchema = new
            {
                type = "object",
                properties = new
                {
                    userId = new { type = "string", description = "The ID of the user" }
                },
                required = new[] { "userId" },
                additionalProperties = false
            }
        };
    }

    private static Tool GetLeasesByPropertyTool()
    {
        return new Tool
        {
            Name = "get_leases_by_property",
            Description = "Get all leases for a specific property",
            InputSchema = new
            {
                type = "object",
                properties = new
                {
                    propertyId = new { type = "string", description = "The ID of the property" }
                },
                required = new[] { "propertyId" },
                additionalProperties = false
            }
        };
    }

    private static Tool CreateLeaseTool()
    {
        return new Tool
        {
            Name = "create_lease",
            Description = "Create a new lease for a property",
            InputSchema = new
            {
                type = "object",
                properties = new
                {
                    propertyId = new { type = "string", description = "The ID of the property" },
                    tenantId = new { type = "string", description = "The ID of the tenant" },
                    startDate = new { type = "string", format = "date", description = "Start date of the lease (YYYY-MM-DD)" },
                    endDate = new { type = "string", format = "date", description = "End date of the lease (YYYY-MM-DD)" },
                    monthlyRent = new { type = "number", description = "Monthly rent amount" },
                    currency = new { type = "string", description = "Currency (e.g., USD)", @default = "USD" }
                },
                required = new[] { "propertyId", "tenantId", "startDate", "endDate", "monthlyRent" },
                additionalProperties = false
            }
        };
    }

    private static Tool MakePaymentTool()
    {
        return new Tool
        {
            Name = "make_payment",
            Description = "Record a payment for a lease",
            InputSchema = new
            {
                type = "object",
                properties = new
                {
                    leaseId = new { type = "string", description = "The ID of the lease" },
                    amount = new { type = "number", description = "Payment amount" },
                    currency = new { type = "string", description = "Currency (e.g., USD)", @default = "USD" },
                    paymentDate = new { type = "string", format = "date", description = "Date of payment (YYYY-MM-DD)" },
                    description = new { type = "string", description = "Payment description", @default = "Monthly rent payment" }
                },
                required = new[] { "leaseId", "amount", "paymentDate" },
                additionalProperties = false
            }
        };
    }

    private static Tool SearchPropertiesTool()
    {
        return new Tool
        {
            Name = "search_properties",
            Description = "Advanced search for properties with multiple criteria",
            InputSchema = new
            {
                type = "object",
                properties = new
                {
                    searchQuery = new { type = "string", description = "General search query" },
                    city = new { type = "string", description = "City to search in" },
                    state = new { type = "string", description = "State to search in" },
                    minPrice = new { type = "number", description = "Minimum price" },
                    maxPrice = new { type = "number", description = "Maximum price" },
                    minBedrooms = new { type = "integer", description = "Minimum number of bedrooms" },
                    maxBedrooms = new { type = "integer", description = "Maximum number of bedrooms" },
                    propertyType = new { type = "string", description = "Property type" },
                    isAvailable = new { type = "boolean", description = "Only show available properties", @default = true }
                },
                additionalProperties = false
            }
        };
    }

    private static Tool CreatePropertyTool()
    {
        return new Tool
        {
            Name = "create_property",
            Description = "Create a new property in the system",
            InputSchema = new
            {
                type = "object",
                properties = new
                {
                    name = new { type = "string", description = "Property name or title" },
                    description = new { type = "string", description = "Property description" },
                    price = new { type = "number", description = "Property price" },
                    beds = new { type = "integer", description = "Number of bedrooms" },
                    baths = new { type = "integer", description = "Number of bathrooms" },
                    squareFeet = new { type = "integer", description = "Property size in square feet" },
                    isActive = new { type = "boolean", description = "Whether the property is active", @default = true },
                    ownerId = new { type = "string", description = "ID of the property owner" },
                    address = new
                    {
                        type = "object",
                        description = "Property address",
                        properties = new
                        {
                            street = new { type = "string", description = "Street address" },
                            city = new { type = "string", description = "City" },
                            state = new { type = "string", description = "State" },
                            zipCode = new { type = "string", description = "ZIP code" }
                        },
                        required = new[] { "street", "city", "state", "zipCode" },
                        additionalProperties = false
                    }
                },
                required = new[] { "name", "description", "price", "beds", "baths", "squareFeet", "ownerId", "address" },
                additionalProperties = false
            }
        };
    }
}
