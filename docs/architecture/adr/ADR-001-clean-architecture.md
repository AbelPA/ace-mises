# ADR-001: Adopt layered Clean Architecture

- **Status:** Accepted
- **Date:** 2026-08-12
- **Decision makers:** TechLead
- **Related to:** ADR-002, ADR-003

## Context

ACE v0.1 is implemented in C#/.NET and needs to clearly separate the domain's economic rules from the
simulation orchestration, the technical infrastructure, and the user interface. The project already
adopts, in `README.md` and in the `src/` folder structure, a layered organization. There is a risk that
the domain will be contaminated by simulation and infrastructure concerns, weakening the economic rules.

The v0.1 specification explicitly requires: the domain project uses `namespace ace.domain`; the domain
**does not know about the simulation**; the domain represents the economic rules and entities; the
application is responsible for running the simulation periods.

## Decision

Adopt Clean Architecture in four layers, with dependencies pointing inward (the domain is the stable
center):

```text
ace.domain
      ↑
ace.application
      ↑
ace.infrastructure
      ↑
ace.console
```

- `ace.domain` — stable center; does not reference any outer layer.
- `ace.application` — depends on `ace.domain`; orchestrates the economic cycle and discovers trade
  opportunities.
- `ace.infrastructure` — depends on `ace.domain` (and on `ace.application` when needed); handles
  persistence, logging, and external configuration.
- `ace.console` — depends on the inner layers; only I/O and initialization.

Rules:

- No outer layer is referenced by the domain.
- The economic cycle (Production → Needs → Trades → Consumption → Survival) is orchestrated by the
  application; `Agent` **does not** contain the cycle sequence.
- The domain has no `Market`; the application discovers bilateral trade opportunities.

## Consequences

**Positive:**

- Stable domain, testable in isolation, and independent of simulation/infrastructure.
- Centralized and clear economic rules; low coupling.
- Facilitates unit testing of the domain without external dependencies.
- Allows swapping infrastructure and console without changing the domain.

**Negative:**

- More projects in the solution and greater initial indirection.
- Requires discipline to keep dependencies pointing inward (continuous verification).

## Alternatives considered

- **Monolithic architecture (single project)** — rejected: it mixes domain rules with simulation and I/O,
  making tests and controlled evolution harder.
- **Feature Folders / Vertical Slices** — rejected for v0.1: the specification's emphasis is on domain
  stability and horizontal separation of responsibilities; vertical slices would dilute the domain
  boundary required by the spec.

## References

- ACE v0.1 specification, sections 2 (Stack and architecture), 19 (Economic cycle), 20 (Separation of
  responsibilities).
- `docs/architecture/architecture.md`.