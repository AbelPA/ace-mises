# Architecture — ACE v0.1

> Architecture overview of the Agent-Based Computational Economy (ACE) v0.1.
> Implemented in C#/.NET with Clean Architecture, object orientation, `record` for simple value objects,
> and `class` for entities with state and behavior.

## Overview

ACE v0.1 is minimal, deterministic, and domain-oriented. The domain is the stable center of the system
and does not know about the simulation. The application layer orchestrates the per-period economic
cycle; infrastructure handles persistence/logging/configuration; the console is only user
input/output.

The fundamental rule is: **do not invent concepts**. Only the concepts explicitly required by the v0.1
specification exist in the domain (`Agent`, `AgentType`, `FoodType`, `Inventory`, `Need`, `Production`,
`Offer`, `Exchange`).

## Layers

```text
ace.domain
      ↑
ace.application
      ↑
ace.infrastructure
      ↑
ace.console
```

The arrows indicate **dependency direction**: all layers depend on the domain (and on inner layers),
never the other way around. The domain is the stable center and does not reference any outer layer.

## Responsibilities

| Layer              | Project (csproj)                | Root namespace       | Responsibilities                                                                  | Allowed dependencies                  |
|--------------------|---------------------------------|----------------------|------------------------------------------------------------------------------------|--------------------------------------|
| Domain             | `src/domain/ace.domain`         | `ace.domain`         | Represent agents, needs, production, inventory, offers, execute trades, apply survival. | None external                        |
| Application        | `src/application/ace.application` | `ace.application`    | Control periods, iterate agents, discover trade opportunities, order interactions, run the simulation, collect results. | `ace.domain`                         |
| Infrastructure     | `src/infraestructure/ace.infrastructure` | `ace.infrastructure` (or `infrastructure`) | Persistence, storage, logging, external configuration. | `ace.domain`, `ace.application`      |
| Console            | `src/console/ace.console`       | `ace.console`        | Start the simulation, display results, user I/O.                                    | `ace.domain`, `ace.application`, `ace.infrastructure` |

> Naming note: the repo contains the `src/infraestructure` directory (the project's current spelling).
> Preserve the existing spelling when creating the corresponding project.

## Domain namespaces

The domain project uses:

```csharp
namespace ace.domain;
```

Allowed sub-namespaces (do not create additional namespaces inside the domain without explicit need):

| Namespace                | Concepts                          |
|--------------------------|------------------------------------|
| `ace.domain.entities.economy`     | `FoodType`, `Need`, `Production`, `Inventory` |
| `ace.domain.entities.agents`      | `Agent`, `AgentType`               |
| `ace.domain.entities.exchange`    | `Offer`, `Exchange`                |

## Rule: do not invent concepts

The implementation must contain **only** these domain concepts:

```text
Agent
AgentType
FoodType
Inventory
Need
Production
Offer
Exchange
```

Do not create:

```text
Market           Money            Price             Utility          Interest
Credit           Bank             Capital           Labor             Wage
Tax              Government       MarketEquilibrium SupplyCurve       DemandCurve
Technology       Entrepreneur     Expectation        Preference        Profit
Loss
```

Also do not create unnecessary interfaces, repositories inside the domain, generic services, factories
without need, artificial aggregates, domain events, additional value objects, price mechanisms, or
market equilibrium mechanisms. If a feature can be implemented directly by the existing entities, do not
create a new abstraction.

## Mapping to projects

```text
src/
├── domain/
│   └── ace.domain/            → ace.domain (+ sub-namespaces)
├── application/
│   └── ace.application/      → ace.application
├── infraestructure/
│   └── ace.infrastructure/   → ace.infrastructure
├── console/
│   └── ace.console/          → ace.console
└── tests/
    └── ace.tests/            → unit tests (domain + A/B scenario)
```

The `ace-mises.sln` solution organizes the projects under the `src` virtual folder. Currently only
`ace.domain` exists; the remaining layers and tests must be created according to the v0.1 specification
implementation plan.

## Economic cycle

The cycle for each period is executed by the **application layer**, not the domain. Mandatory order:

```text
┌─────────────────────┐
│ Period start        │
└──────────┬──────────┘
           ↓
┌─────────────────────┐
│ Production          │
└──────────┬──────────┘
           ↓
┌─────────────────────┐
│ Needs               │
└──────────┬──────────┘
           ↓
┌─────────────────────┐
│ Trades              │
└──────────┬──────────┘
           ↓
┌─────────────────────┐
│ Consumption         │
└──────────┬──────────┘
           ↓
┌─────────────────────┐
│ Survival            │
└──────────┬──────────┘
           ↓
       Next period
```

Do not implement this sequence inside `Agent`. The application:

- calls `Agent.Produce()` in the Production phase;
- queries `Agent.UnfulfilledNeeds()` / `Agent.NeedsFood()` in the Needs phase;
- looks for bilateral trade opportunities and executes `Exchange.Execute()` in the Trades phase;
- calls `Agent.Consume()` in the Consumption phase;
- observes `Agent.IsAlive` in the Survival phase.

The application discovers trade opportunities (A needs X + B has X + B needs Y + A has Y).
**The domain has no `Market`.**

## Separation of responsibilities

- **Domain** — represent agents, needs, production, inventory, offers, execute trades, apply survival
  rules.
- **Application** — control periods, iterate agents, look for trade opportunities, order interactions,
  run the simulation, collect results.
- **Infrastructure** — persistence, storage, logging, external configuration.
- **Console** — only start the simulation, display results, user input/output.

## Future evolution

The next evolution of the model will introduce **limited information**: agents will not automatically
know the stocks and needs of others. This evolution is treated as a later change (spec
`005-limited-information`) and **must not be implemented in v0.1**. Concepts such as money, price,
market, utility, subjective preferences, uncertainty, and negotiation belong to future versions and
remain prohibited.

## References

- [ADR-001 — Layered Clean Architecture](adr/ADR-001-clean-architecture.md)
- [ADR-002 — Domain boundary isolation](adr/ADR-002-domain-boundary.md)
- [ADR-003 — Deterministic simulation orchestrated by the application](adr/ADR-003-deterministic-simulation.md)
- Domain model: `../domain/model.md`
- Glossary: `../domain/glossary.md`
- Executable specifications (Spec Kit): `../../specs/`