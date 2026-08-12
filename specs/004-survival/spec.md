# Specification: 004-survival

> Unfulfilled needs, consumption, binary survival, and the economic cycle in ACE v0.1.
> Includes the **mandatory minimal A/B scenario** as the acceptance criterion.

## About

### Purpose

Define how an agent identifies unfulfilled needs, consumes food at the end of the period, and survives
or dies. Also define the mandatory sequence of the **economic cycle**, orchestrated by the application
layer (not by `Agent`). Finally, fix the A/B scenario that validates the entire v0.1.

### Scope

- `Agent.UnfulfilledNeeds()`, `Agent.NeedsFood()`, `Agent.Consume()`.
- Binary survival (`Alive`/`Dead`).
- Economic cycle (mandatory order) and separation of responsibilities.
- Minimal A/B scenario.

### Out of Scope

- Structure of `Agent`/`Inventory`/`Need`/`Production` (spec `001-minimal-domain`).
- `Produce()` (spec `002-production`).
- `Offer`/`Exchange` (spec `003-barter`).
- Health, aging, partial hunger, damage, recovery, reproduction, partial death (prohibited).

## ADVICE

### Scenario: identify fulfilled need

> GIVEN an `Agent` with `Need(Meat, 1)` and `Inventory` with `Meat = 1`
> WHEN `UnfulfilledNeeds()` is queried
> THEN that need does **not** appear among the unfulfilled, and `NeedsFood()` returns `false`.

### Scenario: identify unfulfilled need

> GIVEN an `Agent` with `Need(Meat, 1)` and `Inventory` with `Meat = 0`
> WHEN `UnfulfilledNeeds()` is queried
> THEN it returns `Need(Meat, 1)` as unfulfilled, and `NeedsFood()` returns `true`.

### Scenario: multiple needs partially covered

> GIVEN an `Agent` with `Need(Meat, 2)` and `Inventory` with `Meat = 1`
> THEN `UnfulfilledNeeds()` reports the unfulfilled need (1 unit missing) and `NeedsFood()` is `true`.

### Scenario: consuming with all needs fulfilled keeps the agent alive

> GIVEN a live `Agent` with `Need(Meat, 1)` and `Inventory` with `Meat = 1`
> WHEN `Consume()` is called
> THEN `Meat` is removed from the inventory and `IsAlive` remains `true`.

### Scenario: consuming with any unfulfilled need kills the agent

> GIVEN a live `Agent` with `Need(Meat, 1)` and `Inventory` with `Meat = 0`
> WHEN `Consume()` is called
> THEN `IsAlive` becomes `false`.

### Scenario: dead agent is ignored in Consume

> GIVEN an `Agent` with `IsAlive == false`
> WHEN `Consume()` is called
> THEN no change occurs (the dead are ignored) and `IsAlive` remains `false`.

### Scenario: survival is binary

> GIVEN any consumption state
> THEN the result is only `Alive` or `Dead`; there is no partial death, health, hunger, damage, or recovery.

### Scenario: complete A/B economic cycle (mandatory minimal scenario)

> GIVEN `A` Producer with `Production(Wheat, 5)` and `Need(Meat, 1)` and `B` Producer with `Production(Meat, 2)` and `Need(Wheat, 1)`
> WHEN the application executes the cycle (Production → Needs → Trades → Consumption → Survival)
> THEN after production: A has `Wheat=5, Meat=0` and B has `Wheat=0, Meat=2`
> AND after the exchange (A → 1 Wheat, B → 1 Meat): A has `Wheat=4, Meat=1` and B has `Wheat=1, Meat=1`
> AND after consumption: A has `Wheat=4, Meat=0` and B has `Wheat=0, Meat=1`
> AND `A.IsAlive == true` and `B.IsAlive == true`.

## Economic cycle

The cycle of each period must be executed by the **application layer**, not by the domain.
(Production → Needs check → Exchange attempts → Consumption → Survival → next period). Mandatory order:

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
│ Needs                │
└──────────┬──────────┘
           ↓
┌─────────────────────┐
│ Trades               │
└──────────┬──────────┘
           ↓
┌─────────────────────┐
│ Consumption          │
└──────────┬──────────┘
           ↓
┌─────────────────────┐
│ Survival             │
└──────────┬──────────┘
           ↓
       Next period
```

**Do not implement this sequence inside `Agent`.**

## Behaviors

### `UnfulfilledNeeds()`

Returns the needs whose required quantity is **not** available in the inventory.

```text
Need: Meat = 1
Inventory: Meat = 0
Result: Meat = 1 unfulfilled
```

If `Inventory: Meat = 1`, the need is fulfilled.

### `NeedsFood()`

Returns `true` when at least one unfulfilled need exists.

### `Consume()`

1. Ignore dead agents.
2. Check each need.
3. Remove the required quantity from the inventory.
4. If any need cannot be fulfilled, set `IsAlive = false`.

Death occurs when the agent cannot fully satisfy its needs.
Survival is binary: `Alive` / `Dead`.

## Mandatory minimal scenario (A/B)

| Agent | Type     | Production | Needs        |
|-------|----------|------------|--------------|
| A     | Producer | Wheat = 5  | Meat = 1     |
| B     | Producer | Meat = 2   | Wheat = 1    |

| Step                  | A (Wheat/Meat) | B (Wheat/Meat) |
|-----------------------|----------------|----------------|
| After production      | 5 / 0          | 0 / 2          |
| After exchange        | 4 / 1          | 1 / 1          |
| After consumption     | 4 / 0          | 0 / 1          |

Final result: `A.IsAlive == true` and `B.IsAlive == true`.

## Separation of responsibilities

- **Domain** — represent agents, needs, production, inventory, offers; execute exchanges; apply survival
  rules.
- **Application** — control periods, iterate agents, look for exchange opportunities, order interactions,
  run the simulation, collect results.
- **Infrastructure** — persistence, storage, logging, external configuration.
- **Console** — only start the simulation, display results, user I/O.

## ADDENDA

- Depends on `spec-001`, `spec-002`, and `spec-003`.
- The cycle is orchestrated by the application (ADR-001/ADR-003); **not** inside `Agent`.
- The A/B scenario is the minimal acceptance criterion of v0.1 (joint validation of the four specs).
- Corresponding experiment: `docs/experiments/EXP-001-two-agents.md`.
- Absolute constraints (constitution): no health/age/hunger/damage/recovery/reproduction/partial death.