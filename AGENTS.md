# Repository Guidelines

## Scope
- This is a multi-project learning repository; there is no global build or test command.
- Treat each subfolder as an independent project with its own toolchain and dependencies.

## Top-Level Organization
- `cpp/`: kata implementations and test harnesses.
- `docker/`: Docker/Kubernetes exercises.
- `dotnet/`: ASP.NET, MAUI, WinForms, console, and prototypes.
- `javascript/`: Vue and other JS projects.
- `rust/`: Rust practice projects.
- `sql/`: SQL exercises and scripts.

## Repository Layout (where to look)
- `dotnet/asp-dotnet/`: FileTesting, FindACoach, HotelListing, MyGarage, TestWebApp (each a `.sln`)
- `dotnet/avalonia/`: Cbam, LiveChartsPrototype, MusicStoreAvaloniaExample
- `dotnet/codewars/Codewars.Training/`: 19 kata libs + NUnit tests
- `dotnet/console/`: AlgorithmTester, GitHubCopilotDemo, LibraryPlayground, PdfToolKit, Prototypes, SomeContractsNDataAsNuGet, ThreadedLogger
- `dotnet/maui/`: MauiAppTesty, Practice.Maui (needs `googleapi.json`, see its README)
- `dotnet/proof-of-concepts/`: handlebars.net, hot-chocolate-graphql
- `dotnet/win-forms/ResourceCompare/`, `dotnet/godot/` (Godot editor project, no dotnet flow)
- `javascript/`: pulsar-admin-app (Vue SPA), set/set-game (Set card game), codewars (Mocha/Chai), vue-udemy (course setups, per-folder commands)
- `rust/codewars-training/` (cargo), `cpp/codewars-training` (lib) + `cpp/codewars-training-tests` (ctest)
- `sql/simple_group_by`, `sql/sql_bug_fixing_the_join` (DBs via root `docker-compose.yml`)
- `docker/`: DockerCourse, KubernetesCourse, KubernetesCourseData, KubernetesCourseNetworking (course material)
- Not checked out: git submodules `quiz-scraper`, `ColorGenerator`, `GameOfLife` (see `.gitmodules`); `pulsardata/` is an empty unused data dir.

## README Convention
- Every area and subproject has a `README.md` with an `## About` section (purpose, stack, entrypoint, commands) on top of the auto-generated `DIRECTORY_NAVIGATION` block.
- When adding a subproject, add its `## About` README and link it from the area README and the root `README.md` Project Map. Never remove or hand-edit inside `DIRECTORY_NAVIGATION` markers.

## Command Matrix (Examples)
- JavaScript (`javascript/pulsar-admin-app`):
  - `npm ci`
  - `npm run dev`
  - `npm run test:run`
- Rust (`rust/codewars-training`):
  - `cargo test`
  - `cargo run`
- .NET (`dotnet/asp-dotnet/FileTesting/FileTesting`):
  - `dotnet build FileTesting.sln`
  - `dotnet run --project FileTesting/FileTesting`
- C++ (`cpp/codewars-training-tests`):
  - `cmake -S . -B build`
  - `cmake --build build`
  - `ctest --test-dir build --output-on-failure`
- .NET tests wherever a test project exists:
  - `dotnet test <Solution>.sln`
- JavaScript katas (`javascript/codewars`):
  - `npm install`
  - `npm test`
- SQL (`sql/simple_group_by`, `sql/sql_bug_fixing_the_join`):
  - `docker compose up` (repo root, provides MariaDB/Postgres)
  - then run the `.sql` scripts per the folder README

## Style & Naming
- Follow language-native conventions per subproject.
- Respect local formatter/linter configs (`.editorconfig`, ESLint, dotnet format, rustfmt, etc.).
- Keep filenames and test names consistent with surrounding files.

## Testing Expectations
- Run tests only for the project(s) you changed.
- Add regression tests when fixing bugs.
- Do not change generated outputs/snapshots unless intentionally updating behavior.

## Commits & PRs
- Use Conventional Commit prefixes when possible.
- Keep commits scoped to one subproject.
- PRs should include paths changed, commands run, and any setup prerequisites.

## Safety
- Never commit secrets or environment credentials.
- Ask before destructive data operations or large cross-project refactors.
