# Real Estate Property Management MCP Server

This is a Model Context Protocol (MCP) server that exposes the Real Estate Property Management system functionality as tools for AI assistants.

## What is MCP?

The Model Context Protocol (MCP) is an open protocol that enables secure connections between host applications (like Claude Desktop, IDEs, or other AI tools) and external data sources and tools. This MCP server allows AI assistants to interact with your real estate management system.

## Available Tools

The MCP server exposes the following tools:

### Property Management
- **`list_properties_for_rent`** - List all properties available for rent with filtering options
- **`get_property_details`** - Get detailed information about a specific property
- **`search_properties`** - Advanced search for properties with multiple criteria

### User Management
- **`get_users`** - Get a list of all users in the system
- **`get_user_properties`** - Get properties owned by a specific user

### Lease Management
- **`get_leases_by_property`** - Get all leases for a specific property
- **`create_lease`** - Create a new lease for a property

### Payment Management
- **`make_payment`** - Record a payment for a lease

## Setup and Configuration

### Prerequisites
- .NET 9.0 SDK
- PostgreSQL database
- The main REPM application configured and running

### Database Configuration
The MCP server uses the same database as the main application. Make sure your connection string is configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=realestatetest;Username=postgres;Password=123;"
  }
}
```

### Running the MCP Server

#### For Development/Testing
```bash
cd REPM.MCP
dotnet run
```

#### For VS Code Integration
The server is configured to work with VS Code through the `.vscode/mcp.json` configuration file. The AI assistant in VS Code can automatically discover and use this server.

## Usage Examples

Here are some example interactions you can have with an AI assistant using this MCP server:

### Property Queries
- "Show me all properties available for rent in New York"
- "Find 2-bedroom apartments under $3000"
- "Get details for property ID 12345"

### User and Lease Management
- "List all users in the system"
- "Show me properties owned by user John Smith"
- "Create a lease for property X with tenant Y"

### Payment Tracking
- "Record a rent payment of $2500 for lease 123"
- "Show all leases for property ABC"

## Architecture

The MCP server follows a clean architecture pattern:

- **Models/** - MCP protocol models and data structures
- **Tools/** - Tool definitions and schemas
- **Handlers/** - Business logic handlers that interface with the application layer
- **Server/** - Core MCP server implementation

The server integrates with the existing CQRS pattern using MediatR, ensuring consistency with the main application.

## Development

### Adding New Tools

1. Define the tool in `Tools/RealEstateTools.cs`
2. Implement the handler in `Handlers/RealEstateToolHandler.cs`
3. Add the routing in the `HandleToolCall` method

### Testing

You can test the MCP server by running it and sending JSON-RPC requests via stdin:

```bash
echo '{"jsonrpc": "2.0", "id": "1", "method": "tools/list"}' | dotnet run
```

## Dependencies

This MCP server depends on:
- REPM.Application (for business logic)
- REPM.Infrastructure (for data access)
- REPM.Domain (for domain models)
- MediatR (for CQRS pattern)
- Microsoft.Extensions.Hosting (for hosting)
- System.Text.Json (for JSON serialization)

## Protocol Compliance

This server implements the Model Context Protocol specification and is compatible with:
- Claude Desktop
- VS Code with MCP extensions
- Other MCP-compatible clients

For more information about MCP, visit: https://github.com/modelcontextprotocol
