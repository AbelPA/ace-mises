# EXP-002: Limited information (prospective, v0.2+)

- **Status:** Prospective — **not implemented in v0.1**
- **Related specs:** `specs/005-limited-information/`
- **Related experiment:** `EXP-001-two-agents.md`

## Objective

Anticipate the next evolution of ACE: remove the perfect-information assumption of v0.1, so that
agents **do not automatically know** the inventories and needs of others. Discovering trade
opportunities will require exploration and signaling (exposure of `Offer`), rather than
centralized matching by an omniscient application layer.

## Theoretical motivation

The Misesian emphasis on **dispersed knowledge**, **decentralized coordination**, and
**entrepreneurial discovery** suggests that economic order emerges from local interactions among
agents with partial information — not from a planner with a global view. v0.1 deliberately
assumes complete information in order to keep the implementation minimal and deterministic; v0.2+
should relax this assumption in order to bring the model closer to the theory.

## Current state (v0.1)

In v0.1, the application layer discovers trade opportunities with **complete information** about
the inventories and needs of all agents (criterion: A needs X + B has X + B needs Y + A has Y).
Agents **do not seek** or communicate; they merely produce, identify their own unfulfilled needs,
take part in trades when the application proposes them, and consume. There is no `Market` and no
price mechanism.

## Prospective hypothesis

> Under limited information, discovering trade opportunities requires **exploration** and
> **exposure of offers** by the agents; trades emerge from a decentralized discovery process
> rather than centralized matching — while still preserving the rule of voluntary, atomic,
> bilateral trade.

## Variables to introduce (hypothetical, v0.2+)

- **Partial visibility** — an agent knows only a subset of the others (by
  neighborhood/reach).
- **Information reach** — radius/topology that determines whom an agent can "observe".
- **`Offer` propagation/exposure** — agents announce offers (`Offer`) that are only locally
  visible; trades occur when two reciprocally compatible offers meet.

> Important: these variables **do not add** money/price/centralized market. They remain out of
> scope even in v0.2. Money and the price mechanism will only be considered in later versions and
> via a new architectural decision (ADR).

## Out of scope even in v0.2

- **Money / Price / Currency** — trade remains direct barter.
- **Centralized market** — discovery remains decentralized; there is no central order book.
- **Multi-round negotiation / bargaining / auctions** — complex mechanisms belong to later
  versions.
- **Utility / subjective preferences / expectations** — v0.2+ addresses only information; it
  does not introduce these value concepts.

## Next steps

- Detail the prospective spec `specs/005-limited-information/` into formal (Given/When/Then)
  scenarios.
- Evaluate new ADRs for the local discovery and `Offer` propagation mechanisms.
- Replicate the A/B scenario (EXP-001) under limited information and analyze how many
  attempts/exposures are required before the reciprocal trade occurs.

## Status

**Prospective / Not implemented.** Any implementation belongs to later versions; none of this
should be introduced in v0.1. See `docs/architecture/adr/ADR-002-domain-boundary.md` (excluded
concepts) and `docs/architecture/adr/ADR-003-deterministic-simulation.md` (future evolution).