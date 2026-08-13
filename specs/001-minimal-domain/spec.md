# Specification: 001-minimal-domain

> Minimal domain concepts of ACE v0.1: goods, needs, production, inventory, agent types, and the `Agent`
> entity. C#/.NET stack, Clean Architecture, object-oriented.

## About

### Purpose

Implement the **minimal domain** of an Agent-Based Computational Economy (ACE) inspired by Ludwig von
Mises's theory of human action (praxeology): agents with basic food needs, some producers, physical
inventory, identification of unfulfilled needs, voluntary barter exchanges, consumption at the end of
each period, and survival/death. This spec concentrates the structural concepts; the production,
exchange, and survival behaviors live in dedicated specs.

### Scope

- `FoodType`, `Need`, `Production`, `Inventory`, `AgentType`, `Agent` (structure and invariants).
- Namespaces: `ace.domain`, `ace.domain.entities.agents`, `ace.domain.entities.economy`, `ace.domain.entities.exchange`.
- Layers: `ace.domain ← ace.application ← ace.infrastructure ← ace.console`; the domain does not know the simulation.
- Value objects as `record`; entities with state/behavior as `class`.

### Out of Scope

- `Produce()` behavior (spec `002-production`).
- `Offer` and `Exchange` and trades (spec `003-barter`).
- `UnfulfilledNeeds`, `NeedsFood`, `Consume`, survival and cycle (spec `004-survival`).
- Any concept from the prohibitions list (money, price, market, utility, ...).

## ADVICE

### Scenario: create Producer agent with needs and productions

> GIVEN a list of valid `Need` and a list of valid `Production`
> WHEN an `Agent` is created with `AgentType.Producer`
> THEN `IsAlive` is `true`, `Inventory` is empty, `Needs` and `Productions` reflect the supplied lists, and `Id` is unique.

### Scenario: create Consumer agent without productions

> GIVEN a list of valid `Need` and no productions
> WHEN an `Agent` is created with `AgentType.Consumer`
> THEN `Productions` is empty, `Needs` reflects the needs, and the agent can consume (every Consumer has needs and inventory).

### Scenario: Producer is also a consumer

> GIVEN an `Agent` of type `Producer`
> THEN the agent has needs, inventory, and can consume and participate in exchanges
> AND `Producer != non-consumer`.

### Scenario: Inventory.Add increments quantity and rejects invalid quantity

> GIVEN an empty `Inventory`
> WHEN `Add(FoodType.Wheat, 5)` is called
> THEN `Get(FoodType.Wheat)` returns `5` and `Has(Wheat, 5)` is `true`.
> GIVEN quantity `0` or negative
> WHEN `Add` is called
> THEN `ArgumentOutOfRangeException` is thrown.

### Scenario: Inventory.Has and Get query the inventory

> GIVEN an `Inventory` with `Wheat = 3`
> THEN `Get(Wheat)` returns `3`, `Has(Wheat, 2)` is `true`, and `Has(Wheat, 4)` is `false`.
> AND `Get(Meat)` (not present) returns `0`.

### Scenario: Inventory.Remove removes available and returns true

> GIVEN an `Inventory` with `Wheat = 4`
> WHEN `Remove(Wheat, 3)` is called
> THEN it returns `true` and `Get(Wheat)` becomes `1`.

### Scenario: Inventory.Remove insufficient returns false and does not alter the inventory

> GIVEN an `Inventory` with `Wheat = 2`
> WHEN `Remove(Wheat, 3)` is called
> THEN it returns `false` and `Get(Wheat)` remains `2`.
> GIVEN quantity `0` or negative
> WHEN `Remove` is called
> THEN `ArgumentOutOfRangeException` is thrown.

### Scenario: invalid Need is rejected

> GIVEN `QuantityPerPeriod <= 0`
> WHEN a `Need` is constructed
> THEN minimal validation fails (`QuantityPerPeriod` must be > 0).

### Scenario: invalid Production is rejected

> GIVEN `QuantityPerPeriod <= 0`
> WHEN a `Production` is constructed
> THEN minimal validation fails (`QuantityPerPeriod` must be > 0).

## Canonical code

### FoodType

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

### Need

```csharp
namespace ace.domain.entities.economy;

public sealed record Need(
    FoodType Food,
    int QuantityPerPeriod);
```
> Minimal validation: `QuantityPerPeriod > 0`.

### Production

```csharp
namespace ace.domain.entities.economy;

public sealed record Production(
    FoodType Food,
    int QuantityPerPeriod);
```
> Minimal validation: `QuantityPerPeriod > 0`. Deterministic.

### Inventory

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

### AgentType

```csharp
namespace ace.domain.entities.agents;

public enum AgentType
{
    Producer,
    Consumer
}
```

### Agent

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
> The `Produce()`, `UnfulfilledNeeds()`, `NeedsFood()`, `Consume()` behaviors are defined in specs
> `002-production` and `004-survival`.

## ADDENDA

- **Do not invent concepts.** Only `Agent`, `AgentType`, `FoodType`, `Inventory`, `Need`, `Production`,
  `Offer`, `Exchange` exist in the domain.
- It is forbidden to create unnecessary additional interfaces/repositories/services/factories/aggregates/events/VOs
  in the domain.
- The cycle sequence is **not** implemented inside `Agent` (spec `004-survival`).
- Continues in: spec `002-production` (production), spec `003-barter` (exchange), spec `004-survival`
  (survival and cycle).
- Model reference: `docs/domain/model.md`. Architecture: `docs/architecture/architecture.md`.