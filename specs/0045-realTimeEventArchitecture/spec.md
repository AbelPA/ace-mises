# Implementation Plan: Real-Time Event Communication

**Branch**: `release/pubsub`  
**Date**: 2026-08-13  
**Spec**: `specs/0045-realTimeEventArchitecture/spec.md`

## Summary

Implement an in-memory Pub/Sub infrastructure for the ACE backend using typed asynchronous events and handlers. A dedicated subscriber will integrate with ASP.NET Core SignalR, allowing simulation events to be delivered to React clients in real time.

The first implementation will be single-process and will prioritize simplicity, testability, and low coupling.

## Technical Context

**Backend**: C# / .NET / ASP.NET Core

**Architecture**: Clean Architecture

**Frontend**: React + TypeScript

**Real-time transport**: ASP.NET Core SignalR

**Event transport**: In-memory asynchronous Event Bus

**Testing**: xUnit for the backend; the project's existing JavaScript/TypeScript testing framework for the frontend

**Communication**:

- REST for synchronous client commands;
- SignalR for real-time events;
- Event Bus for internal communication.

**Storage**: Not required for the messaging infrastructure.

**Deployment**: Single process initially.

## Architecture

```text
ACE.Domain
     |
     v
ACE.Application
     |
     +----------------------+
     |                      |
     v                      v
Event abstractions      Use Cases
     |
     v
ACE.Infrastructure
     |
     +--> InMemoryEventBus
     |
     +--> SignalR Event Handlers
     |
     v
ACE.Api
     |
     +--> REST API
     |
     +--> SimulationHub
```

The frontend remains independent:

```text
React
 |
 +--> REST API
 |
 +--> SignalR Hub
```

## Dependency Rules

### ACE.Domain

May contain:

- entities;
- value objects;
- business rules;
- domain events.

Must not reference:

- ASP.NET Core;
- SignalR;
- React;
- HTTP;
- WebSocket;
- Infrastructure.

### ACE.Application

May contain:

- Event Bus interfaces;
- event handlers;
- use cases;
- application contracts.

Must not depend on a concrete Event Bus implementation.

### ACE.Infrastructure

Must contain:

- in-memory Event Bus;
- infrastructure event handlers;
- SignalR integration.

### ACE.Api

Must contain:

- controllers/endpoints;
- SignalR Hub;
- dependency injection configuration;
- application configuration.

### React

Must contain:

- SignalR connection management;
- hooks;
- UI state;
- visual components.

It must not contain ACE domain business logic.

## Event Bus

Define:

```text
IEvent
IEventHandler<T>
IEventBus
```

The Event Bus must:

1. register handlers;
2. publish events;
3. resolve handlers by event type;
4. execute handlers asynchronously;
5. isolate handler exceptions;
6. respect `CancellationToken`.

## Event Dispatch

The initial implementation must use typed dispatch.

Conceptually:

```text
Publish(FoodProduced)
       |
       +--> FoodProducedHandler #1
       |
       +--> FoodProducedHandler #2
       |
       +--> FoodProducedHandler #3
```

Each handler must be independent.

## SignalR Integration

Create a `SimulationHub`.

Responsibilities:

- connection management;
- disconnection;
- joining groups;
- leaving groups.

The Hub must not execute business rules.

Event integration must be implemented through a subscriber:

```text
IEventBus
   |
   v
SignalREventHandler<T>
   |
   v
IHubContext<SimulationHub>
```

## Simulation Groups

The SignalR group must use the simulation's logical identifier.

```text
simulationId = 123

SignalR Group:
"123"
```

Simulation events must be sent through:

```text
Clients.Group(simulationId)
```

Never use `Clients.All` for simulation-specific events.

## API

Create endpoint:

```http
POST /api/simulations
```

Response:

```json
{
    "simulationId": "GUID"
}
```

The endpoint must start the process asynchronously and return the identifier.

## React Integration

Add the official SignalR client:

```text
@microsoft/signalr
```

Create a dedicated layer:

```text
src/
├── signalr/
│   ├── connection.ts
│   ├── simulationHub.ts
│   └── types.ts
│
├── hooks/
│   └── useSimulationEvents.ts
│
└── api/
    └── simulations.ts
```

The hook must encapsulate:

- connection;
- reconnection;
- subscription;
- listeners;
- cleanup.

React components must not directly manipulate the low-level SignalR API.

## Connection Lifecycle

```text
Disconnected
     |
     v
Connecting
     |
     v
Connected
     |
     v
Reconnecting
     |
     +------> Connected
     |
     +------> Disconnected
```

After reconnection:

1. recover the active simulation;
2. rejoin the group;
3. restore listeners when necessary.

## Initial Event Set

Implement initially:

```text
SimulationStarted
FoodProduced
SimulationCompleted
SimulationFailed
```

Additional events may be added later.

## Testing Strategy

### Unit Tests

Test:

- publishing;
- multiple handlers;
- no handlers;
- handler exceptions;
- cancellation;
- handler isolation.

### Integration Tests

Validate:

```text
POST /api/simulations
       |
       v
Event Bus
       |
       v
Simulation Handler
       |
       v
SignalR
```

### Frontend Tests

Validate:

- connection;
- event reception;
- reconnection;
- subscription;
- cleanup.

### End-to-End

Validate:

```text
React
  |
  | POST
  v
API
  |
  v
Simulation
  |
  v
Event Bus
  |
  v
SignalR
  |
  v
React
```

## Constitution Check

### Gate 1 — Domain Isolation

**PASS**

The domain does not depend on SignalR, ASP.NET Core, or React.

### Gate 2 — Testability

**PASS**

The Event Bus is abstracted behind an interface and can be replaced with a test implementation.

### Gate 3 — Simplicity

**PASS**

No external message broker is introduced in the first version.

### Gate 4 — Separation of Concerns

**PASS**

The Event Bus, SignalR, API, and React client have separate responsibilities.

### Gate 5 — Future Scalability

**PASS**

The Event Bus is abstracted to allow a future distributed implementation.

## Project Structure

```text
src/
├── ACE.Domain/
│   └── Events/
│
├── ACE.Application/
│   ├── Abstractions/
│   │   └── Messaging/
│   └── EventHandlers/
│
├── ACE.Infrastructure/
│   └── Messaging/
│
└── ACE.Api/
    └── Hubs/

client/
└── ace-web/
    └── src/
        ├── api/
        ├── signalr/
        ├── hooks/
        └── components/

tests/
├── ACE.Application.Tests/
├── ACE.Infrastructure.Tests/
├── ACE.Api.Tests/
└── ace-web/
```

## Complexity Tracking

No architectural violations are expected.

Do not add:

- MediatR;
- RabbitMQ;
- Kafka;
- Redis;
- excessive generic abstractions;
- a CQRS framework;

just to implement this feature.

The implementation should favor explicit, small, and understandable code.
