# ACE — Constitution

> Governing principles of the Agent-Based Computational Economy (ACE) specification suite, version
> v0.1. These principles delimit the boundary that every spec in this suite must respect.

ACE is an agent-based computational economy inspired by Ludwig von Mises's theory of human action
(praxeology). v0.1 is **minimal, deterministic, and domain-oriented**.

## Principles

1. **Minimal domain, domain-oriented** — the implementation contains exclusively the explicit
   concepts of the specification (`Agent`, `AgentType`, `FoodType`, `Inventory`, `Need`, `Production`,
   `Offer`, `Exchange`). Nothing is invented. No money/price/market or other advanced economic concepts.

2. **Clean Architecture** — the domain (`ace.domain` and its sub-namespaces) is the stable core and
   **does not know the simulation**. Dependencies point inward:
   `ace.domain ← ace.application ← ace.infrastructure ← ace.console`. The application orchestrates the
   economic cycle; `Agent` does not contain the cycle sequence.

3. **Determinism** — v0.1 is deterministic: deterministic production, no uncertainty, no variable
   productivity, no expectations. Same inputs always produce the same results.

4. **Voluntary bilateral exchange** — the only economic transfer between agents is exchange via
   `Exchange`, bilateral and voluntary. The exchange is **atomic**: if either side cannot deliver,
   `Execute()` returns `false` and **no inventory is altered**. There are no `Give`/`Transfer`/
   `Donate`/`Steal` in v0.1.

5. **Binary survival** — the `Agent.IsAlive` state is binary (`Alive`/`Dead`). If any need cannot be
   fulfilled at consumption, the agent dies. No health, age, partial hunger, damage, recovery, or
   reproduction.

6. **Incremental evolution** — limited information and advanced economic concepts belong to future
   versions (v0.2+) and are **never** introduced in v0.1. Every addition of a concept requires a
   deliberate decision (ADR) and a dedicated spec.

## Formal prohibitions

The concepts below **do not exist** in v0.1 and must not be introduced in any spec of this suite without
an explicit architectural decision that promotes them:

```text
Money            Price          Currency         Utility          Interest
Credit           Bank           Capital          Labor            Wage
Tax              Government     Market           MarketEquilibrium
SupplyCurve      DemandCurve    Technology       Entrepreneur
Expectation      Preference     Profit           Loss
```

Also prohibited in v0.1: perfect/imperfect information (in the sense of complex discovery), complex
discovery, negotiation, multiple rounds of bargaining, prices, auctions, centralized market, variable
productivity, uncertainty, subjective preferences.

## Specs structure

| Spec                          | Purpose                                                          | Status      |
|-------------------------------|------------------------------------------------------------------|-------------|
| `001-minimal-domain`          | Minimal domain concepts, agent types, Agent, Inventory, Need, Production, FoodType. | Implementable v0.1 |
| `002-production`              | `Agent.Produce()` behavior and deterministic production.        | Implementable v0.1 |
| `003-barter`                  | Barter: `Offer`, `Exchange`, atomic bilateral exchange, opportunity criterion. | Implementable v0.1 |
| `004-survival`                | `UnfulfilledNeeds`, `NeedsFood`, `Consume`, binary survival, economic cycle and A/B scenario. | Implementable v0.1 |
| `005-limited-information`     | Limited information (future evolution).                          | Prospective (v0.2+) |

> The `005-limited-information` spec is **prospective** and must **not** be implemented in v0.1.