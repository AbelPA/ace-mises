# EXP-001: Two agents, bilateral barter

- **Status:** Implementable in v0.1
- **Related specs:** `specs/001-minimal-domain/`, `specs/002-production/`, `specs/003-barter/`,
  `specs/004-survival/`
- **Reference model:** `docs/domain/model.md`

## Objective

Demonstrate the central proposition of ACE v0.1: agents with survival needs, some produce foods,
and when one agent has a surplus that another needs — and a reciprocal need exists — a voluntary
bilateral trade can occur, allowing both to survive the period.

## Hypothesis

> When two producers have reciprocal needs and complementary surpluses, a voluntary bilateral
> trade (A delivers 1 Wheat, B delivers 1 Meat) allows each to satisfy its need and both remain
> alive at the end of the period.

## Setup

| Agent | Type     | Production (per period) | Needs (per period) |
|-------|----------|-------------------------|--------------------|
| A     | Producer | Wheat = 5               | Meat = 1           |
| B     | Producer | Meat = 2                | Wheat = 1          |

Initial inventories empty. Both start with `IsAlive = true`.

## Procedure

The cycle is orchestrated by the application layer (not by `Agent`).

1. **Production** — `A.Produce()` adds 5 Wheat to A's inventory; `B.Produce()` adds 2 Meat to
   B's inventory.
2. **Needs check** —
   - A needs Meat = 1 and has Meat = 0 → unfulfilled need.
   - B needs Wheat = 1 and has Wheat = 0 → unfulfilled need.
3. **Trade attempt** — a reciprocal opportunity exists: A needs Meat (B has it), B needs Wheat
   (A has it). The application discovers the opportunity and executes `Exchange` with:
   `Buyer = A`, `Seller = B`, `FoodFromBuyer = Wheat`, `QuantityFromBuyer = 1`,
   `FoodFromSeller = Meat`, `QuantityFromSeller = 1`. `Execute()` validates both inventories and
   performs the atomic bilateral transfer (A removes 1 Wheat, B removes 1 Meat; then A receives
   1 Meat, B receives 1 Wheat).
4. **Consumption** — `A.Consume()` removes Meat = 1 from A's inventory; `B.Consume()` removes
   Wheat = 1 from B's inventory. Both needs are satisfied.
5. **Survival** — both needs were satisfied; both remain `IsAlive = true`.

## Expected results

### After production

| Agent | Wheat | Meat |
|-------|-------|------|
| A     | 5     | 0    |
| B     | 0     | 2    |

### After trade (A → 1 Wheat, B → 1 Meat)

| Agent | Wheat | Meat |
|-------|-------|------|
| A     | 4     | 1    |
| B     | 1     | 1    |

### After consumption

| Agent | Wheat | Meat |
|-------|-------|------|
| A     | 4     | 0    |
| B     | 0     | 1    |

### Final state

| Agent | IsAlive |
|-------|---------|
| A     | true    |
| B     | true    |

## Derived acceptance criteria

- The code compiles and respects the defined namespaces (`ace.domain`, `.agents`, `.economy`,
  `.exchange`).
- It uses only the defined concepts (`Agent`, `AgentType`, `FoodType`, `Inventory`, `Need`,
  `Production`, `Offer`, `Exchange`).
- It does not introduce money/price nor `Market`.
- Producers also consume (A and B are Producers and satisfy needs).
- The trade is bilateral and atomic; no inventory is altered if the trade fails.
- The **application** discovers the trade opportunity; the domain has no `Market`.
- The simulation (the cycle) lives outside `ace.domain`.
- The A/B scenario produces exactly the final inventories above with both alive.

## Notes

- Determinism: the same configuration must always produce the same result (see
  `docs/architecture/adr/ADR-003-deterministic-simulation.md`).
- This experiment simultaneously validates specs `001-minimal-domain`, `002-production`,
  `003-barter`, and `004-survival`, and is the minimum acceptance criterion for v0.1.
- v0.1 assumes complete information: the application knows the agents' inventories and needs in
  order to discover the opportunity. The limitation of this assumption is addressed in `EXP-002`
  and in the prospective spec `005-limited-information`.