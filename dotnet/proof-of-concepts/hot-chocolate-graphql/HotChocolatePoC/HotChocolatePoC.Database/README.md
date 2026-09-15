# HotChocolatePoC.Database

Auto-generated directory index for `dotnet/proof-of-concepts/hot-chocolate-graphql/HotChocolatePoC/HotChocolatePoC.Database`.

## About

EF Core data layer: article catalog model (`ArticlesDbContext` + entities) with
MySQL/MariaDB migrations (2022, predates the timestamp handling change — generate
a fresh migration before `database update` on a new DB). Timestamps are app-set
(`UtcNow` on write), not DB-generated.

- Stack: EF Core 9, Pomelo MySQL 9
- Note: e2e tests bypass MySQL via SQLite (`EnsureCreated`, no migrations)

<!-- DIRECTORY_NAVIGATION:START -->
## Directory Navigation

- Directory: `dotnet/proof-of-concepts/hot-chocolate-graphql/HotChocolatePoC/HotChocolatePoC.Database`
- Parent: [`..`](..) | [Parent README](../README.md)
- Children: [Context](Context/) | [README](Context/README.md), [Entities](Entities/) | [README](Entities/README.md), [Migrations](Migrations/) | [README](Migrations/README.md)
<!-- DIRECTORY_NAVIGATION:END -->
