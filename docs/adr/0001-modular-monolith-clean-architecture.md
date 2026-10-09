# 0001. Modular monolith with Clean Architecture

- Status: Accepted
- Date: 2026-10-09

## Context

The product has 20+ business modules, a small team, and must be sold as multi-tenant SaaS.
Microservices would multiply deployment, data consistency and operations work before there is a product.
Without firm boundaries, a monolith decays into a tangle where any class calls any other.

## Decision

One deployable API, split into layers with a one-way dependency rule:

```
Api ──> Application ──> Domain
 │          │
 │          └──> Contracts
 ├──> Infrastructure ──> Application
 └──> Persistence    ──> Application
```

- Domain and Contracts reference nothing.
- Application defines ports (interfaces such as `IClock`, `ITenantContext`, repositories);
  Infrastructure and Persistence implement them.
- Api is the composition root and the HTTP surface only.
- Modules (Sales, Inventory, Purchasing, ...) are folders inside each layer, plus a SQL schema each.
  A module talks to another module only through its Application interfaces, never its tables.
- `ERP.Reporting` and `ERP.BackgroundJobs` from the proposal are created when the first report engine and
  first background job are built, not before.

## Alternatives considered

- Microservices from day one: too much operational cost for the team size; boundaries are not yet known.
- Project per module: clearer isolation, but 60+ projects and slow builds. Revisit if a module is extracted.

## Consequences

- Architecture tests enforce the rule on every build; a violation fails CI.
- Extracting a module later is possible because modules share no tables and talk through interfaces.
