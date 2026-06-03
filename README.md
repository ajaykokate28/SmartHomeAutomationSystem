# Smart Home Automation System

A C# console application demonstrating core **Object-Oriented Programming (OOP)** principles, all five **SOLID** principles, and eight **Design Patterns** through a realistic Smart Home domain.

---

## Table of Contents

- [Overview](#overview)
- [Technology Stack](#technology-stack)
- [Solution Structure](#solution-structure)
- [Domain Models](#domain-models)
- [Design Patterns Implemented](#design-patterns-implemented)
- [SOLID Principles Applied](#solid-principles-applied)
- [OOP Concepts Applied](#oop-concepts-applied)
- [Service Layer](#service-layer)
- [Unit Tests](#unit-tests)
- [XML Documentation](#xml-documentation)
- [Getting Started](#getting-started)
- [Running Tests](#running-tests)

---

## Overview

The Smart Home Automation System manages smart devices (lights, thermostats, cameras), user accounts, and automation rules. It supports:

- **Device management** — add, update, delete, and query smart home devices.
- **User management** — register and manage Admin and Homeowner users.
- **Automation rules** — create event-driven and time-scheduled rules that control devices automatically.
- **Notifications** — publish real-time notifications when device state changes occur.
- **Reports** — generate summary reports for devices, rules, and users.

---

## Technology Stack

| Component | Details |
|---|---|
| Language | C# 13 |
| Framework | .NET 9.0 |
| Test Framework | xUnit 2.9.2 |
| Mocking Library | Moq 4.20.72 |
| IDE | Visual Studio / VS Code |

---

## Solution Structure

```
Design Pattern Basic Assignment/
│
├── SmartHomeAutomationSystem/          # Main application project
│   ├── Program.cs                      # Entry point — composition root (manual DI)
│   │
│   ├── Models/                         # Domain entities
│   │   ├── Device.cs
│   │   ├── User.cs                     # Abstract base + AdminUser + HomeownerUser
│   │   ├── AutomationRule.cs
│   │   ├── Notification.cs
│   │   └── Report.cs
│   │
│   ├── Services/                       # Business logic layer
│   │   ├── Interfaces/
│   │   │   ├── IDeviceService.cs
│   │   │   ├── IUserService.cs
│   │   │   ├── IAutomationRuleService.cs
│   │   │   └── IReportService.cs
│   │   ├── DeviceService.cs
│   │   ├── UserService.cs
│   │   ├── AutomationRuleService.cs    # Implements IDisposable for timer cleanup
│   │   ├── NotificationService.cs
│   │   └── ReportService.cs
│   │
│   └── Patterns/                       # Design Pattern implementations
│       ├── Factory/
│       │   └── UserFactory.cs          # Factory Method pattern
│       ├── Repository/
│       │   ├── IRepository.cs          # Generic repository interface
│       │   ├── DeviceRepository.cs
│       │   ├── UserRepository.cs
│       │   └── AutomationRuleRepository.cs
│       ├── PubSub/
│       │   ├── EventBus.cs             # Singleton + Pub/Sub pattern
│       │   ├── INotificationSubscriber.cs
│       │   └── PubSubManager.cs        # Facade over EventBus
│       ├── Singleton/
│       │   └── SingletonBase.cs        # Generic Singleton base
│       ├── Interceptor/
│       │   └── InterceptorBase.cs      # Interceptor + LoggingInterceptor
│       ├── LazyLoading/
│       │   └── LazyLoader.cs           # Generic lazy initialisation wrapper
│       └── ResourcePool/
│           └── ResourcePool.cs         # Generic thread-safe object pool
│
└── SmartHomeAutomationSystem.Tests/    # xUnit test project
    ├── Services/
    │   ├── DeviceServiceTests.cs
    │   ├── UserServiceTests.cs
    │   ├── AutomationRuleServiceTests.cs
    │   └── ReportServiceTests.cs
    └── Patterns/
        ├── Factory/
        │   └── UserFactoryTests.cs
        ├── PubSub/
        │   └── EventBusTests.cs
        ├── Repository/
        │   ├── DeviceRepositoryTests.cs
        │   ├── UserRepositoryTests.cs
        │   └── AutomationRuleRepositoryTests.cs
        ├── ResourcePool/
        │   └── ResourcePoolTests.cs
        └── LazyLoading/
            └── LazyLoaderTests.cs
```

---

## Domain Models

### `Device`
Represents a physical smart home device.

| Property | Type | Description |
|---|---|---|
| `Id` | `Guid` | Auto-generated unique identifier |
| `Name` | `string` | Human-readable device name (e.g. "Living Room Light") |
| `Type` | `string` | Device category (e.g. Light, Thermostat, Camera) |
| `Status` | `string` | Current state (e.g. On, Off, Idle) |

---

### `User` (abstract)
Abstract base class for system users. Concrete subtypes define the `Role`.

| Subtype | Role |
|---|---|
| `AdminUser` | `"Admin"` |
| `HomeownerUser` | `"Homeowner"` |

| Property | Type | Description |
|---|---|---|
| `Id` | `Guid` | Auto-generated unique identifier |
| `Name` | `string` | Display name |
| `Role` | `string` | Abstract — implemented by subclass |

---

### `AutomationRule`
Defines a rule that triggers an action when a condition is met.

| Property | Type | Description |
|---|---|---|
| `Id` | `Guid` | Auto-generated unique identifier |
| `Name` | `string` | Rule name |
| `Condition` | `string` | Trigger condition (e.g. `"motion detected"`) |
| `Action` | `string` | Action to execute (e.g. `"turn on light"`) |
| `Schedule` | `string?` | Optional HH:mm schedule; `null` for event-driven rules |

---

### `Notification`
A message published via the `EventBus` when a state change occurs.

| Property | Type | Description |
|---|---|---|
| `Message` | `string` | Notification text |
| `Timestamp` | `DateTime` | Time of creation |

---

### `Report`
A generated summary of system data.

| Property | Type | Description |
|---|---|---|
| `Type` | `string` | Category (e.g. `"Device Usage"`, `"Automation Rules"`) |
| `Content` | `string` | Formatted text body |
| `Timestamp` | `DateTime` | Generation time |

---

## Design Patterns Implemented

### 1. Repository Pattern
**Location:** `Patterns/Repository/`

Abstracts data access behind a generic interface, decoupling business logic from storage.

```csharp
public interface IRepository<T> where T : class
{
    void Add(T entity);
    bool Update(Guid id, T updatedEntity);
    bool Delete(Guid id);
    List<T> GetAll();
    T? GetById(Guid id);
}
```

Concrete implementations: `DeviceRepository`, `UserRepository`, `AutomationRuleRepository`.

---

### 2. Factory Method Pattern
**Location:** `Patterns/Factory/UserFactory.cs`

Centralises creation of `User` subtypes. The caller specifies a role string; the factory returns the correct concrete type.

```csharp
User user = UserFactory.CreateUser("Admin", "Ajay");    // → AdminUser
User user = UserFactory.CreateUser("Homeowner", "Vijay"); // → HomeownerUser
```

Throws `ArgumentException` for unknown roles.

---

### 3. Singleton Pattern
**Location:** `Patterns/Singleton/SingletonBase.cs` and `Patterns/PubSub/EventBus.cs`

`SingletonBase<T>` provides a reusable, thread-safe generic base using `Lazy<T>`.

`EventBus` uses the same approach with `sealed` to prevent inheritance and a `private` constructor to prevent direct instantiation.

```csharp
EventBus.Instance.Subscribe(notificationService);
EventBus.Instance.Publish(new Notification { Message = "..." });
```

---

### 4. Publish/Subscribe (Observer) Pattern
**Location:** `Patterns/PubSub/`

`EventBus` acts as the message broker. Subscribers implement `INotificationSubscriber.Receive()`. Thread safety is ensured via `lock` and snapshot-before-dispatch.

```
EventBus
  ├── Subscribe(INotificationSubscriber)
  ├── Unsubscribe(INotificationSubscriber)
  └── Publish(Notification) → broadcasts to all subscribers (thread-safe snapshot)
```

`PubSubManager` is a Facade over `EventBus` that simplifies lifecycle management (bulk unsubscribe via `DeregisterAll()`).

---

### 5. Lazy Loading Pattern
**Location:** `Patterns/LazyLoading/LazyLoader.cs`

Generic wrapper that defers expensive object creation until the first access.

```csharp
var loader = new LazyLoader<ExpensiveService>(() => new ExpensiveService());
// Service is NOT created yet

var svc = loader.Value; // Service created here on first access
bool ready = loader.IsValueCreated; // true
```

---

### 6. Object Pool (Resource Pool) Pattern
**Location:** `Patterns/ResourcePool/ResourcePool.cs`

Manages a pool of reusable objects to avoid repeated allocation. Thread-safe via `lock`.

```csharp
var pool = new ResourcePool<DbConnection>(() => new DbConnection(), maxSize: 10);

var conn = pool.Acquire();   // gets from pool or creates new
// ... use conn ...
pool.Release(conn);          // returns to pool for reuse
```

---

### 7. Interceptor Pattern
**Location:** `Patterns/Interceptor/InterceptorBase.cs`

Abstract base that wraps operations with `BeforeExecute` / `AfterExecute` hooks. Implements cross-cutting concerns such as logging and auditing without modifying business logic.

```csharp
var interceptor = new LoggingInterceptor();
interceptor.Execute("SaveDevice", () => deviceService.AddDevice("Light", "Light", "On"));
// Output:
// [LoggingInterceptor] Starting 'SaveDevice'
// Device added successfully.
// [LoggingInterceptor] Completed 'SaveDevice' in 2ms
```

---

### 8. Facade Pattern
**Location:** `Patterns/PubSub/PubSubManager.cs`

Simplifies the `EventBus` API by wrapping subscribe/unsubscribe lifecycle management.

```csharp
var manager = new PubSubManager(EventBus.Instance);
manager.Register(notificationService);
// later...
manager.DeregisterAll(); // cleans up all subscriptions at once
```

---

## SOLID Principles Applied

### S — Single Responsibility Principle
Each class has one reason to change:
- `DeviceService` → only manages device CRUD.
- `NotificationService` → only handles notification display.
- `ReportService` → only generates reports.
- `UserFactory` → only creates `User` instances.
- Repository classes → only persist a single entity type.

---

### O — Open/Closed Principle
Classes are open for extension but closed for modification:
- `User` is abstract; new roles are added by creating new subclasses (`AdminUser`, `HomeownerUser`) without touching existing code.
- `InterceptorBase` is extended by `LoggingInterceptor` without modifying the base class.
- New repository types implement `IRepository<T>` without changing service code.

---

### L — Liskov Substitution Principle
Subtypes are fully substitutable for their base types:
- `AdminUser` and `HomeownerUser` both behave correctly wherever `User` is expected.
- `DeviceRepository`, `UserRepository`, `AutomationRuleRepository` all satisfy `IRepository<T>` completely.

---

### I — Interface Segregation Principle
Interfaces are narrow and role-specific:
- `IDeviceService` — device operations only.
- `IUserService` — user operations only.
- `IAutomationRuleService` — rule operations only.
- `IReportService` — reporting only.
- `IRepository<T>` — data access only.
- `INotificationSubscriber` — notification receipt only.

No client is forced to depend on methods it doesn't use.

---

### D — Dependency Inversion Principle
High-level modules depend on abstractions, not concretions:

```csharp
// DeviceService depends on IRepository<Device>, not DeviceRepository
public DeviceService(IRepository<Device> repository) { ... }

// ReportService depends on service interfaces, not concrete services
public ReportService(IDeviceService deviceService,
                     IAutomationRuleService ruleService,
                     IUserService userService) { ... }
```

Dependencies are wired together in `Program.cs` (composition root):

```csharp
IDeviceService deviceService = new DeviceService(new DeviceRepository());
IUserService userService = new UserService(new UserRepository());
IAutomationRuleService ruleService = new AutomationRuleService(new AutomationRuleRepository());
IReportService reportService = new ReportService(deviceService, ruleService, userService);
```

---

## OOP Concepts Applied

| Concept | Where |
|---|---|
| **Encapsulation** | All fields are `private readonly`; state is only exposed through properties and methods |
| **Abstraction** | `User` is abstract; service interfaces define contracts without revealing implementation |
| **Inheritance** | `AdminUser` and `HomeownerUser` extend `User`; `LoggingInterceptor` extends `InterceptorBase` |
| **Polymorphism** | Factory returns `User` base type; callers work with `IDeviceService`, `IUserService` etc. regardless of concrete type |
| **Composition** | Services compose repositories; `ReportService` composes three service interfaces |
| **`required` keyword** | All non-nullable model properties use `required` for compile-time safety |
| **`IDisposable`** | `AutomationRuleService` implements `IDisposable` to stop and dispose its background timer |

---

## Service Layer

### `DeviceService`
| Method | Description |
|---|---|
| `AddDevice(name, type, status)` | Creates and persists a new device; publishes a notification |
| `UpdateDevice(id, name, type, status)` | Updates an existing device; publishes a status-change notification |
| `DeleteDevice(id)` | Removes a device by ID |
| `ViewDevices()` | Prints all devices to the console |
| `GetAllDevices()` | Returns all devices as a list |

---

### `UserService`
| Method | Description |
|---|---|
| `RegisterUser(name, role)` | Creates a role-specific user via `UserFactory` and persists it |
| `UpdateUser(id, newName)` | Renames an existing user |
| `DeleteUser(id)` | Removes a user by ID |
| `ViewUsers()` | Prints all users to the console |
| `GetAllUsers()` | Returns all users as a list |

---

### `AutomationRuleService`
| Method | Description |
|---|---|
| `CreateRule(name, condition, action, schedule?)` | Creates a new automation rule |
| `UpdateRule(id, ...)` | Updates an existing rule |
| `DeleteRule(id)` | Removes a rule by ID |
| `ExecuteRules(deviceService)` | Evaluates all rules against current device state |
| `StartScheduler(deviceService)` | Starts a background timer that checks time-scheduled rules every 5 seconds |
| `GetAllRules()` | Returns all rules |
| `GetRuleById(id)` | Returns a single rule by ID |
| `Dispose()` | Stops and disposes the background timer |

---

### `ReportService`
| Method | Description |
|---|---|
| `GenerateDeviceUsageReport()` | Reports all devices with their current status |
| `GenerateAutomationRuleReport()` | Reports all automation rules |
| `GenerateUserActivityReport()` | Reports all users and their roles |

---

### `NotificationService`
Implements `INotificationSubscriber`. Receives and prints `Notification` messages published via the `EventBus`.

---

## Unit Tests

**75 tests — 100% pass rate.**

| Test Class | Tests | Coverage Focus |
|---|---|---|
| `DeviceServiceTests` | 7 | Add, Update (found/not-found), Delete (found/not-found), GetAll (populated/empty) |
| `UserServiceTests` | 7 | Register (Admin/Homeowner/invalid role), Update (found/not-found), Delete, GetAll |
| `AutomationRuleServiceTests` | 11 | Create, Update (found/not-found), Delete, GetAll, GetById (found/null), ExecuteRules (motion match/no match), Schedule |
| `ReportServiceTests` | 4 | Each report method calls the correct service, no-throw on empty data |
| `UserFactoryTests` | 8 | Admin/Homeowner creation, case-insensitivity (`[Theory]`), invalid role, empty role |
| `EventBusTests` | 6 | Singleton identity, Publish to multiple subscribers, duplicate-subscribe guard, Unsubscribe, null subscriber guard, null notification guard |
| `DeviceRepositoryTests` | 7 | Add, GetAll (empty), GetById (found/null), Update (found/not-found), Delete (found/not-found) |
| `UserRepositoryTests` | 6 | Add, GetById, Update (found/not-found), Delete (found/not-found) |
| `AutomationRuleRepositoryTests` | 5 | Add, GetById, Update, Delete (found/not-found) |
| `ResourcePoolTests` | 6 | Acquire (empty pool), Acquire after Release (reuse), Release increments count, maxSize cap, Release null guard, constructor null guard |
| `LazyLoaderTests` | 5 | Value result, factory called once only, IsValueCreated false/true, constructor null guard |

### Test Tools

- **xUnit** — test runner and assertions
- **Moq** — mocking `IRepository<T>` and service interfaces to isolate units under test

### Example Test

```csharp
[Fact]
public void ExecuteRules_MotionDetected_ShouldUpdateLightDevice()
{
    var rule = new AutomationRule
    {
        Name = "Motion Light",
        Condition = "motion detected",
        Action = "turn on light"
    };
    _repositoryMock.Setup(r => r.GetAll()).Returns(new List<AutomationRule> { rule });

    var lightId = Guid.NewGuid();
    var deviceServiceMock = new Mock<IDeviceService>();
    deviceServiceMock.Setup(d => d.GetAllDevices()).Returns(new List<Device>
    {
        new() { Id = lightId, Name = "Living Room Light", Type = "Light", Status = "Off" }
    });

    _sut.ExecuteRules(deviceServiceMock.Object);

    deviceServiceMock.Verify(
        d => d.UpdateDevice(lightId, "Living Room Light", "Light", "On"),
        Times.Once);
}
```

---

## XML Documentation

All public types and members carry `/// <summary>` XML documentation comments. An XML documentation file is automatically generated at build time:

```
bin/Debug/net9.0/SmartHomeAutomationSystem.xml
```

Enabled in `SmartHomeAutomationSystem.csproj`:

```xml
<GenerateDocumentationFile>true</GenerateDocumentationFile>
<DocumentationFile>bin\$(Configuration)\$(TargetFramework)\SmartHomeAutomationSystem.xml</DocumentationFile>
```

Implementations use `/// <inheritdoc/>` to inherit documentation from their interfaces, ensuring no duplication.

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Clone / Open

```bash
cd "Design Pattern Basic Assignment"
```

### Build

```bash
dotnet build SmartHomeAutomationSystem/SmartHomeAutomationSystem.csproj
```

### Run

```bash
dotnet run --project SmartHomeAutomationSystem/SmartHomeAutomationSystem.csproj
```

**Expected console output (excerpt):**

```
=== Device Management ===
Device added successfully.
Device added successfully.
ID: ..., Name: Living Room Light, Type: Light, Status: Off
ID: ..., Name: Thermostat, Type: Thermostat, Status: Idle
[Notification] [14:00:00] Device 'Living Room Light' status changed to 'On'.
Device updated.
Device deleted.
ID: ..., Name: Thermostat, Type: Thermostat, Status: Idle

=== User Management ===
User registered successfully.
...

=== Automation Rules ===
Automation rule created.
Scheduler started. Checking for scheduled rules...
Press Enter to stop the scheduler and generate reports...

=== Reports ===
[2026-05-25 14:00:00] Device Usage Report:
Garden Light (Light) - Status: On
...
```

---

## Running Tests

```bash
cd SmartHomeAutomationSystem.Tests
dotnet test
```

**Expected output:**

```
Test Run Successful.
Total tests: 75
     Passed: 75
 Total time: ~2.4 Seconds
```

To run a specific test class:

```bash
dotnet test --filter "FullyQualifiedName~DeviceServiceTests"
```

To run with verbose output:

```bash
dotnet test --logger "console;verbosity=normal"
```

---

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│                          Program.cs                                  │
│                    (Composition Root / Manual DI)                    │
└────────────────────────────┬────────────────────────────────────────┘
                             │ constructs via interfaces
          ┌──────────────────┼────────────────────┐
          ▼                  ▼                    ▼
  ┌──────────────┐  ┌───────────────┐  ┌─────────────────────┐
  │ DeviceService│  │  UserService  │  │AutomationRuleService│
  │ :IDeviceServ │  │ :IUserService │  │ :IAutomationRule... │
  └──────┬───────┘  └──────┬────────┘  └──────────┬──────────┘
         │                 │                       │
         ▼                 ▼                       ▼
  IRepository<Device>  IRepository<User>  IRepository<AutomationRule>
         │                 │                       │
         ▼                 ▼                       ▼
  DeviceRepository   UserRepository    AutomationRuleRepository
                                                   │
                             ┌─────────────────────┘
                             │ calls IDeviceService
                             ▼
                    ┌─────────────────┐
                    │  EventBus       │  ← Singleton + Pub/Sub
                    │  (thread-safe)  │
                    └────────┬────────┘
                             │ publishes Notification
                             ▼
                    NotificationService
                    (INotificationSubscriber)
```
