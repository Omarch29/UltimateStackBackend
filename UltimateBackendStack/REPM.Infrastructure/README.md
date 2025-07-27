# 🏗️ REPM.Infrastructure

The **REPM.Infrastructure** project is responsible for implementing data access, persistence, and external integrations for the Real Estate Property Manager (REPM) system. It leverages **Entity Framework Core** and follows architectural patterns like the **Repository Pattern** and the **Unit of Work Pattern** to enforce clean separation of concerns and ensure the domain layer remains persistence-agnostic.

## 🏛️ Infrastructure Architecture

```mermaid
graph TB
    subgraph "Application Layer"
        APP[REPM.Application]
        COMMANDS[Commands]
        QUERIES[Queries]
    end
    
    subgraph "Infrastructure Layer"
        REPO[Repository Layer]
        CONTEXT[REPMContext]
        UOW[Unit of Work]
        SEED[Database Seeder]
    end
    
    subgraph "Entity Framework Core"
        EF[EF Core]
        MIGRATIONS[Migrations]
        CONFIG[Entity Configurations]
    end
    
    subgraph "Database Layer"
        DB[(PostgreSQL)]
        TABLES[Tables]
        INDEXES[Indexes]
        CONSTRAINTS[Constraints]
    end
    
    APP --> REPO
    COMMANDS --> UOW
    QUERIES --> REPO
    
    REPO --> CONTEXT
    UOW --> CONTEXT
    CONTEXT --> EF
    
    EF --> MIGRATIONS
    EF --> CONFIG
    EF --> DB
    
    DB --> TABLES
    DB --> INDEXES
    DB --> CONSTRAINTS
    
    SEED --> CONTEXT
    
    style REPO fill:#e3f2fd
    style CONTEXT fill:#e8f5e8
    style EF fill:#fff3e0
    style DB fill:#fce4ec
```

---

## 📦 What This Project Contains

- `REPMContext` (EF Core `DbContext`)
- Generic `Repository<TEntity>` implementation
- Interfaces for `IRepository<TEntity>` and `IUnitOfWork`
- Entity configurations and value object ownership setup
- Database seeding and migrations
- Security and audit tracking

## 🔄 Repository Pattern Flow

```mermaid
sequenceDiagram
    participant App as Application Layer
    participant Repo as Repository
    participant Context as REPMContext
    participant DB as Database
    
    App->>Repo: GetByIdAsync(id)
    Repo->>Context: Find entity by ID
    Context->>DB: SELECT query
    DB-->>Context: Entity data
    Context-->>Repo: Domain entity
    Repo-->>App: Domain entity
    
    App->>Repo: Insert(entity)
    Repo->>Context: Add to DbSet
    App->>Repo: SaveChangesAsync()
    Repo->>Context: SaveChangesAsync()
    Context->>DB: INSERT/UPDATE queries
    DB-->>Context: Success
    Context-->>Repo: Changes saved
    Repo-->>App: Success
```

---

## 🧩 Repository Pattern

The **Repository Pattern** provides a consistent abstraction over data access. It encapsulates querying and persistence logic, allowing the rest of the application to work with a simple interface for interacting with data.

### ✅ `Repository<TEntity>` Highlights:

```mermaid
classDiagram
    class IRepository~T~ {
        <<interface>>
        +IQueryable~T~ Query
        +IQueryable~T~ QueryToRead
        +IQueryable~T~ QueryDeleted
        +GetByIdAsync(id) Task~T~
        +Insert(entity) void
        +Delete(entity) void
        +SaveChangesAsync() Task
    }
    
    class Repository~T~ {
        -DbContext _context
        -DbSet~T~ _dbSet
        +Query IQueryable~T~
        +QueryToRead IQueryable~T~
        +QueryDeleted IQueryable~T~
        +GetByIdAsync(id) Task~T~
        +Insert(entity) void
        +Delete(entity) void
        +SaveChangesAsync() Task
    }
    
    IRepository~T~ <|-- Repository~T~
```

- Works with any entity that inherits from `BaseEntity`
- Exposes three query options:
  - `Query` — entities that are not deleted
  - `QueryToRead` — same as above, but without change tracking
  - `QueryDeleted` — soft-deleted entities
- Implements common CRUD operations:
  - `GetByIdAsync`, `Insert`, `Delete`, `AddRange`, `DeleteRange`
- Tracks soft deletes via the `IsDeleted` flag
- Automatically wires up `DbContext` and `DbSet<TEntity>`

Example usage in application layer:
```csharp
var user = await _userRepository.GetByIdAsync(userId);
_repository.Delete(user);
await _repository.SaveChangesAsync();
```

---

## 🧾 Unit of Work Pattern

The **Unit of Work Pattern** ensures that a series of operations across multiple repositories can be committed as a single transaction.

```mermaid
graph LR
    subgraph "Unit of Work Scope"
        R1[User Repository]
        R2[Property Repository]
        R3[Lease Repository]
    end
    
    R1 --> UOW[Unit of Work]
    R2 --> UOW
    R3 --> UOW
    
    UOW --> COMMIT[Single Transaction Commit]
    
    subgraph "Audit Trail"
        CREATED[CreatedAt]
        UPDATED[UpdatedAt]
        CREATEDBY[CreatedBy]
        UPDATEDBY[UpdatedBy]
    end
    
    COMMIT --> CREATED
    COMMIT --> UPDATED
    COMMIT --> CREATEDBY
    COMMIT --> UPDATEDBY
```

### ✅ `REPMContext` Implements `IUnitOfWork`

The `SaveChangesAsync()` method in `REPMContext`:

- Automatically fills in `CreatedAt`, `UpdatedAt`, `CreatedBy`, and `UpdatedBy` fields
- Accesses the current user from `IHttpContextAccessor` for audit tracking
- Applies entity state logic before committing

This allows any repository to access `UnitOfWork.SaveChangesAsync()` and commit changes safely and consistently.

---

## 🧠 REPMContext Overview

```mermaid
erDiagram
    REPMContext {
        DbSet_User_ Users
        DbSet_Property_ Properties
        DbSet_Lease_ Leases
        DbSet_Payment_ Payments
    }
    
    User ||--o{ Property : owns
    Property ||--o{ Lease : "has leases"
    User ||--o{ Lease : rents
    Lease ||--o{ Payment : "receives payments"
    
    Property {
        Guid Id PK
        string Name
        Address Address "Owned Entity"
        bool IsDeleted "Soft Delete"
        DateTime CreatedAt "Audit"
        DateTime UpdatedAt "Audit"
    }
    
    Address {
        string Street
        string City
        string State
        string ZipCode
        string Country
    }
```

`REPMContext` is the Entity Framework `DbContext` and includes:

- Entity sets: `Users`, `Properties`, `Leases`, `Payments`
- Configuration of **owned types** like:
  - `Address` inside `Property`
  - `LeasePeriod` and `RentAmount` inside `Lease`
  - `Amount` inside `Payment`
- Relationships between entities with proper `OnDelete(DeleteBehavior.Cascade)`
- Soft-delete handling via `BaseEntity.IsDeleted`

The context serves as the central point of coordination between domain models and the database.

## 🗄️ Database Schema

```mermaid
graph TB
    subgraph "Database Tables"
        USERS[Users Table]
        PROPS[Properties Table]
        LEASES[Leases Table]
        PAYMENTS[Payments Table]
    end
    
    subgraph "Owned Entities"
        ADDR[Address Fields in Properties]
        PERIOD[LeasePeriod Fields in Leases]
        MONEY[Money Fields in Payments]
    end
    
    subgraph "Indexes & Constraints"
        PK[Primary Keys]
        FK[Foreign Keys]
        IDX[Performance Indexes]
        CHK[Check Constraints]
    end
    
    USERS --> PROPS
    PROPS --> LEASES
    LEASES --> PAYMENTS
    
    PROPS --> ADDR
    LEASES --> PERIOD
    PAYMENTS --> MONEY
    
    USERS --> PK
    PROPS --> PK
    LEASES --> PK
    PAYMENTS --> PK
    
    PROPS --> FK
    LEASES --> FK
    PAYMENTS --> FK
    
    USERS --> IDX
    PROPS --> IDX
    LEASES --> IDX
    
    LEASES --> CHK
    PAYMENTS --> CHK
```

---

## 🗂️ Folder Structure

```
REPM.Infrastructure/
├── Persistence/
│   ├── REPMContext.cs
│   └── Seeding/
│       └── DatabaseSeeder.cs
├── Repositories/
│   └── Repository.cs
├── Interfaces/
│   ├── IRepository.cs
│   └── IUnitOfWork.cs
├── Security/
│   └── AuditableEntityConfiguration.cs
└── Migrations/
    └── [EF Core Migrations]
```

## 🔄 Data Flow Lifecycle

```mermaid
graph LR
    subgraph "Request Processing"
        REQ[Client Request]
        APP[Application Layer]
        REPO[Repository]
        CONTEXT[REPMContext]
        DB[Database]
    end
    
    REQ --> APP
    APP --> REPO
    REPO --> CONTEXT
    CONTEXT --> DB
    
    subgraph "Audit & Tracking"
        AUDIT[Audit Fields]
        SOFT[Soft Delete]
        TRACK[Change Tracking]
    end
    
    CONTEXT --> AUDIT
    CONTEXT --> SOFT
    CONTEXT --> TRACK
    
    subgraph "Response"
        ENTITY[Domain Entity]
        DTO[DTO Projection]
        JSON[JSON Response]
    end
    
    DB --> ENTITY
    ENTITY --> DTO
    DTO --> JSON
```

1. Application calls a repository method (e.g., `Insert`, `Delete`, `GetByIdAsync`)
2. The repository interacts with `REPMContext`
3. `REPMContext.SaveChangesAsync()` processes audit info and commits
4. Domain layer remains decoupled from persistence logic

---

This infrastructure layer ensures that the rest of the application remains **clean, focused, and persistence-agnostic**, staying true to the principles of **Clean Architecture** and **DDD**.
