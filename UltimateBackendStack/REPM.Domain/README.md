# 🧠 REPM.Domain

The **REPM.Domain** project is the core of the Real Estate Property Manager system and follows the principles of **Domain-Driven Design (DDD)** and **Clean Architecture**. This layer encapsulates all the **business rules**, **domain models**, and **invariants**, ensuring that the application remains flexible, testable, and maintainable.

## 🏗️ Domain Architecture

```mermaid
graph TB
    subgraph "Domain Layer (Core)"
        ENT[Entities]
        VO[Value Objects]
        AGG[Aggregates]
        DOM_SERV[Domain Services]
        DOM_EVENTS[Domain Events]
        DOM_EXC[Domain Exceptions]
        ENUMS[Enumerations]
    end
    
    subgraph "External Dependencies"
        APP[Application Layer]
        INFRA[Infrastructure Layer]
        API[API Layer]
    end
    
    subgraph "Domain Concepts"
        PROPERTY[Property Management]
        LEASE[Lease Lifecycle]
        PAYMENT[Payment Processing]
        USER[User Management]
    end
    
    ENT --> PROPERTY
    ENT --> LEASE
    ENT --> PAYMENT
    ENT --> USER
    
    VO --> ENT
    AGG --> ENT
    DOM_SERV --> ENT
    DOM_EVENTS --> ENT
    DOM_EXC --> ENT
    ENUMS --> ENT
    
    APP -.-> ENT
    INFRA -.-> ENT
    API -.-> APP
    
    style ENT fill:#fff3e0
    style VO fill:#e8f5e8
    style DOM_SERV fill:#e3f2fd
    style DOM_EVENTS fill:#fce4ec
```

---

## 🏗️ Clean Architecture & DDD Principles

- **Independence**: This layer has no dependencies on external technologies, frameworks, or infrastructure.
- **Rich Domain Model**: Business logic lives inside the domain models (Entities, Aggregates, and Value Objects).
- **Separation of Concerns**: Use Cases and Infrastructure are not part of this layer.
- **Ubiquitous Language**: The domain code reflects the language of real estate professionals (e.g., Lease, Property, Payment).

## 🏢 Domain Model Structure

```mermaid
erDiagram
    Property ||--o{ Lease : "has leases"
    User ||--o{ Property : owns
    User ||--o{ Lease : rents
    Lease ||--o{ Payment : "receives payments"
    Property ||--|| Address : "has address"
    Lease ||--|| DateRange : "has period"
    Lease ||--|| Money : "has rent amount"
    Payment ||--|| Money : "has amount"
    
    Property {
        Guid Id PK
        string Name
        Address Address
        string Description
        decimal Price
        int Beds
        int Baths
        int SquareFeet
        bool IsListedForRent
        Guid OwnerId FK
        DateTime CreatedAt
        DateTime UpdatedAt
        bool IsDeleted
    }
    
    User {
        Guid Id PK
        string Name
        string Email
        DateTime CreatedAt
        DateTime UpdatedAt
        bool IsDeleted
    }
    
    Lease {
        Guid Id PK
        Guid PropertyId FK
        Guid RenterId FK
        DateRange LeasePeriod
        Money RentAmount
        LeaseStatus Status
        DateTime CreatedAt
        DateTime UpdatedAt
        bool IsDeleted
    }
    
    Payment {
        Guid Id PK
        Guid LeaseId FK
        Money Amount
        DateTime PaymentDate
        string Description
        PaymentStatus Status
        DateTime CreatedAt
        DateTime UpdatedAt
        bool IsDeleted
    }
    
    Address {
        string Street
        string City
        string State
        string ZipCode
        string Country
    }
    
    DateRange {
        DateTime StartDate
        DateTime EndDate
    }
    
    Money {
        decimal Amount
        string Currency
    }
```

---

## 🧱 Entities & Aggregates

### 🏠 `Property`

```mermaid
classDiagram
    class Property {
        <<Aggregate Root>>
        +Guid Id
        +string Name
        +Address Address
        +Guid OwnerId
        +bool IsListedForRent
        +string Description
        +decimal Price
        +int Beds
        +int Baths
        +int SquareFeet
        +List~Lease~ Leases
        +bool IsAvailable
        +ListForRent() void
        +UnlistFromRent() void
        +CheckAvailability(DateRange) bool
        +CreateLease(User, DateRange, Money) Lease
    }
    
    class Address {
        <<Value Object>>
        +string Street
        +string City
        +string State
        +string ZipCode
        +string Country
        +Validate() void
    }
    
    Property *-- Address : contains
```

- Aggregate Root
- Holds property data: name, address, owner ID
- Can be listed/unlisted for rent
- Can check availability for a date range

### 📜 `Lease`

```mermaid
classDiagram
    class Lease {
        <<Aggregate Root>>
        +Guid Id
        +Guid PropertyId
        +Guid RenterId
        +DateRange LeasePeriod
        +Money RentAmount
        +LeaseStatus Status
        +List~Payment~ Payments
        +bool IsActive
        +bool IsExpired
        +Activate() void
        +Expire() void
        +Cancel() void
        +AddPayment(Money, DateTime) Payment
        +GetOutstandingAmount() Money
    }
    
    class DateRange {
        <<Value Object>>
        +DateTime StartDate
        +DateTime EndDate
        +bool IsValid
        +bool Contains(DateTime) bool
        +bool Overlaps(DateRange) bool
        +TimeSpan Duration
    }
    
    class Money {
        <<Value Object>>
        +decimal Amount
        +string Currency
        +Add(Money) Money
        +Subtract(Money) Money
        +IsPositive bool
        +IsZero bool
    }
    
    Lease *-- DateRange : contains
    Lease *-- Money : contains
```

- Aggregate Root
- Represents rental agreements
- Ensures valid start/end dates
- Maintains `LeaseStatus` (Pending, Active, Expired, Canceled)

### 💳 `Payment`

```mermaid
stateDiagram-v2
    [*] --> Pending
    Pending --> Completed : Process Payment
    Pending --> Failed : Payment Error
    Pending --> Overdue : Past Due Date
    Failed --> Pending : Retry Payment
    Overdue --> Completed : Late Payment
    Overdue --> Canceled : Cancel Payment
    Completed --> [*]
    Canceled --> [*]
```

- Linked to a `Lease`
- Validates payment timing and amount
- Tracks `PaymentStatus` (Pending, Completed, Failed, Overdue, Canceled)

### 👤 `User`
- Represents both renters and property owners
- Holds basic identity info

---

## 🧱 Value Objects

```mermaid
graph TB
    subgraph "Value Objects"
        ADDR[Address]
        MONEY[Money]
        DATE[DateRange]
    end
    
    subgraph "Characteristics"
        IMMUT[Immutable]
        EQUAL[Equality by Value]
        VALID[Self-Validating]
        ATOMIC[Atomic Updates]
    end
    
    ADDR --> IMMUT
    ADDR --> EQUAL
    ADDR --> VALID
    
    MONEY --> IMMUT
    MONEY --> EQUAL
    MONEY --> VALID
    MONEY --> ATOMIC
    
    DATE --> IMMUT
    DATE --> EQUAL
    DATE --> VALID
    
    subgraph "Usage Examples"
        EX1[Property has Address]
        EX2[Lease has Money rent]
        EX3[Lease has DateRange period]
    end
    
    ADDR --> EX1
    MONEY --> EX2
    DATE --> EX3
```

### 🏠 `Address`
- Composed of street, city, state, and zip code
- Immutable and used inside `Property`

### 💵 `Money`
- Represents a currency + value pair
- Prevents invalid money states

### 📆 `DateRange`
- Enforces a valid time period
- Used in `Lease` to represent rental duration

---

## 🧩 Enums

```mermaid
graph LR
    subgraph "LeaseStatus"
        LS1[Pending]
        LS2[Active]
        LS3[Expired]
        LS4[Canceled]
    end
    
    subgraph "PaymentStatus"
        PS1[Pending]
        PS2[Completed]
        PS3[Failed]
        PS4[Overdue]
        PS5[Canceled]
    end
    
    subgraph "State Transitions"
        LS1 --> LS2
        LS2 --> LS3
        LS1 --> LS4
        LS2 --> LS4
        
        PS1 --> PS2
        PS1 --> PS3
        PS1 --> PS4
        PS3 --> PS1
        PS4 --> PS2
        PS4 --> PS5
    end
```

### `LeaseStatus`
- `Pending`, `Active`, `Expired`, `Canceled`

### `PaymentStatus`
- `Pending`, `Completed`, `Failed`, `Overdue`, `Canceled`

---

## 💥 Domain Events

```mermaid
sequenceDiagram
    participant Entity
    participant DomainEvent
    participant EventHandler
    participant ExternalSystem
    
    Entity->>DomainEvent: Raise Event
    Note over Entity: LeaseCreated Event
    DomainEvent->>EventHandler: Publish via MediatR
    EventHandler->>ExternalSystem: Send Notification
    EventHandler->>ExternalSystem: Update Analytics
    EventHandler->>ExternalSystem: Trigger Workflow
```

### `LeaseCreated`
- Raised when a new lease is established

### `PaymentReceived`
- Triggered when a payment is recorded

### `PropertyListedForRent`
- Raised when a property is marked available for rent

> Events are implemented using `IDomainEvent` and published via `IMediator`.

---

## 🧰 Domain Exceptions

```mermaid
classDiagram
    class DomainException {
        <<abstract>>
        +string Message
        +string ErrorCode
    }
    
    class InvalidLeasePeriodException {
        +InvalidLeasePeriodException(DateRange)
    }
    
    class OverduePaymentException {
        +OverduePaymentException(Payment)
    }
    
    class PropertyNotAvailableException {
        +PropertyNotAvailableException(Property, DateRange)
    }
    
    DomainException <|-- InvalidLeasePeriodException
    DomainException <|-- OverduePaymentException
    DomainException <|-- PropertyNotAvailableException
```

- `InvalidLeasePeriodException`
- `OverduePaymentException`
- `PropertyNotAvailableException`

These exceptions enforce domain invariants and prevent invalid state transitions.

---

## 🧠 Domain Services

```mermaid
graph TB
    subgraph "Domain Services"
        LEASE_SERV[LeaseDomainService]
        PAYMENT_SERV[PaymentDomainService]
        PROPERTY_SERV[PropertyDomainService]
    end
    
    subgraph "Service Responsibilities"
        OVERLAP[Prevent Overlapping Leases]
        VALIDATE[Validate Business Rules]
        COORDINATE[Coordinate Multi-Entity Operations]
        CALCULATE[Complex Calculations]
    end
    
    LEASE_SERV --> OVERLAP
    LEASE_SERV --> VALIDATE
    PAYMENT_SERV --> CALCULATE
    PROPERTY_SERV --> COORDINATE
    
    subgraph "Example Operations"
        EX1[Check Lease Conflicts]
        EX2[Calculate Prorated Rent]
        EX3[Validate Property Availability]
    end
    
    OVERLAP --> EX1
    CALCULATE --> EX2
    COORDINATE --> EX3
```

### `LeaseDomainService`
- Encapsulates domain logic that doesn't naturally fit into a single entity
- Example: prevents overlapping leases on the same property

---

## 🧪 Testing the Domain

```mermaid
graph TB
    subgraph "Unit Testing Strategy"
        ENT_TEST[Entity Tests]
        VO_TEST[Value Object Tests]
        SERV_TEST[Domain Service Tests]
        EVENT_TEST[Domain Event Tests]
    end
    
    subgraph "Test Categories"
        BIZ_RULES[Business Rule Validation]
        INVARIANTS[Domain Invariants]
        STATE_TRANS[State Transitions]
        CALCULATIONS[Domain Calculations]
    end
    
    ENT_TEST --> BIZ_RULES
    ENT_TEST --> STATE_TRANS
    VO_TEST --> INVARIANTS
    VO_TEST --> CALCULATIONS
    SERV_TEST --> BIZ_RULES
    EVENT_TEST --> STATE_TRANS
    
    subgraph "Benefits"
        FAST[Fast Execution]
        ISOLATED[No Dependencies]
        RELIABLE[Deterministic]
        FOCUSED[Business Logic Only]
    end
    
    BIZ_RULES --> FAST
    INVARIANTS --> ISOLATED
    STATE_TRANS --> RELIABLE
    CALCULATIONS --> FOCUSED
```

Because the domain layer has no external dependencies, it is highly testable. Business rules can be validated through unit tests targeting the entities and services directly.

---

## 📦 Folder Structure

```
REPM.Domain/
├── Entities/
│   ├── Property.cs
│   ├── User.cs
│   ├── Lease.cs
│   ├── Payment.cs
│   └── BaseEntity.cs
├── ValueObjects/
│   ├── Address.cs
│   ├── Money.cs
│   └── DateRange.cs
├── Enums/
│   ├── LeaseStatus.cs
│   └── PaymentStatus.cs
├── DomainEvents/
│   ├── LeaseCreated.cs
│   ├── PaymentReceived.cs
│   └── PropertyListedForRent.cs
├── DomainExceptions/
│   ├── InvalidLeasePeriodException.cs
│   ├── OverduePaymentException.cs
│   └── PropertyNotAvailableException.cs
├── DomainServices/
│   └── LeaseDomainService.cs
└── Interfaces/
    └── IDomainEvent.cs
```

---

## 🧭 Summary

The domain layer models the **heart of the REPM system**. By adhering to **DDD** and **Clean Architecture**, we ensure the system is robust, expressive, and adaptable to future changes — all while keeping the core logic protected from external concerns.
