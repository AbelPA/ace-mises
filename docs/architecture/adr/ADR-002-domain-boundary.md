# ADR-002: Domain boundary isolation (no advanced economic concepts)

- **Status:** Accepted
- **Date:** 2026-08-12
- **Decision makers:** TechLead
- **Related to:** ADR-001, ADR-003

## Context

v0.1 aims to be the **smallest possible implementation** capable of demonstrating the central proposition:

> Agents have survival needs. Some produce food. When one agent has a surplus that another needs, and a
> reciprocal need exists, a voluntary exchange can occur.

There is a high risk of *scope creep*: introducing money, price, utility, market, preferences, and other
advanced economic concepts too early. The v0.1 specification is emphatic in listing allowed and
forbidden concepts, and in forbidding unnecessary abstractions (interfaces, repositories, services,
factories, artificial aggregates, domain events, additional value objects, price mechanisms, and market
equilibrium mechanisms).

It is necessary to formally record the v0.1 domain boundary as an architectural decision, so that any
future addition goes through a new deliberate decision.

## Decision

The v0.1 domain will contain **exclusively** the concepts:

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

Distributed across the namespaces `ace.domain`, `ace.domain.agents`, `ace.domain.economy`,
`ace.domain.exchange`. No additional namespaces will be created in the domain without explicit need.

Formal prohibitions (do not introduce in v0.1):

```text
Money        Price        Currency     Utility      Interest
Credit       Bank         Capital      Labor         Wage
Tax          Government   Market       MarketEquilibrium
SupplyCurve  DemandCurve  Technology   Entrepreneur
Expectation  Preference   Profit        Loss
```

Abstraction prohibitions (without explicit need): unnecessary interfaces, repositories inside the
domain, generic services, factories without need, artificial aggregates, domain events, additional value
objects, price mechanisms, market equilibrium mechanisms.

Canonical economic rules of v0.1:

- Production is **deterministic**; no production costs; production does not remove resources from
  inventory.
- Need represents only survival; it is not preference/utility/price/intensity/priority.
- Inventory represents physical ownership; no monetary value/price/accounting.
- The **only** economic transfer between agents is via `Exchange` (voluntary bilateral exchange,
  atomic). There are no `Give`, `Transfer`, `Donate`, `Steal`.
- A failed exchange returns `false` and **does not change any stock**.
- The domain **has no `Market`**; the application discovers bilateral trade opportunities.
- Binary survival (`Alive`/`Dead`); no health/age/hunger/damage/recovery/reproduction.

## Consequences

**Positive:**

- Lean, cohesive domain aligned with the theory (human action, voluntary exchange, barter).
- Drastically reduces the risk of *scope creep* and of v0.1 growing beyond what is demonstrable.
- Facilitates testing and review: any concept outside the list is automatically rejected.

**Negative:**

- Limited economic expressiveness in v0.1 (no price, no market).
- Plausible concepts (money, market, limited information) only enter through a deliberate future
  decision (new ADRs/specs).

## Alternatives considered

- **Include `Money`/`Price` since v0.1** — rejected: they belong to v0.2+; introducing them now would
  violate the minimalism goal and the theory of human action based on direct barter.
- **Introduce `Market` in the domain for opportunity discovery** — rejected: the specification
  explicitly forbids `Market` in the domain; the application handles discovery.
- **Layer of generic services/interfaces in the domain** — rejected: the specification forbids
  unnecessary abstractions in v0.1.

## References

- ACE v0.1 specification, sections 3 (Fundamental rule), 23 (Absolute constraints), 24 (Acceptance
  criteria), 26 (Expected outcome).
- `docs/domain/model.md` (Excluded concepts).
- `specs/constitution.md` (Formal prohibitions).