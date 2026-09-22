# tower-defense

## About

Sci-Fi 2D maze tower defense prototype (GDScript, Godot 4.7.2). Free maze
building with AStarGrid2D pathfinding, money-is-HP economy (game over below
zero), endless deterministic waves, meta skill-tree stub, local JSONL
telemetry for data-driven balancing.

- Stack: Godot 4.7.2, GDScript (no .NET flow)
- Entrypoint: `scenes/Main.tscn` (open/import the folder in the Godot editor)
- Commands (console binary via `%GODOT_BIN%`):
  - Import: `godot --headless --path . --import --quit`
  - Run: `godot --path .`
  - Tests: `godot --headless --path . -s res://addons/gut/gut_cmdln.gd -gdir=res://tests -ginclude_subdirs -gexit`
  - GUT addon is vendored under `addons/gut/` (v9.6.1, MIT)

## Status

T01/T02 scaffold: variable grid, click-to-build gun maze (left builds,
right sells), live path display. Combat, wave loop, telemetry analysis and
juice follow as T03–T06. Vision: see repo docs / task thread.
