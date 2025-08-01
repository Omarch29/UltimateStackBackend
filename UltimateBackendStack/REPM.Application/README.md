# ⚙️ REPM.Application

The **REPM.Application** layer is the core of the business orchestration logic in the Real Estate Property Manager (REPM) system. It serves as the bridge between the API layer (GraphQL) and the domain layer, handling commands, queries, filtering, and validations in a decoupled and scalable way.

## 🏗️ Application Architecture

```mermaid
graph TB
    subgraph "API Layer"
        GQL[GraphQL API]
        MCP[MCP Server]
    end
    
    subgraph "Application Layer"
        MED[MediatR Pipeline]
        CMD[Commands]
        QRY[Queries]
        HAND[Handlers]
        VAL[Validators]
        MAP[AutoMapper]
        FILTER[Query Filters]
    end
    
    subgraph "Cross-Cutting Concerns"
        LOG[Logging]
        CACHE[Caching]
        AUDIT[Audit Trail]
        ERR[Error Handling]
    end
    
    subgraph "Domain & Infrastructure"
        DOM[Domain Layer]
        REPO[Repositories]
        DB[(Database)]
    end
    
    GQL --> MED
    MCP --> MED
    
    MED --> CMD
    MED --> QRY
    MED --> VAL
    
    CMD --> HAND
    QRY --> HAND
    VAL --> HAND
    
    HAND --> MAP
    HAND --> FILTER
    HAND --> DOM
    HAND --> REPO
    
    MED --> LOG
    MED --> CACHE
    MED --> AUDIT
    MED --> ERR
    
    REPO --> DB
    
    style MED fill:#e3f2fd
    style HAND fill:#e8f5e8
    style DOM fill:#fff3e0
```

---

## 🧭 Purpose

This layer is responsible for:

- Defining **CQRS** operations (`Commands` and `Queries`)
- Coordinating request handling through the **Mediator Pattern**
- Exposing **DTOs** to communicate with external layers
- Applying business logic orchestrations without polluting the domain
- Automating filtering, pagination, and transformation of data

This keeps the domain layer clean and focused, and provides flexibility in how requests are handled.

## 🔄 CQRS Flow Architecture

```mermaid
graph LR
    subgraph "Command Side (Write Operations)"
        WCLIENT[Client Request]
        WCMD[Command]
        WVAL[Validator]
        WHAND[Command Handler]
        WDOM[Domain Logic]
        WREPO[Repository]
    end
    
    subgraph "Query Side (Read Operations)"
        RCLIENT[Client Request]
        RQRY[Query]
        RFILTER[Query Filter]
        RHAND[Query Handler]
        RPROJ[Projection]
        RREPO[Repository]
    end
    
    subgraph "Shared Infrastructure"
        MED[MediatR]
        DB[(Database)]
    end
    
    WCLIENT --> WCMD
    RCLIENT --> RQRY
    
    WCMD --> MED
    RQRY --> MED
    
    MED --> WVAL
    MED --> RFILTER
    
    WVAL --> WHAND
    RFILTER --> RHAND
    
    WHAND --> WDOM
    WHAND --> WREPO
    
    RHAND --> RPROJ
    RHAND --> RREPO
    
    WREPO --> DB
    RREPO --> DB
    
    style WCMD fill:#ffebee
    style RQRY fill:#e8f5e8
    style MED fill:#e3f2fd
```

---

## 🧱 Patterns in Use

### 📌 Mediator Pattern (via MediatR)

```mermaid
sequenceDiagram
    participant Client
    participant Controller
    participant MediatR
    participant Handler
    participant Domain
    participant Repository
    
    Client->>Controller: HTTP Request
    Controller->>MediatR: Send(command/query)
    MediatR->>Handler: Route to Handler
    Handler->>Domain: Business Logic
    Handler->>Repository: Data Access
    Repository-->>Handler: Result
    Handler-->>MediatR: Response
    MediatR-->>Controller: Result
    Controller-->>Client: HTTP Response
```

The **Mediator Pattern** decouples the sender of a request from its handler. In REPM:

- Every `Command` or `Query` is sent through `IMediator`
- `Handlers` process the request and return results
- No direct calls between layers — only MediatR coordinates them

Example:

```csharp
await _mediator.Send(new CreateLeaseCommand(...));
```

This promotes **loose coupling**, **single responsibility**, and **testability**.

---

### 📌 CQRS (Command Query Responsibility Segregation)

```mermaid
graph TB
    subgraph "Commands (Write)"
        C1[CreatePropertyCommand]
        C2[CreateLeaseCommand]
        C3[MakePaymentCommand]
        C4[UpdatePropertyCommand]
    end
    
    subgraph "Queries (Read)"
        Q1[GetPropertiesForRentQuery]
        Q2[GetUserByIdQuery]
        Q3[GetLeasesByPropertyQuery]
        Q4[GetPropertyDetailsQuery]
    end
    
    subgraph "Handlers"
        CH[Command Handlers]
        QH[Query Handlers]
    end
    
    C1 --> CH
    C2 --> CH
    C3 --> CH
    C4 --> CH
    
    Q1 --> QH
    Q2 --> QH
    Q3 --> QH
    Q4 --> QH
    
    CH --> WRITE[Write Database]
    QH --> READ[Read Database]
    
    style C1 fill:#ffebee
    style Q1 fill:#e8f5e8
```

This pattern separates **write operations** (`Commands`) from **read operations** (`Queries`). Each use case is modeled independently for clarity and performance:

- `Commands` modify state and return nothing or a result ID
- `Queries` return data and never change state

This results in:

- Cleaner separation of concerns
- Easier scalability
- Better encapsulation of behavior

---

## 🔍 Filter Automation with `QueryFilterHelper`

The `QueryFilterHelper` class enables automatic application of filtering logic based on dynamic user-provided filters.

### ✅ Features:

- Accepts any class implementing `IFilter`
- Dynamically reads properties from the filter object
- Matches those properties against the entity's properties (even nested ones like `Address.City`)
- Builds an expression tree on the fly using `System.Linq.Expressions`
- Applies the composed predicate to the `IQueryable<T>`

### 🔤 Filter Property Naming Convention

In order for `QueryFilterHelper` to dynamically match and apply filters correctly, the property names in your filter class must:

- Exactly match the name of the property in the target entity (e.g., `City`, `Price`, `Beds`)
- Or follow a prefix pattern such as `Min` or `Max` to apply range-based filters

Examples:
- `City` matches `Property.Address.City`
- `MinRent` or `MaxRent` match `Property.Price` with greater than/less than filters
- `ZipCode` matches `Property.Address.ZipCode`

This naming convention allows the filter engine to generate the appropriate expression without writing any manual logic.

### 🧠 Why it's powerful:

- Reduces boilerplate `if` statements for filtering
- Supports future filters by simply adding new properties to the filter class
- Allows nested matching for navigation properties
- Keeps query handlers clean and reusable

### 📦 Example Usage:

```csharp
query = QueryFilterHelper.ApplyFilters(query, filters);
```

This helps keep GraphQL queries and query handlers lean, dynamic, and DRY.

---

## 📊 Data Projection with AutoMapper `ProjectTo`

To convert domain or EF Core entities into DTOs efficiently, the REPM application uses `AutoMapper`’s `ProjectTo` method in query handlers:

```csharp
var result = query.ProjectTo<PropertyDto>(_mapper.ConfigurationProvider);
```

### ✅ Why use `ProjectTo`?

- **Efficient**: Translates DTO projections into SQL directly (only needed fields are selected)
- **Composable**: Keeps the query in `IQueryable` form for paging, sorting, and further filtering
- **Safe**: Avoids over-fetching data that isn't requested
- **Clean**: Removes the need for manual mapping code inside query handlers

---

### 🧪 Returning `IQueryable` to the API Layer

When a query handler returns `IQueryable<T>`, it allows the GraphQL API (HotChocolate) to:

- Apply additional filters using `[UseFiltering]`
- Apply automatic pagination using `[UsePaging]`
- Enable sorting via `[UseSorting]`

This approach provides maximum flexibility, offloading query shaping responsibilities to the GraphQL layer while still preserving optimized database access through EF Core.

> ⚠️ Be careful not to materialize the query (`ToListAsync`) too early unless necessary.

---

## 🗂️ Folder Structure

```
REPM.Application/
├── CQRS/
│   ├── Commands/
│   └── Queries/
├── DTOs/
├── Filters/
├── Interfaces/
├── Helpers/
├── Validators/
```

---

## 🚀 What This Enables for the Future

- Plug-and-play query filters with no added boilerplate
- Easily swappable handler logic
- Scalable architecture that adapts to more complex workflows (e.g., notifications, background tasks)
- Testable command and query handlers for business validation

---

By separating the application logic in this layer, REPM achieves a **modular**, **maintainable**, and **test-friendly** backend system — ready to evolve with future feature demands.
