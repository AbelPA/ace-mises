# Specification: 003-barter

> Barter in ACE v0.1: voluntary bilateral exchange via direct barter, without money.
> `Offer` (proposal) and `Exchange` (atomic transfer).

## About

### Purpose

Define the only form of economic transfer between agents in v0.1: the bilateral exchange of food via
barter. There is no money, price, or `Market` in the domain; the application discovers reciprocal
exchange opportunities and executes `Exchange`.

### Scope

- `Offer` (record) — barter proposal.
- `Exchange` (class) — bilateral physical transfer.
- Atomic execution rule.
- Voluntary exchange rule (sole economic transfer).
- Minimal exchange-opportunity criterion.

### Out of Scope

- Complex discovery mechanisms, multi-round negotiation, auctions, centralized market.
- Money, Price, Currency, Utility, Preference (prohibited).
- The opportunity-discovery behavior lives in the application; the domain has no `Market`.

## ADVICE

### Scenario: execute valid exchange transfers bilaterally

> GIVEN `A` with `Wheat >= 1` and `B` with `Meat >= 1`
> WHEN `new Exchange(A, B, Wheat, 1, Meat, 1).Execute()` is called
> THEN it returns `true`, `A` loses `1 Wheat` and gains `1 Meat`, and `B` loses `1 Meat` and gains `1 Wheat`.

### Scenario: fails when buyer does not own the good

> GIVEN `A` with `Wheat = 0` and `B` with `Meat = 1`
> WHEN `Exchange(A, B, Wheat, 1, Meat, 1).Execute()` is called
> THEN it returns `false` and **no** inventory is altered.

### Scenario: fails when seller does not own the good

> GIVEN `A` with `Wheat = 1` and `B` with `Meat = 0`
> WHEN `Exchange(A, B, Wheat, 1, Meat, 1).Execute()` is called
> THEN it returns `false` and **no** inventory is altered.

### Scenario: invalid exchange does not alter any inventory

> GIVEN any failure condition (insufficient inventory on either side)
> WHEN `Exchange.Execute()` is called and returns `false`
> THEN the inventories of `Buyer` and `Seller` remain exactly as before the call.

### Scenario: Offer represents a proposal without monetary price

> GIVEN `A` and food
> WHEN `new Offer(A, Wheat, 1, Meat, 1)` is created
> THEN it represents only `Seller`, `OfferedFood`, `OfferedQuantity`, `RequestedFood`,
> `RequestedQuantity`; there is no price/currency.

### Scenario: exchange is bilateral and voluntary

> GIVEN two agents with reciprocal surpluses
> THEN the transfer occurs only via `Exchange`
> AND there are no `Give`, `Transfer`, `Donate`, `Steal` in v0.1.

## Canonical code

### Offer

```csharp
using ace.domain.agents;
using ace.domain.economy;

namespace ace.domain.exchange;

public sealed record Offer(
    Agent Seller,
    FoodType OfferedFood,
    int OfferedQuantity,
    FoodType RequestedFood,
    int RequestedQuantity);
```

### Exchange

```csharp
using ace.domain.agents;
using ace.domain.economy;

namespace ace.domain.exchange;

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

## Rules

### Execution rule (atomic)

An exchange can only occur when:

1. the first agent owns the good it will deliver;
2. the second agent owns the good it will deliver;
3. both quantities are valid;
4. both sides can perform the transfer.

```text
If A owns X
and B owns Y

A → X
B → Y

only then:

A receives Y
B receives X
```

Never allow `A loses X` and `B cannot deliver Y`. If any condition fails, `Execute() == false`
and **no inventory must be altered**.

### Voluntary exchange rule

The exchange cannot be unilateral. Do not implement `Give()`, `Transfer()`, `Donate()`, `Steal()` as
economic mechanisms of v0.1. The only economic transfer between agents must occur through `Exchange`.

### Minimal exchange-opportunity criterion

A simple exchange is possible when:

```text
A needs X
+
B owns X
+
B needs Y
+
A owns Y
```

Example (A/B):

```text
A:
produces Wheat
needs Meat

B:
produces Meat
needs Wheat

A → Wheat → B
A ← Meat  ← B
```

The application may look for these opportunities among agents. **The domain must not contain a `Market`
object to perform this discovery.**

## ADDENDA

- Depends on `spec-001-minimal-domain` (`Agent`, `Inventory`, `FoodType`).
- Opportunity discovery is performed by the application (ADR-001/ADR-003); there is no `Market` in the domain.
- The complete A/B scenario (production → exchange → consumption → survival) is validated in `spec-004-survival`
  and in the experiment `docs/experiments/EXP-001-two-agents.md`.
- Prohibited: money, price, currency, centralized market (constitution).