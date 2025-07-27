# 🤖 REPM MCP Server

The Real Estate Property Management (REPM) Model Context Protocol (MCP) Server exposes real estate management tools for AI assistants using the .NET MCP SDK. This server enables natural language interaction with the REPM system through AI assistants like Claude.

## 🏗️ MCP Architecture

```mermaid
graph TB
    subgraph "AI Assistant Layer"
        CLAUDE[Claude AI]
        CHATGPT[ChatGPT]
        OTHER[Other AI Assistants]
    end
    
    subgraph "MCP Protocol Layer"
        MCP_SERVER[MCP Server]
        STDIO[STDIN/STDOUT]
        JSON_RPC[JSON-RPC Protocol]
    end
    
    subgraph "Tool Processing Layer"
        TOOLS[Tool Definitions]
        HANDLERS[Tool Handlers]
        VALIDATION[Input Validation]
        ROUTING[Request Routing]
    end
    
    subgraph "Application Integration"
        MEDIATOR[MediatR Pipeline]
        COMMANDS[Commands]
        QUERIES[Queries]
        APP_LAYER[Application Layer]
    end
    
    subgraph "Data Layer"
        REPO[Repositories]
        DB[(PostgreSQL)]
    end
    
    CLAUDE --> STDIO
    CHATGPT --> STDIO
    OTHER --> STDIO
    
    STDIO --> JSON_RPC
    JSON_RPC --> MCP_SERVER
    
    MCP_SERVER --> TOOLS
    TOOLS --> HANDLERS
    HANDLERS --> VALIDATION
    VALIDATION --> ROUTING
    
    ROUTING --> MEDIATOR
    MEDIATOR --> COMMANDS
    MEDIATOR --> QUERIES
    COMMANDS --> APP_LAYER
    QUERIES --> APP_LAYER
    
    APP_LAYER --> REPO
    REPO --> DB
    
    style CLAUDE fill:#e3f2fd
    style MCP_SERVER fill:#f3e5f5
    style TOOLS fill:#e8f5e8
    style MEDIATOR fill:#fff3e0
```

## 🚀 Overview

This MCP server provides AI assistants with tools to interact with the real estate property management system, including:

- **Property Management** - Create, list, and search properties
- **User Management** - Retrieve and manage user information
- **Lease Operations** - Create and manage lease agreements
- **Payment Processing** - Record and track lease payments
- **Advanced Search** - Filter properties with multiple criteria

## 🛠️ Available Tools

```mermaid
graph LR
    subgraph "Property Tools"
        T1[create_property]
        T2[list_properties_for_rent]
        T3[search_properties]
        T4[get_property_details]
    end
    
    subgraph "User Tools"
        T5[get_users]
        T6[get_user_properties]
    end
    
    subgraph "Lease Tools"
        T7[create_lease]
        T8[get_leases_by_property]
    end
    
    subgraph "Payment Tools"
        T9[make_payment]
    end
    
    subgraph "Application Layer"
        APP[REPM.Application]
    end
    
    T1 --> APP
    T2 --> APP
    T3 --> APP
    T4 --> APP
    T5 --> APP
    T6 --> APP
    T7 --> APP
    T8 --> APP
    T9 --> APP
    
    style T1 fill:#e3f2fd
    style T5 fill:#e8f5e8
    style T7 fill:#fff3e0
    style T9 fill:#fce4ec
```

### 🏠 Property Management Tools

1. **`create_property`** - Add new properties to the system
   - Input: name, description, price, beds, baths, squareFeet, address, ownerId
   - Output: Property ID and success confirmation

2. **`list_properties_for_rent`** - Browse available rental properties
   - Input: Optional filters (city, price range, bedrooms, etc.)
   - Output: List of available properties with details

3. **`search_properties`** - Advanced property search with multiple criteria
   - Input: Complex filter combinations
   - Output: Filtered property results

4. **`get_property_details`** - Get detailed information about a specific property
   - Input: Property ID
   - Output: Complete property information including owner details

### 👥 User Management Tools

5. **`get_users`** - List all users in the system
   - Input: None
   - Output: List of all users with basic information

6. **`get_user_properties`** - Get properties owned by a specific user
   - Input: User ID
   - Output: List of properties owned by the user

### 📋 Lease Management Tools

7. **`create_lease`** - Create new lease agreements
   - Input: propertyId, tenantId, startDate, endDate, monthlyRent, currency
   - Output: Lease ID and confirmation

8. **`get_leases_by_property`** - Get all leases for a specific property
   - Input: Property ID
   - Output: List of leases with details and payment history

### 💰 Payment Processing Tools

9. **`make_payment`** - Record lease payments
   - Input: leaseId, amount, currency, paymentDate, description
   - Output: Payment confirmation and receipt details

## 🔄 Request Processing Flow

```mermaid
sequenceDiagram
    participant AI as AI Assistant
    participant MCP as MCP Server
    participant Handler as Tool Handler
    participant MediatR as MediatR
    participant App as Application Layer
    participant DB as Database
    
    AI->>MCP: Tool Call Request (JSON-RPC)
    MCP->>Handler: Route to Tool Handler
    Handler->>Handler: Validate Input Parameters
    Handler->>MediatR: Send Command/Query
    MediatR->>App: Execute Business Logic
    App->>DB: Data Operation
    DB-->>App: Result
    App-->>MediatR: Response
    MediatR-->>Handler: Result
    Handler->>Handler: Format Response
    Handler-->>MCP: Tool Result
    MCP-->>AI: JSON-RPC Response
```

## 🎯 Natural Language Examples

The MCP server enables natural language interaction through AI assistants:

### Property Creation
**User**: *"Create a property in Buenos Aires with 3 bedrooms, 2 bathrooms, 1500 square feet, priced at $2800 per month"*

**AI Response**: Uses `create_property` tool with extracted parameters

### Property Search
**User**: *"Show me all properties available for rent in Los Angeles under $3000 with at least 2 bedrooms"*

**AI Response**: Uses `list_properties_for_rent` with filters

### Lease Management
**User**: *"Create a one-year lease starting January 1st for the downtown condo, tenant ID abc123, monthly rent $2500"*

**AI Response**: Uses `create_lease` tool with date calculations

## 🏗️ Architecture Components

```mermaid
graph TB
    subgraph "MCP Server Components"
        PROG[Program.cs]
        SERVER[McpServer.cs]
        MODELS[McpModels.cs]
        TOOLS[RealEstateTools.cs]
        HANDLERS[RealEstateToolHandler.cs]
    end
    
    subgraph "Configuration"
        CONFIG[appsettings.json]
        LOGGING[Logging Config]
        DB_CONFIG[Database Config]
    end
    
    subgraph "Integration"
        APP_REF[Application Reference]
        INFRA_REF[Infrastructure Reference]
        DOMAIN_REF[Domain Reference]
    end
    
    PROG --> SERVER
    PROG --> CONFIG
    SERVER --> MODELS
    SERVER --> TOOLS
    SERVER --> HANDLERS
    
    HANDLERS --> APP_REF
    APP_REF --> INFRA_REF
    INFRA_REF --> DOMAIN_REF
    
    CONFIG --> LOGGING
    CONFIG --> DB_CONFIG
    
    style PROG fill:#e3f2fd
    style SERVER fill:#f3e5f5
    style HANDLERS fill:#e8f5e8
```

### Core Components

- **Models**: JSON-RPC request/response models for MCP communication
- **Tools**: Tool definitions and parameter schemas
- **Handlers**: Business logic for processing tool requests
- **Server**: STDIO-based MCP server implementation

## 🚀 Quick Start

### Prerequisites

```mermaid
graph LR
    subgraph "Requirements"
        NET[".NET 9.0 SDK"]
        PG["PostgreSQL Database"]
        CONN["Connection String"]
        AI["AI Assistant (Claude)"]
    end
    
    NET --> BUILD[Build Project]
    PG --> DB_SETUP[Database Setup]
    CONN --> CONFIG[Configure appsettings.json]
    AI --> INTEGRATION[MCP Integration]
    
    BUILD --> RUN[Run MCP Server]
    DB_SETUP --> RUN
    CONFIG --> RUN
    INTEGRATION --> RUN
```

- .NET 9.0 SDK
- PostgreSQL database (same as main API)
- Valid connection string in `appsettings.json`
- AI assistant with MCP support (Claude recommended)

### Running the Server

```bash
# Option 1: Use the provided script
./run-mcp.sh

# Option 2: Manual steps
dotnet build
cd bin/Debug/net9.0
./REPM.MCP

# Option 3: Development mode
dotnet run
```

### VS Code Integration

The server includes VS Code configuration for easy testing and development:

1. Use **F5** to run with debugging
2. Set breakpoints in tool handlers
3. Monitor STDIO communication in the terminal

## ⚙️ Configuration

### Database Connection

Update `appsettings.json` with your PostgreSQL connection:

```json
{
  "ConnectionStrings": {
    "WebApiDatabase": "Host=localhost;Port=5432;Database=realestatetest;Username=omar;Password=rootroot"
  }
}
```

### Logging Configuration

```mermaid
graph TB
    subgraph "Logging Levels"
        NONE[None - Production]
        ERROR[Error - Debugging]
        WARN[Warning - Development]
        INFO[Information - Verbose]
    end
    
    subgraph "Output Channels"
        STDOUT[STDOUT - Reserved for MCP]
        STDERR[STDERR - Error Logging]
        FILE[File - Persistent Logs]
    end
    
    subgraph "MCP Constraints"
        CLEAN[Clean STDOUT Required]
        JSON[JSON-RPC Only]
        NO_NOISE[No Log Contamination]
    end
    
    NONE --> STDOUT
    ERROR --> STDERR
    WARN --> FILE
    INFO --> FILE
    
    STDOUT --> CLEAN
    CLEAN --> JSON
    JSON --> NO_NOISE
```

Logging is configured for minimal output suitable for MCP STDIO communication:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "None",
      "Microsoft": "None",
      "Microsoft.Hosting.Lifetime": "None"
    }
  }
}
```

## 🧪 Tool Usage Examples

### Create Property Example

```json
{
  "method": "tools/call",
  "params": {
    "name": "create_property",
    "arguments": {
      "name": "Downtown Loft",
      "description": "Modern loft in city center",
      "price": 2800,
      "beds": 2,
      "baths": 2,
      "squareFeet": 1400,
      "ownerId": "owner-uuid-here",
      "address": {
        "street": "123 Main Street",
        "city": "Buenos Aires",
        "state": "BA",
        "zipCode": "1000"
      }
    }
  }
}
```

### Search Properties Example

```json
{
  "method": "tools/call",
  "params": {
    "name": "list_properties_for_rent",
    "arguments": {
      "city": "Los Angeles",
      "minPrice": 1500,
      "maxPrice": 3000,
      "minBedrooms": 2,
      "maxBedrooms": 4
    }
  }
}
```

### Create Lease Example

```json
{
  "method": "tools/call",
  "params": {
    "name": "create_lease",
    "arguments": {
      "propertyId": "property-uuid-here",
      "tenantId": "tenant-uuid-here",
      "startDate": "2024-01-01",
      "endDate": "2024-12-31",
      "monthlyRent": 2500,
      "currency": "USD"
    }
  }
}
```

## 🔧 Development

### Project Structure

```
REPM.MCP/
├── Models/           # MCP protocol models
│   └── McpModels.cs
├── Tools/            # Tool definitions and schemas  
│   └── RealEstateTools.cs
├── Handlers/         # Business logic for tools
│   └── RealEstateToolHandler.cs
├── Server/           # MCP server implementation
│   └── McpServer.cs
├── Program.cs        # Application entry point
├── appsettings.json  # Configuration
└── run-mcp.sh       # Convenience script
```

### Adding New Tools

```mermaid
graph LR
    subgraph "Development Process"
        DEFINE[Define Tool Schema]
        IMPLEMENT[Implement Handler]
        REGISTER[Register in Server]
        TEST[Test with AI]
    end
    
    DEFINE --> IMPLEMENT
    IMPLEMENT --> REGISTER
    REGISTER --> TEST
    
    subgraph "Code Locations"
        TOOLS_FILE[RealEstateTools.cs]
        HANDLER_FILE[RealEstateToolHandler.cs]
        SERVER_FILE[McpServer.cs]
    end
    
    DEFINE --> TOOLS_FILE
    IMPLEMENT --> HANDLER_FILE
    REGISTER --> SERVER_FILE
```

1. Define tool schema in `Tools/RealEstateTools.cs`
2. Implement handler logic in `Handlers/RealEstateToolHandler.cs` 
3. Register tool in server initialization
4. Test with AI assistant integration

### Testing Approach

```mermaid
graph TB
    subgraph "Testing Methods"
        UNIT[Unit Tests]
        INTEGRATION[Integration Tests]
        AI_TEST[AI Assistant Tests]
        MANUAL[Manual JSON-RPC]
    end
    
    subgraph "Test Scenarios"
        HAPPY[Happy Path]
        ERROR[Error Handling]
        VALIDATION[Input Validation]
        EDGE[Edge Cases]
    end
    
    UNIT --> HAPPY
    UNIT --> VALIDATION
    INTEGRATION --> ERROR
    INTEGRATION --> EDGE
    AI_TEST --> HAPPY
    AI_TEST --> ERROR
    MANUAL --> VALIDATION
    MANUAL --> EDGE
```

Use VS Code debugging or connect with an MCP-compatible AI assistant:

1. Start the server: `./run-mcp.sh`
2. Server communicates via STDIO using JSON-RPC
3. Send tool requests and receive responses
4. Monitor logs for debugging information

## 🔍 Troubleshooting

### Common Issues

```mermaid
graph TB
    subgraph "Connection Issues"
        C1[PostgreSQL Not Running]
        C2[Invalid Connection String]
        C3[Database Permissions]
        C4[Network Connectivity]
    end
    
    subgraph "Configuration Issues"
        CF1[Missing appsettings.json]
        CF2[Wrong Working Directory]
        CF3[File Permissions]
        CF4[Invalid JSON Format]
    end
    
    subgraph "Protocol Issues"
        P1[STDIO Communication Error]
        P2[JSON-RPC Format Error]
        P3[Message Parsing Error]
        P4[Response Timeout]
    end
    
    subgraph "Solutions"
        S1[Check Database Status]
        S2[Verify Configuration]
        S3[Validate Message Format]
        S4[Review Logs]
    end
    
    C1 --> S1
    C2 --> S1
    CF1 --> S2
    CF2 --> S2
    P1 --> S3
    P2 --> S3
    P3 --> S4
    P4 --> S4
```

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

## 🔗 Integration

This MCP server integrates seamlessly with the main REPM system:

```mermaid
graph LR
    subgraph "Shared Components"
        DOMAIN[Shared Domain]
        APP[Shared Application]
        INFRA[Shared Infrastructure]
    end
    
    subgraph "Benefits"
        CONSISTENCY[Data Consistency]
        REUSE[Code Reuse]
        MAINTENANCE[Easy Maintenance]
        EVOLUTION[System Evolution]
    end
    
    DOMAIN --> CONSISTENCY
    APP --> REUSE
    INFRA --> MAINTENANCE
    DOMAIN --> EVOLUTION
    APP --> EVOLUTION
    INFRA --> EVOLUTION
```

- **Shared Domain**: Uses same entities and value objects
- **Shared Application**: Leverages CQRS commands and queries  
- **Shared Infrastructure**: Uses same database and repositories
- **Consistent Behavior**: Same business rules across all interfaces

## 🎯 Benefits

- **Natural Language Interface**: Intuitive property management through AI
- **Consistent Data Model**: Same domain logic as GraphQL API
- **Scalable Architecture**: Built on proven CQRS patterns
- **AI-Friendly**: Optimized for AI assistant integration
- **Developer Experience**: Easy to extend and maintain

---

The REPM MCP Server bridges the gap between human language and technical real estate management operations, enabling powerful AI-assisted property management workflows.
