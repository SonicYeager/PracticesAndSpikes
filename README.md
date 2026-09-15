# Code Spike and Practice Repository

This repository serves as a dynamic space for code spikes, practice exercises, and prototyping various software development concepts. It's a personal playground for learning, experimenting with new technologies, and honing existing skills across a wide range of programming languages and platforms.

## About This Repository

As a "spike" repository, the code here is not intended for production use. Instead, it's a living collection of small projects, code katas, and proof-of-concepts. The primary goal is to facilitate rapid learning and experimentation in a structured and organized manner.

## Getting Started

To get started with the projects in this repository, you'll need to have the necessary development environment set up for the specific language or technology you're interested in.

### Prerequisites

- **Git:** To clone the repository.
- **Language-Specific Runtimes and SDKs:** Depending on the project, you might need Node.js, .NET SDK, Rust, a C++ compiler, etc.
- **Docker:** Some projects might use Docker for containerization.

### Installation

1. **Clone the repository:**
   ```bash
   git clone https://github.com/your-username/your-repository.git
   ```
2. **Navigate to a project directory:**
   ```bash
   cd path/to/project
   ```
3. **Follow the project-specific instructions:**
   Each project directory contains a `README.md` file with detailed instructions on how to build, run, and test the code.

## Usage

Each top-level directory in this repository represents a different programming language or technology. Within each of these directories, you'll find individual projects, each with its own `README.md` file that provides specific details about that project.

## Categories

- **[C++](./cpp/README.md):** A collection of C++ projects, including solutions to Codewars katas, focusing on modern C++ features and best practices.
- **[Docker](./docker/README.md):** Various Docker configurations and projects, demonstrating how to use Docker for containerizing applications.
- **[.NET](./dotnet/README.md):** A wide range of .NET projects, including console applications, ASP.NET Core web APIs, and more.
- **[JavaScript](./javascript/README.md):** A diverse set of JavaScript projects, including frontend applications with Vue.js, solutions to Codewars katas, and other experiments.
- **[Rust](./rust/README.md):** A collection of Rust projects, including solutions to Codewars katas, exploring Rust's unique features for systems programming.
- **[SQL](./sql/README.md):** A series of SQL scripts and projects, covering various aspects of database design and querying.

## Project Map

Every subproject documents itself in its own README (stack, entrypoint, commands).
Start from the area README, then drill into the project README.

- [cpp/](cpp/) ([area README](cpp/README.md)) — C++23 katas ([codewars-training](cpp/codewars-training/)) + [GTest harness](cpp/codewars-training-tests/)
- [docker/](docker/) ([area README](docker/README.md)) — course material: [DockerCourse](docker/DockerCourse/), [KubernetesCourse](docker/KubernetesCourse/), [KubernetesCourseData](docker/KubernetesCourseData/), [KubernetesCourseNetworking](docker/KubernetesCourseNetworking/)
- [dotnet/](dotnet/) ([area README](dotnet/README.md))
  - [asp-dotnet/](dotnet/asp-dotnet/) — [FileTesting](dotnet/asp-dotnet/FileTesting/), [FindACoach](dotnet/asp-dotnet/FindACoach/), [HotelListing](dotnet/asp-dotnet/HotelListing/), [MyGarage](dotnet/asp-dotnet/MyGarage/), [TestWebApp](dotnet/asp-dotnet/TestWebApp/)
  - [avalonia/](dotnet/avalonia/) — [Cbam](dotnet/avalonia/Cbam/), [LiveChartsPrototype](dotnet/avalonia/LiveChartsPrototype/), [MusicStoreAvaloniaExample](dotnet/avalonia/MusicStoreAvaloniaExample/)
  - [codewars/](dotnet/codewars/) — [Codewars.Training](dotnet/codewars/Codewars.Training/) (19 katas + NUnit)
  - [console/](dotnet/console/) — [AlgorithmTester](dotnet/console/AlgorithmTester/), [GitHubCopilotDemo](dotnet/console/GitHubCopilotDemo/), [LibraryPlayground](dotnet/console/LibraryPlayground/), [PdfToolKit](dotnet/console/PdfToolKit/), [Prototypes](dotnet/console/Prototypes/), [SomeContractsNDataAsNuGet](dotnet/console/SomeContractsNDataAsNuGet/), [ThreadedLogger](dotnet/console/ThreadedLogger/)
  - [godot/](dotnet/godot/) — Squash the Creeps (Godot 4 C# tutorial)
  - [maui/](dotnet/maui/) — [MauiAppTesty](dotnet/maui/MauiAppTesty/), [Practice.Maui](dotnet/maui/Practice.Maui/) (needs `googleapi.json`, see its README)
  - [proof-of-concepts/](dotnet/proof-of-concepts/) — [handlebars.net](dotnet/proof-of-concepts/handlebars.net/), [hot-chocolate-graphql](dotnet/proof-of-concepts/hot-chocolate-graphql/)
  - [win-forms/](dotnet/win-forms/) — [ResourceCompare](dotnet/win-forms/ResourceCompare/)
- [javascript/](javascript/) ([area README](javascript/README.md)) — [pulsar-admin-app](javascript/pulsar-admin-app/) (Pulsar admin SPA), [set/set-game](javascript/set/set-game/) (Set card game), [codewars](javascript/codewars/) (JS katas), [vue-udemy](javascript/vue-udemy/) (course setups)
- [rust/](rust/) ([area README](rust/README.md)) — [codewars-training](rust/codewars-training/) (Rust katas)
- [sql/](sql/) ([area README](sql/README.md)) — [simple_group_by](sql/simple_group_by/), [sql_bug_fixing_the_join](sql/sql_bug_fixing_the_join/); databases via root `docker-compose.yml`

## Notes

- Git submodules (`quiz-scraper`, `ColorGenerator`, `GameOfLife`) are registered in `.gitmodules` but not checked out — run `git submodule update --init` to fetch them.
- `pulsardata/` is an empty, currently unused data directory (the pulsar volume in the root `docker-compose.yml` is commented out).
- Root `docker-compose.yml` provides shared services: PostgreSQL (`local_postgres`, 5432), MariaDB (`local_db`, 3306) and Pulsar standalone (`local_pulsar`, 8080/6650).

## Contributing

Contributions are welcome! If you have any suggestions, improvements, or new projects to add, please feel free to open an issue or submit a pull request.

## License

This repository is licensed under the MIT License. See the [LICENSE](./LICENSE) file for more details.

<!-- DIRECTORY_NAVIGATION:START -->
## Directory Navigation

- Directory: `/` (repository root)
- Parent: _None_
- Children: [.vscode](.vscode/) | [README](.vscode/README.md), [cpp](cpp/) | [README](cpp/README.md), [docker](docker/) | [README](docker/README.md), [dotnet](dotnet/) | [README](dotnet/README.md), [javascript](javascript/) | [README](javascript/README.md), [rust](rust/) | [README](rust/README.md), [sql](sql/) | [README](sql/README.md)
<!-- DIRECTORY_NAVIGATION:END -->
