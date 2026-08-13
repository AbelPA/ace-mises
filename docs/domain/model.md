# Domain Model — ACE v0.1

> Reference documentation for the domain model of the **Agent-Based Computational Economy (ACE)**
> v0.1, inspired by Ludwig von Mises's theory of human action (praxeology). This version is
> **minimal, deterministic, and domain-oriented**.
> For the executable formal specification, see the specs in `specs/`.

## Overview

The v0.1 domain represents an economy in which agents have basic food needs, some produce foods,
store foods in inventory, identify needs they cannot satisfy, perform voluntary barter trades,
consume foods at the end of each period, and stay alive or die when they cannot satisfy their
needs.

The domain **does not know about the simulation**. It only represents the economic rules and
entities. The cycle (production → needs → trades → consumption → survival) is orchestrated by the
application layer, never by `Agent`.

## Relationships

```text
                          ┌───────────┐
                          │ AgentType │ (enum: Producer | Consumer)
                          └─────┬─────┘
                                │ used by
                                ▼
    ┌──────────────────────────────────────────────────┐
    │                      Agent                       │
    │  Id · Name · Type · Inventory · Needs            │
    │  Productions · IsAlive                           │
    │                                                  │
    │  Produce() · UnfulfilledNeeds() · NeedsFood()   │
    │  Consume()                                       │
    └───┬───────────┬──────────────────┬───────────────┘
        │           │                  │
        │ has        │ has              │ identifies
        ▼           ▼                  ▼
   ┌─────────┐  ┌────────────┐  ┌───────────────────┐
   │ Inventory│  │ Need       │  │ Production        │
   │         │  │ (record)   │  │ (record, determin.)│
   │<Food,int>│  └────────────┘  └───────────────────┘
   └────┬────┘
        │ referenced by
        ▼
  ┌────────────────────────────────────────────────────┐
  │                      Exchange                      │
  │  Buyer · Seller · FoodFromBuyer · QuantityFromBuyer│
  │  FoodFromSeller · QuantityFromSeller · Execute()  │
  └────────────────────────────────────────────────────┘
        ▲ referenced by
        │
   ┌────┴────┐
   │  Offer  │ (record: barter proposal)
   └─────────┘

FoodType (enum: Wheat | Corn | Meat | Milk) — used by Inventory, Need, Production, Offer, Exchange
```

## Entities and value objects

| Name        | Type   | Namespace            | Responsibility                            | Constraints                                  |
|-------------|--------|----------------------|-------------------------------------------|---------------------------------------------|
| `FoodType`  | enum   | `ace.domain.entities.economy` | Identifies the economic goods            | Only Wheat, Corn, Meat, Milk              |
| `Need`      | record | `ace.domain.entities.economy` | Survival need per period                | `QuantityPerPeriod > 0`                     |
| `Production`| record | `ace.domain.entities.economy` | Deterministic production capacity        | `QuantityPerPeriod > 0`; deterministic    |
| `Inventory` | class  | `ace.domain.entities.economy` | Physical stock of foods                  | `Add`/`Remove` require qty > 0; physical   |
| `AgentType` | enum   | `ace.domain.entities.agents`  | Agent type                               | Producer is also a consumer                |
| `Agent`     | class  | `ace.domain.entities.agents`  | Main entity with state/behavior          | Binary `IsAlive`; knows its own rules     |
| `Offer`     | record | `ace.domain.entities.exchange`| Barter proposal                          | No monetary price                          |
| `Exchange`  | class  | `ace.domain.entities.exchange`| Bilateral food trade                     | Atomic; failure does not alter inventories |

---

## FoodType

Identifies the only goods in v0.1.

```csharp
namespace ace.domain.entities.economy;

public enum FoodType
{
    Wheat,
    Corn,
    Meat,
    Milk
}
```

Do not add other foods. Do not model tools, machines, houses, clothes, energy, or money.

---

## Need

A need represents a minimum quantity of food that the agent must consume per period to survive.
It does **not** represent preference, utility, price, intensity, or priority. It represents
exclusively a survival need.

```csharp
namespace ace.domain.entities.economy;

public sealed record Need(
    FoodType Food,
    int QuantityPerPeriod);
```

Example: `new Need(FoodType.Meat, 1);`

**Minimum validation:** `QuantityPerPeriod` must be greater than zero.

---

## Production

A production represents the **deterministic** capacity to produce a given food during a period.

```csharp
namespace ace.domain.entities.economy;

public sealed record Production(
    FoodType Food,
    int QuantityPerPeriod);
```

Example: `new Production(FoodType.Wheat, 5);`

Production is deterministic. Do not model labor, land, capital, productivity, technology, costs,
factors of production, or productive uncertainty.

**Minimum validation:** `QuantityPerPeriod` must be greater than zero.

---

## Inventory

Every agent owns a physical stock of foods. It represents the agent's physical property.
Do not implement monetary value, price, valuation, accounting, or inventory cost.

```csharp
namespace ace.domain.entities.economy;

public sealed class Inventory
{
    private readonly Dictionary<FoodType, int> _items = new();

    public int Get(FoodType food)
        => _items.GetValueOrDefault(food);

    public bool Has(FoodType food, int quantity)
        => Get(food) >= quantity;

    public void Add(FoodType food, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        _items[food] = Get(food) + quantity;
    }

    public bool Remove(FoodType food, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        if (!Has(food, quantity))
            return false;

        _items[food] -= quantity;

        return true;
    }

    public IReadOnlyDictionary<FoodType, int> Items
        => _items;
}
```

---

## AgentType

```csharp
namespace ace.domain.entities.agents;

public enum AgentType
{
    Producer,
    Consumer
}
```

- **Producer** — has at least one production capability.
- **Consumer** — has no production capability of its own.

**Important rule:** every Producer is also a consumer. `AgentType` does **not** determine whether
the agent may consume. All agents have needs, inventory, may consume, and may take part in
trades.

```text
Producer != non-consumer
Consumer != agent without needs
```

---

## Agent

`Agent` is the main entity of the domain.

```csharp
using ace.domain.entities.economy;

namespace ace.domain.entities.agents;

public sealed class Agent
{
    public Guid Id { get; }

    public string Name { get; }

    public AgentType Type { get; }

    public Inventory Inventory { get; }

    public IReadOnlyList<Need> Needs { get; }

    public IReadOnlyList<Production> Productions { get; }

    public bool IsAlive { get; private set; }

    public Agent(
        string name,
        AgentType type,
        IEnumerable<Need> needs,
        IEnumerable<Production> productions)
    {
        Id = Guid.NewGuid();

        Name = name;
        Type = type;

        Needs = needs.ToList();
        Productions = productions.ToList();

        Inventory = new Inventory();

        IsAlive = true;
    }
}
```

The agent has behavior for: `Produce()`, `UnfulfilledNeeds()`, `NeedsFood()`, `Consume()`.
The details of these behaviors live in specs `002-production` and `004-survival`.

### Agent production (`Produce()`)

1. Checks whether the agent is alive.
2. Iterates over all its productions.
3. Adds the produced quantity to the inventory.

```text
Production
     ↓
Inventory
```

Production **does not remove** resources from the inventory. Do not model production costs.
If the agent is dead, it does not produce.

### Unfulfilled needs (`UnfulfilledNeeds()` / `NeedsFood()`)

`UnfulfilledNeeds()` returns the needs whose required quantity is **not** available in the
inventory.

Example: `Need: Meat = 1` with `Inventory: Meat = 0` → `Meat = 1` unfulfilled.
With `Inventory: Meat = 1` → satisfied.

`NeedsFood()` returns `true` when at least one unfulfilled need exists.

### Consumption and survival (`Consume()`)

Consumption happens after the trade attempts. `Consume()` must:

1. Ignore dead agents.
2. Check each need.
3. Remove the required quantity from the inventory.
4. If any need cannot be satisfied, set `IsAlive = false`.

Death occurs when the agent cannot fully satisfy its needs.
Survival is **binary** (`Alive` / `Dead`).

Do not implement health, aging, degrees of hunger, damage, recovery, reproduction, or partial
death.

---

## Offer

An offer represents a barter proposal. It represents only: who offers, what they offer, how much
they offer, what they want to receive, and how much they want to receive. **Do not add a monetary
price.**

```csharp
using ace.domain.entities.agents;
using ace.domain.entities.economy;

namespace ace.domain.entities.exchange;

public sealed record Offer(
    Agent Seller,
    FoodType OfferedFood,
    int OfferedQuantity,
    FoodType RequestedFood,
    int RequestedQuantity);
```

---

## Exchange

A trade represents the physical transfer of foods between two agents.

```csharp
using ace.domain.entities.agents;
using ace.domain.entities.economy;

namespace ace.domain.entities.exchange;

public sealed class Exchange
{
    public Agent Buyer { get; }

    public Agent Seller { get; }

    public FoodType FoodFromBuyer { get; }

    public int QuantityFromBuyer { get; }

    public FoodType FoodFromSeller { get; }

    public int QuantityFromSeller { get; }

    public Exchange(
        Agent buyer,
        Agent seller,
        FoodType foodFromBuyer,
        int quantityFromBuyer,
        FoodType foodFromSeller,
        int quantityFromSeller)
    {
        Buyer = buyer;
        Seller = seller;

        FoodFromBuyer = foodFromBuyer;
        QuantityFromBuyer = quantityFromBuyer;

        FoodFromSeller = foodFromSeller;
        QuantityFromSeller = quantityFromSeller;
    }

    public bool Execute()
    {
        // validate both inventories
        // execute bilateral transfer
        // return success or failure
    }
}
```

### Execution rule (atomic)

A trade can only occur when:

1. the first agent has the good it will deliver;
2. the second agent has the good it will deliver;
3. both quantities are valid;
4. both sides can perform the transfer.

The operation must be atomic from the domain's point of view:

```text
If A has X
and B has Y

A → X
B → Y

only then:

A receives Y
B receives X
```

Never allow `A loses X` and `B cannot deliver Y`. If any condition fails,
`Execute() == false` and **no inventory must be altered**.

### Voluntary trade rule

The trade cannot be unilateral. Do not implement `Give()`, `Transfer()`, `Donate()`, `Steal()`
as economic mechanisms in v0.1. The only economic transfer between agents must happen through
`Exchange`.

### Minimum trade opportunity criterion

A simple trade is possible when:

```text
A needs X
+
B has X
+
B needs Y
+
A has Y
```

Example: A produces Wheat and needs Meat; B produces Meat and needs Wheat →

```text
A → Wheat → B
A ← Meat  ← B
```

The application may look for these opportunities among agents. **The domain must not own a
`Market` object to perform this discovery.**

---

## Domain invariants

- Every Producer is also a consumer (`Producer != non-consumer`).
- Survival is binary: `Alive` or `Dead`; no partial death.
- The trade is **bilateral and voluntary**; the only economic transfer between agents is via
  `Exchange`.
- The trade is **atomic**: if it fails, no inventory is altered.
- Production **does not remove** resources from the inventory; it only adds.
- Production is **deterministic** (no uncertainty).
- The domain **does not know about the simulation**; the cycle is orchestrated by the
  application.
- There is no money, price, currency, or market in the domain.
- Need does not represent preference/utility/price/intensity/priority.

## Explicitly EXCLUDED concepts (v0.1)

Do not, under any circumstances, introduce in this version:

```text
Money        Price        Currency     Utility      Interest
Credit       Bank         Capital      Labor        Wage
Tax          Government   Market       MarketEquilibrium
SupplyCurve  DemandCurve  Technology   Entrepreneur
Expectation  Preference   Profit       Loss
```

Also do not implement yet: perfect/imperfect information, complex discovery, negotiation, multiple
bargaining rounds, prices, auctions, centralized market, variable productivity, uncertainty, or
subjective preferences. These concepts belong to future versions (see spec
`005-limited-information`).

## Economic cycle

The cycle of each period is executed by the **application layer**, not by the domain. The
mandatory order:

```text
┌─────────────────────┐
│ Period start        │
└──────────┬──────────┘
           ↓
┌─────────────────────┐
│ Production           │
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

Do not implement this sequence inside `Agent`. (See `docs/architecture/architecture.md` and
ADR-003.)