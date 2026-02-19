# AGENTS.md

## Purpose
This repository is a multi-language playground for code spikes, katas, and proof-of-concepts. There is no single build or test system for the whole repo. Work within the specific project directory you are touching.

## How To Navigate
- Start at the top-level `README.md` for the overall intent and directory map.
- Each project or course folder typically has its own `README.md` with build/run steps.
- Dependencies are project-scoped (for example `package.json` for JS, `.csproj` for .NET, `Cargo.toml` for Rust).

## High-Level Layout
- `cpp/`: C++ katas and tests.
- `docker/`: Docker and Kubernetes course materials and exercises.
- `dotnet/`: .NET projects (ASP.NET, MAUI, WinForms, console apps, proof-of-concepts).
- `javascript/`: JavaScript projects (Codewars, Vue course work, Pulsar admin app).
- `rust/`: Rust katas and practice.
- `sql/`: SQL exercises.

## Working Conventions
- Treat each subproject as standalone.
- Use the closest `README.md` in that subfolder for commands and expectations.
- When uncertain, read the local docs before making assumptions.

## Notes
- This is a learning repository, not production code.
