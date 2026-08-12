# Specification: 002-production

> Agent production behavior in ACE v0.1: `Agent.Produce()`.

## About

### Purpose

Define how an agent adds food to its inventory from its `Production` capabilities, deterministically.
Production occurs in the **Production** phase of the economic cycle, orchestrated by the application
layer.

### Scope

- The `Agent.Produce()` method.
- Semantics: production adds to the inventory; it does not remove resources; it does not model costs.

### Out of Scope

- Discovery of exchange opportunities (spec `003-barter`).
- Consumption and survival (spec `004-survival`).
- Uncertainty, variable productivity, technology, production costs, factors of production (prohibited).

## ADVICE

### Scenario: live Producer produces and the inventory increases

> GIVEN a Producer `Agent` with `Production(Wheat, 5)` and `IsAlive == true`
> WHEN `Produce()` is called
> THEN `Inventory.Get(Wheat)` is `5`.

### Scenario: multiple productions accumulate in the inventory

> GIVEN a Producer `Agent` with `Production(Wheat, 5)` and `Production(Corn, 2)`
> WHEN `Produce()` is called
> THEN `Inventory.Get(Wheat)` is `5` and `Inventory.Get(Corn)` is `2`.

### Scenario: repeated production in subsequent periods accumulates

> GIVEN a Producer `Agent` with `Production(Wheat, 5)` having already produced once (`Wheat = 5`)
> WHEN `Produce()` is called again (new period)
> THEN `Inventory.Get(Wheat)` is `10`.

### Scenario: dead agent does not produce

> GIVEN an `Agent` with `IsAlive == false` and `Production(Wheat, 5)`
> WHEN `Produce()` is called
> THEN the inventory remains unchanged (nothing is produced).

### Scenario: production does not remove resources from the inventory

> GIVEN a Producer `Agent` with `Inventory` containing `Wheat = 3` and `Production(Wheat, 5)`
> WHEN `Produce()` is called
> THEN `Inventory.Get(Wheat)` is `8` (3 + 5); production only adds, never removes.

## Formal rule

```text
Production
      ↓
Inventory
```

`Produce()` must:

1. Check whether the agent is alive; if dead, do nothing.
2. Iterate over all its productions.
3. Add the produced quantity to the inventory.

Production **does not remove** resources from the inventory. Do not model production costs.

## ADDENDA

- Depends on `spec-001-minimal-domain` (`Agent`, `Production`, `Inventory`).
- Production is deterministic (ADR-003); no uncertainty/variable productivity.
- `Produce()` is called by the application in the Production phase of the cycle (spec `004-survival`).
- Architecture reference: ADR-003.