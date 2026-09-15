# HotChocolatePoC.E2ETests

End-to-end tests for the HotChocolatePoC GraphQL API (xunit.v3, Microsoft Testing
Platform runner): real HTTP + real WebSocket against the booted app, SQLite
in-memory instead of MySQL, one isolated factory (server + database) per test.

Covered: paged/filtered/sorted/projected queries, single-object query, typed
`KEY_NOT_FOUND` errors, `addArticle` (server-generated id, persistence re-read),
`updateArticle` error path, `[Authorize]` rejection + authed DataLoader batching,
`bookAdded` subscription over `graphql-transport-ws`, schema/operation contract.

Run from `HotChocolatePoC/` (its `global.json` switches `dotnet test` to MTP mode):

```bash
dotnet test --solution HotChocolatePoC.sln
```
