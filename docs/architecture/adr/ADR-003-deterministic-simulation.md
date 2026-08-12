# ADR-003: Deterministic simulation orchestrated by the application

- **Status:** Accepted
- **Date:** 2026-08-12
- **Decision makers:** TechLead
- **Related to:** ADR-001, ADR-002

## Context

v0.1 must be **minimal, deterministic, and domain-oriented**. The specification requires that the
economic cycle per period follow a mandatory order (Production → Needs → Trades → Consumption →
Survival) and that this sequence **not** be implemented inside `Agent` — it is the application layer's
role to orchestrate it. Production is deterministic; uncertainty, variable productivity, technology,
expectations, or subjective preferences are not modeled in v0.1.

The separation is important because mixing simulation and domain would create coupling between economic
rules and temporal control, making it harder to reuse and test the domain.

## Decision

1. The **application layer** (`ace.application`) orchestrates the per-period economic cycle:
   - Production: calls `Agent.Produce()`.
   - Needs: queries `Agent.UnfulfilledNeeds()` / `Agent.NeedsFood()`.
   - Trades: discovers bilateral trade opportunities (A needs X + B has X + B needs Y + A has Y) and
     executes `Exchange.Execute()`.
   - Consumption: calls `Agent.Consume()`.
   - Survival: observes `Agent.IsAlive`.
   
2. `Agent` **does not** contain the cycle sequence; it only exposes individual domain operations.

3. v0.1 is **deterministic**: deterministic production; no uncertainty, expectation, variable
   productivity, technology, subjective preferences, multi-round negotiation, auctions, or centralized
   market.

4. Future **limited information** (agents do not automatically know others' stocks/needs) is treated as
   later evolution (spec `005-limited-information`), **not** in v0.1.

## Consequences

**Positive:**

- **Reproducible** simulation: same inputs produce same results, ideal for tests and A/B scenario
  validation.
- Domain remains stable and testable in isolation, with no dependency on time/control.
- Experiments are replicable and auditable.

**Negative:**

- Lower behavioral expressiveness in v0.1 (no stochasticity, no uncertainty).
- Opportunity discovery in v0.1 assumes complete information (admitted limitation; addressed in future
  evolution).

## Alternatives considered

- **Economic cycle inside `Agent`** — rejected: the specification explicitly requires that the sequence
  does not reside in `Agent`; it would mix simulation control with domain rules.
- **Stochastic simulation since v0.1** — rejected: the specification requires determinism in v0.1 and
  forbids uncertainty/variable productivity in this version.
- **Opportunity discovery in the domain (a `Market` object)** — rejected: it violates ADR-002 and the
  prohibition of `Market`.

## References

- ACE v0.1 specification, sections 10 (Agent production), 19 (Economic cycle), 23 (Absolute
  constraints), 26 (Expected outcome).
- `docs/architecture/architecture.md` (Economic cycle).
- Prospective spec `specs/005-limited-information/`.