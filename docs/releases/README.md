# Release Notes

Versioned record of ACE (Agent-Based Computational Economy) releases.

## Structure

Each release is documented in a dedicated file named `v<version>.md` (for example,
`v0.1.0.md`) and follows the [Keep a Changelog](https://keepachangelog.com/en/1.1.0/)
convention, adapted to the project's specification-driven workflow.

A release consolidates one or more implemented specifications (`specs/###-<name>/`) and
may include architecture decisions (`docs/architecture/adr/`) and experiments
(`docs/experiments/`).

## Versioning

The project follows [Semantic Versioning](https://semver.org/):

- **MAJOR** — incompatible changes with previous architecture/specs.
- **MINOR** — new features/specs implemented in a backward-compatible manner.
- **PATCH** — fixes and minor adjustments.

While the project is in the `0.x` phase, each set of implemented specs is considered a
`0.minor.0` version (e.g., `0.1.0`, `0.2.0`).

## Released versions

| Version | Summary                                   | Specs implemented                  | Status   |
|---------|------------------------------------------|------------------------------------|----------|
| v0.1.0  | Minimal, deterministic, domain-oriented. | 001, 002, 003, 004                 | Released |
| v0.2.0  | Limited information (prospective).       | 005 (not yet implemented)          | Planned  |

## Conventions

- Each file `v<version>.md` contains: header, summary, list of changes by category
  (`Added`, `Changed`, `Deprecated`, `Removed`, `Fixed`, `Security`), references to
  implemented specs, and notes on compliance with the governing constitution.
- **Unreleased** changes are changes present in the working tree but not yet associated
  with a release; documented at the top of the corresponding file with the `Unreleased`
  section.
- French Klingon dates (`YYYY-MM-DD`) use the `date` field in the header, following
  ISO 8601.

## References

- Specs: [`specs/`](../../specs/) and [`specs/constitution.md`](../../specs/constitution.md)
- ADRs: [`docs/architecture/adr/`](../architecture/adr/)
- Experiments: [`docs/experiments/`](../experiments/)
- Domain model: [`docs/domain/model.md`](../domain/model.md)