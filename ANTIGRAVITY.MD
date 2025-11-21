# ANTIGRAVITY.MD

## Agent Context

This file serves as a scratchpad and context store for the Antigravity agent. It contains architectural insights, reminders, and relevant information for navigating and understanding the codebase.

## Repository Insights

- **Nature of Repository**: This is a multi-language "monorepo" style collection of independent projects. There is no single build system for the entire repository.
- **Navigation**: When working on a task, identify the specific language and project directory first.
- **Dependencies**: Dependencies are managed at the project level (e.g., `package.json` for JS, `.csproj` for .NET, `Cargo.toml` for Rust).

## Key Directories

- **`cpp/`**: C++ Katas and tests.
- **`dotnet/`**: Extensive collection of .NET projects including MAUI, Avalonia, and ASP.NET.
- **`javascript/`**: Vue.js focus with some general JS practice.
- **`rust/`**: Rust learning exercises.
- **`docker/`**: Containerization and orchestration learning.
- **`sql/`**: Database query practice.

## Workflow Tips

- **Context Switching**: Be aware of the context switch when moving between languages.
- **Build/Run**: Always check the local `README.md` or standard language conventions for building and running projects.
