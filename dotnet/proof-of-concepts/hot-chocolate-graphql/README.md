# Hot Chocolate GraphQL

A sample on how to use GraphQL and the potential benefits and or drawbacks.

# Possible UseCases
- Very Simple and Selective Get/Update/Add -> JsonPatch alternative? (altough not simply REST conform?)
- live updating communication to webapps -> progress tracking and such

# HotChocolate Features used
There are more then used and tried in this PoC. The selected ones are most likly to be used by us.
Covered by e2e tests: paging, filtering, sorting, projections, single-object query,
mutations (conventions payloads), typed domain errors (error filter), field auth
(`[Authorize]` incl. rejection path), DataLoader batching, WebSocket subscriptions
(`graphql-transport-ws`), schema contract (incl. payload shapes).

# Stack (upgraded 2026-09, verified via NuGet + ChilliCream docs)
- .NET 10, HotChocolate 15.1.18, EF Core 9 + Pomelo MySQL 9, AutoMapper 15, xunit.v3 (MTP runner)
- Deliberately NOT latest: HC 16 (re-architecture, migration guide still WIP; revisit separately),
  EF 10 (no Pomelo build yet — Pomelo 9 caps EF at 9; revisit when Pomelo 10 ships)

# Run & test
- Real DB: `docker compose up` at repo root (MariaDB `local_db`, 3306), then
  `dotnet run --project HotChocolatePoC/HotChocolatePoC` (Banana Cake Pop: `/graphql`).
  First run needs `dotnet ef database update` (migrations from 2022) — model changed
  since (timestamps are app-set now), so generate a migration first if the DB is fresh.
- E2E (no external DB — SQLite in-memory): `dotnet test --solution HotChocolatePoC.sln`
  from `HotChocolatePoC/` (`global.json` there switches `dotnet test` to MTP mode;
  scoped to this folder so VSTest suites elsewhere are unaffected).

## Project status
Done (PoC + 11 e2e tests green); follow-ups: Pomelo 10 → EF 10, HC 16 evaluation.

<!-- DIRECTORY_NAVIGATION:START -->
## Directory Navigation

- Directory: `dotnet/proof-of-concepts/hot-chocolate-graphql`
- Parent: [`..`](..) | [Parent README](../README.md)
- Children: [HotChocolatePoC](HotChocolatePoC/) | [README](HotChocolatePoC/README.md)
<!-- DIRECTORY_NAVIGATION:END -->
