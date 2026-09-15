# HotChocolatePoC

Auto-generated directory index for `dotnet/proof-of-concepts/hot-chocolate-graphql/HotChocolatePoC/HotChocolatePoC`.

## About

The GraphQL API itself (ASP.NET Core + HotChocolate 15): article queries
(paging/filtering/sorting/projections, DataLoader batching behind `[Authorize]`),
mutations (conventions payloads, typed `KEY_NOT_FOUND` errors via error filter)
and `bookAdded` WebSocket subscriptions. Needs MariaDB (see parent README);
e2e-covered via `../HotChocolatePoC.E2ETests`.

- Stack: net10.0, HotChocolate 15.1.18, AutoMapper 15, EF Core 9 (Pomelo MySQL)
- Entrypoint: `Program.cs` (`/graphql`, Banana Cake Pop in Development)
- Commands: `dotnet run --project HotChocolatePoC.csproj`

<!-- DIRECTORY_NAVIGATION:START -->
## Directory Navigation

- Directory: `dotnet/proof-of-concepts/hot-chocolate-graphql/HotChocolatePoC/HotChocolatePoC`
- Parent: [`..`](..) | [Parent README](../README.md)
- Children: [AutoMapperConfig](AutoMapperConfig/) | [README](AutoMapperConfig/README.md), [DataLoaders](DataLoaders/) | [README](DataLoaders/README.md), [MutationTypes](MutationTypes/) | [README](MutationTypes/README.md), [Properties](Properties/) | [README](Properties/README.md), [QueryTypes](QueryTypes/) | [README](QueryTypes/README.md), [Subscriptions](Subscriptions/) | [README](Subscriptions/README.md), [TypeExtensions](TypeExtensions/) | [README](TypeExtensions/README.md), [Types](Types/) | [README](Types/README.md)
<!-- DIRECTORY_NAVIGATION:END -->
