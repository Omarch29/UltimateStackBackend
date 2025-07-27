# REPM MCP Server - Copilot Instructions

This is a Model Context Protocol (MCP) server for the Real Estate Property Management system.

## Project Structure
- This is a .NET console application that implements the MCP protocol
- It exposes real estate management functionality as tools for AI assistants
- Uses STDIO for communication with MCP clients

## Key Components
- **Models/McpModels.cs** - Core MCP protocol models (requests, responses, tools)
- **Tools/RealEstateTools.cs** - Tool definitions with JSON schemas
- **Handlers/RealEstateToolHandler.cs** - Business logic handlers using MediatR
- **Server/McpServer.cs** - Main MCP server implementation
- **Program.cs** - Entry point with STDIO communication loop

## Available Tools
1. list_properties_for_rent - List available rental properties
2. get_property_details - Get specific property information
3. get_users - List all users
4. get_user_properties - Get properties owned by a user
5. get_leases_by_property - Get leases for a property
6. create_lease - Create a new lease
7. make_payment - Record a payment
8. search_properties - Advanced property search

## Integration
- Uses existing CQRS/MediatR pattern from the main application
- Shares the same database and business logic
- Communicates via JSON-RPC over STDIO

## SDK Reference
Based on the MCP specification: https://github.com/modelcontextprotocol/create-python-server

## Running
```bash
dotnet run
```

The server reads JSON-RPC requests from stdin and writes responses to stdout.

## VS Code Integration
Configured in `.vscode/mcp.json` for automatic discovery by VS Code AI assistants.
