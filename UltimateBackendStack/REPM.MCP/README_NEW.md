# REPM MCP Server

The Real Estate Property Management (REPM) Model Context Protocol (MCP) Server exposes real estate management tools for AI assistants using the .NET MCP SDK.

## Overview

This MCP server provides AI assistants with tools to interact with the real estate property management system, including:

- Property listing and search
- User management
- Lease creation and management
- Payment processing
- Property analytics

## Features

### Available Tools

1. **list_properties_for_rent** - Search and filter available rental properties
2. **get_user_by_id** - Retrieve user information by ID
3. **get_all_users** - Get all users in the system
4. **create_lease** - Create a new lease agreement
5. **get_leases_for_property** - Get all leases for a specific property
6. **make_payment** - Process lease payments
7. **list_property_for_rent** - List a property as available for rent
8. **unlist_property_for_rent** - Remove a property from rental listings

### Architecture

- **Models**: JSON-RPC request/response models for MCP communication
- **Tools**: Tool definitions and parameter schemas
- **Handlers**: Business logic for processing tool requests
- **Server**: STDIO-based MCP server implementation

## Quick Start

### Prerequisites

- .NET 9.0 SDK
- PostgreSQL database (same as main API)
- Valid connection string in `appsettings.json`

### Running the Server

```bash
# Option 1: Use the provided script
./run-mcp.sh

# Option 2: Manual steps
dotnet build
cd bin/Debug/net9.0
./REPM.MCP
```

### VS Code Integration

The server includes VS Code configuration for easy testing and development:

1. Use **F5** to run with debugging
2. Set breakpoints in tool handlers
3. Monitor STDIO communication in the terminal

## Configuration

### Database Connection

Update `appsettings.json` with your PostgreSQL connection:

```json
{
  "ConnectionStrings": {
    "WebApiDatabase": "Host=localhost;Port=5432;Database=realestatetest;Username=omar;Password=rootroot"
  }
}
```

### Logging

Logging is configured for minimal output suitable for MCP STDIO communication:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

## Tool Usage Examples

### List Properties for Rent

```json
{
  "method": "tools/call",
  "params": {
    "name": "list_properties_for_rent",
    "arguments": {
      "city": "Seattle",
      "minPrice": 1000,
      "maxPrice": 3000,
      "minBedrooms": 2
    }
  }
}
```

### Create a Lease

```json
{
  "method": "tools/call",
  "params": {
    "name": "create_lease",
    "arguments": {
      "propertyId": "123e4567-e89b-12d3-a456-426614174000",
      "tenantId": "987fcdeb-51a2-4def-8901-123456789abc",
      "startDate": "2024-01-01",
      "endDate": "2024-12-31",
      "monthlyRent": 2500,
      "currency": "USD"
    }
  }
}
```

### Process Payment

```json
{
  "method": "tools/call",
  "params": {
    "name": "make_payment",
    "arguments": {
      "leaseId": "456e7890-e12b-34d5-a678-901234567def",
      "amount": 2500,
      "currency": "USD",
      "paymentDate": "2024-01-01"
    }
  }
}
```

## Development

### Project Structure

```
REPM.MCP/
├── Models/           # MCP protocol models
├── Tools/            # Tool definitions and schemas  
├── Handlers/         # Business logic for tools
├── Server/           # MCP server implementation
├── Program.cs        # Application entry point
├── appsettings.json  # Configuration
└── run-mcp.sh       # Convenience script
```

### Adding New Tools

1. Define tool schema in `Tools/RealEstateTools.cs`
2. Implement handler logic in `Handlers/RealEstateToolHandler.cs` 
3. Register tool in server initialization

### Testing

Use VS Code debugging or connect with an MCP-compatible AI assistant:

1. Start the server: `./run-mcp.sh`
2. Server communicates via STDIO using JSON-RPC
3. Send tool requests and receive responses

## Troubleshooting

### Connection Issues

- Verify PostgreSQL is running
- Check connection string in `appsettings.json`
- Ensure database exists and user has permissions

### Configuration Not Found

- Run from `bin/Debug/net9.0/` directory
- Verify `appsettings.json` is copied to output directory
- Check file permissions

### STDIO Communication

- Server expects JSON-RPC over STDIO
- Use proper message formatting
- Check for parsing errors in logs

## Integration

This MCP server integrates with the main REPM system:

- **Shared Domain**: Uses same entities and value objects
- **Shared Application**: Leverages CQRS commands and queries  
- **Shared Infrastructure**: Uses same database and repositories

## Support

For issues and questions:

1. Check the troubleshooting section
2. Review VS Code debugging output
3. Verify database connectivity
4. Validate JSON-RPC message format
