# Glossary — ACE v0.1

Glossary of terms from the domain and architecture of the Agent-Based Computational Economy (ACE) v0.1.
For structural details, see `model.md`.

## ACE
Agent-Based Computational Economy: an agent-based computational economy. In this project, inspired
by Ludwig von Mises's theory of human action (praxeology) — dispersed knowledge, decentralized
coordination, emergent order.

## Agent
Main entity of the domain (`ace.domain.entities.agents.Agent`). Has needs, inventory, production
capabilities (if Producer), and a binary life state. See `model.md`.

## AgentType
Enum (`ace.domain.entities.agents.AgentType`) with values `Producer` and `Consumer`.
- **Producer** — has at least one production capability.
- **Consumer** — has no production capability of its own.
Essential rule: **every Producer is also a consumer**. `AgentType` does not determine who may consume.

## Economic good
Food represented by `FoodType`. In v0.1, the only goods that exist are foods.

## FoodType
Enum (`ace.domain.entities.economy.FoodType`) with values `Wheat`, `Corn`, `Meat`, `Milk`.
No other good (tools, machines, houses, clothes, energy, money) exists in v0.1.

## Need
`ace.domain.entities.economy.Need` (record): minimum quantity of a food that the agent must consume per
period to survive. It does **not** represent preference, utility, price, intensity, or priority —
only survival. Invariant: `QuantityPerPeriod > 0`.

## Production
`ace.domain.entities.economy.Production` (record): **deterministic** capacity to produce a food per
period. No labor/land/capital/technology/costs/uncertainty. Invariant: `QuantityPerPeriod > 0`.

## Inventory
`ace.domain.entities.economy.Inventory` (class): physical stock of foods per `FoodType`, owned by the
agent. Query methods (`Get`/`Has`), addition (`Add`), and conditional removal (`Remove`).
Represents physical property — no monetary value, price, or accounting.

## Barter
Direct exchange of goods for goods, with no money. The only form of economic transfer between
agents in v0.1.

## Offer
`ace.domain.entities.exchange.Offer` (record): barter proposal identifying who offers, what they offer,
how much, what they want to receive, and how much. No monetary price.

## Exchange
`ace.domain.entities.exchange.Exchange` (class): bilateral physical transfer of foods between two agents.
Has `Execute()`, which is **atomic**: a failure does not alter any inventory.

## Bilateral exchange
Every exchange involves both sides delivering a good. There is no unilateral transfer. See
`model.md`.

## Voluntary exchange
The exchange depends on the cooperation of both parties. In v0.1 there is no: `Give`,
`Transfer`, `Donate`, `Steal`. The application discovers trade opportunities; the domain has no
`Market`.

## Binary survival (Alive/Dead)
State `Agent.IsAlive`. Alive: all needs satisfied during consumption. Dead: any need not
satisfied. There is no health, age, partial hunger, damage, recovery, or reproduction.

## Period
Unit of time of the simulation in which the economic cycle runs once (orchestrated by the
application).

## Economic cycle
Mandatory sequence per period: Production → Needs → Trades → Consumption → Survival → next
period. Executed by the application layer, **not** by `Agent`.

## Domain
Central layer (`ace.domain` and sub-namespaces under `entities`: `agents`, `economy`, `exchange`). Represents
rules and economic entities. **It does not know about the simulation.**

## Application
`ace.application` layer. Controls periods, iterates agents, discovers trade opportunities,
orders interactions, runs the simulation, and collects results.

## Infrastructure
`ace.infrastructure` layer. Persistence, storage, logging, external configurations.

## Console
`ace.console` layer. Only starts the simulation, displays results, and handles user I/O.

## Clean Architecture
Layered architecture with dependencies pointing inward (stable domain). Order:
`ace.domain ← ace.application ← ace.infrastructure ← ace.console`. See ADR-001.

## Determinism
Property of v0.1 being minimal and deterministic: deterministic production, no uncertainty, no
variable productivity. Ensures a reproducible simulation. See ADR-003.

## Limited information (future notion)
Planned evolution for v0.2+: agents will not automatically know the inventories and needs of
others. Discovering opportunities will require exploration/exposure of offers.
**Not implemented in v0.1.** See spec `005-limited-information`.

---

## Out of scope (v0.1)

The concepts below **do not exist** in v0.1 and must not be introduced. They belong to future
versions.

- **Money / Currency** — there is no money; the economy operates by barter.
- **Price** — there is no price mechanism; exchanges are bilateral by declared quantities.
- **Market / MarketEquilibrium / SupplyCurve / DemandCurve** — there is no centralized market
  object; the application only discovers bilateral trade opportunities.
- **Utility / Preference** — needs represent survival, not utility or preference.
- **Capital / Labor / Wage / Interest / Credit / Bank / Profit / Loss** — there are no factors of
  production, costs, wages, or financial system.
- **Tax / Government** — there is no State or taxation.
- **Technology / Entrepreneur / Expectation** — production is deterministic; there is no
  technology, entrepreneurship, or subjective expectations in v0.1.

See also: `model.md` (excluded concepts) and the formal prohibition in `specs/constitution.md`.