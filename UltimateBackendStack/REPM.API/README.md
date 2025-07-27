# 🌐 REPM.API - GraphQL Gateway

The **REPM.API** project serves as the GraphQL gateway for the Real Estate Property Manager (REPM) system. Built with **HotChocolate**, it provides a modern, type-safe, and efficient API for managing real estate properties, leases, users, and payments.

## 🏗️ GraphQL Architecture

```mermaid
graph TB
    subgraph "Client Layer"
        WEB[Web Applications]
        MOBILE[Mobile Apps]
        TOOLS[API Tools]
    end
    
    subgraph "GraphQL Layer"
        GQL[GraphQL Endpoint]
        SCHEMA[Schema Definition]
        RESOLVERS[Resolvers]
    end
    
    subgraph "Query & Mutation Types"
        QUERIES[Queries]
        MUTATIONS[Mutations]
        SUBS[Subscriptions]
    end
    
    subgraph "Feature Areas"
        PROP[Property Operations]
        USER[User Management]
        LEASE[Lease Operations]
        PAY[Payment Processing]
    end
    
    subgraph "Application Layer"
        APP[REPM.Application]
        MEDIATOR[MediatR]
        HANDLERS[Command/Query Handlers]
    end
    
    WEB --> GQL
    MOBILE --> GQL
    TOOLS --> GQL
    
    GQL --> SCHEMA
    SCHEMA --> RESOLVERS
    RESOLVERS --> QUERIES
    RESOLVERS --> MUTATIONS
    RESOLVERS --> SUBS
    
    QUERIES --> PROP
    QUERIES --> USER
    QUERIES --> LEASE
    QUERIES --> PAY
    
    MUTATIONS --> PROP
    MUTATIONS --> USER
    MUTATIONS --> LEASE
    MUTATIONS --> PAY
    
    PROP --> APP
    USER --> APP
    LEASE --> APP
    PAY --> APP
    
    APP --> MEDIATOR
    MEDIATOR --> HANDLERS
    
    style GQL fill:#e3f2fd
    style APP fill:#e8f5e8
    style MEDIATOR fill:#fff3e0
```

## 🔧 Features

### GraphQL Capabilities
- **Type-safe queries** with strong schema validation
- **Real-time subscriptions** for live updates
- **Advanced filtering** with HotChocolate filtering
- **Automatic pagination** support
- **Field selection** for optimized data fetching
- **Error handling** with detailed error responses

### Available Operations

```mermaid
graph LR
    subgraph "Queries"
        Q1[propertiesForRent]
        Q2[users]
        Q3[leasesByProperty]
        Q4[propertyDetails]
    end
    
    subgraph "Mutations"
        M1[createProperty]
        M2[createLease]
        M3[makePayment]
        M4[updateProperty]
    end
    
    subgraph "Filters"
        F1[City Filter]
        F2[Price Range]
        F3[Bedroom Count]
        F4[Property Type]
    end
    
    Q1 --> F1
    Q1 --> F2
    Q1 --> F3
    Q1 --> F4
    
    style Q1 fill:#e8f5e8
    style M1 fill:#ffebee
```

## 🚀 Request Flow

```mermaid
sequenceDiagram
    participant Client
    participant GraphQL
    participant Resolver
    participant MediatR
    participant Handler
    participant Repository
    participant Database
    
    Client->>GraphQL: Send Query/Mutation
    GraphQL->>Resolver: Route to Resolver
    Resolver->>MediatR: Send Command/Query
    MediatR->>Handler: Execute Handler
    Handler->>Repository: Data Operation
    Repository->>Database: SQL Query
    Database-->>Repository: Result Set
    Repository-->>Handler: Domain Objects
    Handler-->>MediatR: Response
    MediatR-->>Resolver: Result
    Resolver-->>GraphQL: Formatted Response
    GraphQL-->>Client: JSON Response
```

## 📊 Schema Structure

```mermaid
erDiagram
    Query {
        propertiesForRent PropertyConnection
        users UserConnection
        propertyDetails Property
        leasesByProperty LeaseConnection
    }
    
    Mutation {
        createProperty CreatePropertyPayload
        createLease CreateLeasePayload
        makePayment MakePaymentPayload
        updateProperty UpdatePropertyPayload
    }
    
    Property {
        ID id
        String name
        Address address
        String description
        Decimal price
        Int beds
        Int baths
        Int squareFeet
        Boolean isActive
        User owner
    }
    
    Address {
        String street
        String city
        String state
        String zipCode
        String country
    }
    
    User {
        ID id
        String name
        String email
        PropertyConnection properties
        LeaseConnection leases
    }
    
    Lease {
        ID id
        Property property
        User renter
        DateRange leasePeriod
        Money rentAmount
        LeaseStatus status
        PaymentConnection payments
    }
```

## 🔍 Example Queries

### Get Properties with Filtering
```graphql
query GetPropertiesInCity {
  propertiesForRent(
    filters: { 
      city: "Buenos Aires"
      minPrice: 1000
      maxPrice: 3000
      minBedrooms: 2
    }
    first: 10
  ) {
    edges {
      node {
        id
        name
        address {
          street
          city
          state
        }
        price
        beds
        baths
        owner {
          name
          email
        }
      }
    }
    pageInfo {
      hasNextPage
      endCursor
    }
  }
}
```

### Create New Property
```graphql
mutation CreateProperty {
  createProperty(input: {
    name: "Downtown Loft"
    description: "Modern loft in city center"
    price: 2500
    beds: 2
    baths: 2
    squareFeet: 1200
    ownerId: "user-id-here"
    address: {
      street: "123 Main St"
      city: "Buenos Aires"
      state: "BA"
      zipCode: "1000"
    }
  }) {
    property {
      id
      name
      address {
        city
      }
    }
    errors {
      message
      code
    }
  }
}
```

## 🛠️ Development Setup

### Prerequisites
- .NET 9 SDK
- PostgreSQL database
- REPM.Application and REPM.Infrastructure projects

### Running the API
```bash
cd REPM.API
dotnet run
```

### GraphQL Playground
Access the GraphQL Playground at: `http://localhost:5000/graphql`

## 🧪 Testing

### Unit Testing
```bash
dotnet test
```

### Integration Testing
The API includes integration tests for:
- GraphQL schema validation
- Query execution
- Mutation operations
- Error handling

## 📈 Performance Features

- **DataLoader** pattern for efficient data fetching
- **Projection** to select only requested fields
- **Pagination** to handle large result sets
- **Caching** for frequently accessed data
- **Connection pooling** for database efficiency

## 🔐 Security

- **Input validation** on all mutations
- **Authorization** filters for protected operations
- **Rate limiting** to prevent abuse
- **CORS** configuration for cross-origin requests

---

The REPM.API serves as a powerful, flexible gateway that leverages GraphQL's strengths while maintaining clean architecture principles and optimal performance.