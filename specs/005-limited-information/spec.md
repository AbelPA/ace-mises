# Specification: 005-limited-information

> **Status: Prospective — NOT implemented in v0.1.**
> This spec describes the next planned evolution (v0.2+). None of the scenarios below must be
> implemented in v0.1; they are hypothetical and are explicitly marked as `[PROSPECTIVE]`.

## About

### Purpose

Anticipate the evolution of ACE to remove the perfect-information assumption of v0.1: in v0.2+, agents
**will not automatically know** the inventories and needs of others. The discovery of exchange
opportunities will require **exploration** and **exposure of offers**, emerging from a decentralized
discovery process rather than centralized matching by an omniscient application.

### Scope

- Relaxation of the complete-information assumption (only the "information" dimension; no value/price
  concept is introduced).
- Hypotheses about partial visibility, information reach, and `Offer` exposure.

### Out of Scope

- **Money, Price, Currency** — exchange remains direct barter; no currency is introduced.
- **Centralized market / order book / auctions** — discovery remains decentralized.
- **Multi-round negotiation / bargaining** — complex mechanisms belong to later versions.
- **Utility / Preference / Expectation** — v0.2+ deals only with information; it does not introduce these
  value concepts.
- Everything in `spec-001..004` remains valid; this spec does not alter the v0.1 domain.

## ADVICE

> The scenarios below are **prospective** and must **not** pass in v0.1. They are hypotheses to guide
> the design of v0.2+.

### Scenario: [PROSPECTIVE] agent does not know another's inventory by default

> GIVEN two agents A and B in v0.2+
> THEN by default A does not know B's inventory or needs
> AND A can only act on information it actually possesses.

### Scenario: [PROSPECTIVE] exposing an Offer signals interest locally

> GIVEN an agent A that wants `Meat` and has surplus `Wheat`
> WHEN A exposes an `Offer(Wheat, 1, Meat, 1)` with limited reach
> THEN only agents within A's reach can observe the offer.

### Scenario: [PROSPECTIVE] discovery emerges from exploration, not centralized matching

> GIVEN agents with partial information, exposed locally
> WHEN two reciprocally compatible offers meet within mutual reach
> THEN a voluntary bilateral exchange can occur via `Exchange`
> AND the application does not act as an omniscient central matcher.

### Scenario: [PROSPECTIVE] replicate A/B under limited information

> GIVEN the A/B configuration of v0.1 (EXP-001) under limited-information rules
> THEN multiple attempts/exposures may be required before the reciprocal exchange occurs
> AND the final result (if the exchange occurs) remains consistent with EXP-001 (both survive).

## Theoretical motivation

The Misesian emphasis on **dispersed knowledge**, **decentralized coordination**, and **entrepreneurial
discovery** suggests that economic order emerges from local interactions among agents with partial
information. v0.1 assumes complete information intentionally, in order to keep the implementation minimal
and deterministic (ADR-003).

## ADDENDA

- **Do not implement in v0.1.** v0.2+ concepts require a new architectural decision (ADR) and a dedicated
  spec before any implementation.
- Still prohibited, even in v0.2+: money, price, currency, centralized market, utility, subjective
  preferences.
- Related to `docs/experiments/EXP-002-limited-information.md`.
- References: `docs/architecture/adr/ADR-002-domain-boundary.md`,
  `docs/architecture/adr/ADR-003-deterministic-simulation.md`.