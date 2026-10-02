# Architecture

Prototype architecture: one scene, pure-logic core classes, a thin scene
controller for presentation. The split exists so gameplay rules are
unit-testable without rendering (see *Testing*).

## Layers

```
scenes/Main.tscn
  └─ Main (Node2D, scripts/game.gd)      ← scene controller
       ├─ floor/marker/tower/drone/... sprites (created at runtime)
       └─ Hud (CanvasLayer): icon panel, game-over screen (dim + summary)

scripts/*.gd (class_name RefCounted)     ← core logic, no scene access
  Maze · Pathfinder · Economy · WaveGen · Drone · Gun · Projectile
  Telemetry · SkillStub
```

Core classes never reference nodes, `origin`, `TILE` or textures. The scene
controller owns every sprite, tween, audio player and HUD label.

## Core classes

| Class | File | Responsibility |
|---|---|---|
| `Maze` | `scripts/maze.gd` | Buildable grid (`built: cell → tower_type`), spawn/base reservation, `can_build()` via hypothetical block + connectivity check, `build()`, `sell()` |
| `Pathfinder` | `scripts/pathfinder.gd` | `AStarGrid2D` wrapper; `DIAGONAL_MODE_NEVER`, solid points, `find_path()` returns cell ids, `has_path()` |
| `Economy` | `scripts/economy.gd` | Money-as-health: `on_kill()`, `on_leak()`, `spend()`, `is_game_over()` (strictly `< 0`) |
| `WaveGen` | `scripts/wave.gd` | Static `composition(n, seed)` → `{wave, count, hp, speed, tanks, fast}` |
| `Drone` | `scripts/drone.gd` | Grid-space walker: `advance(dt)` (returns `true` on leak), `take_damage()`, `reroute()`, `facing()`, `distance_to_base()`, `KIND_MODS` |
| `Gun` | `scripts/gun.gd` | Range/cadence/targeting; `acquire()` picks the drone closest to base, `try_fire(dt, targets)` returns the target when a shot is due |
| `Projectile` | `scripts/projectile.gd` | Homing tracer; `advance(dt)` returns `true` on hit, fizzles when the target dies or leaks |
| `Telemetry` | `scripts/telemetry.gd` | Buffers JSON events, `flush(path)` writes JSONL |
| `SkillStub` | `scripts/skill_stub.gd` | Meta stub: one bonus persisted via `ConfigFile` |

## Coordinates

Core logic uses **grid space**: 1 unit = 1 cell, cell center is
`Vector2(cell) + Vector2(0.5, 0.5)`, speed is cells/second. The scene converts
once at the boundary:

```gdscript
world = origin + grid_pos * TILE        # _grid_to_world()
cell  = Vector2i((world - origin) / TILE).floor()
```

`WaveGen` speeds are px/s (author-friendly); `game.gd` divides by `TILE`
before handing them to `Drone`. `Pathfinder` works in integer cells and is
independent of `origin`/`TILE` (its `cell_size` is cosmetic for path length
only).

## Frame order (`game.gd _process`)

1. `_update_spawner` — runs the `BREAK_SECONDS` countdown (auto-starts the
   next wave), pops one drone from the queue every `SPAWN_INTERVAL`
2. `_update_drones` — advance along the path, handle leaks
3. `_update_guns` — acquire target, aim barrel, fire (muzzle tracer + flash)
4. `_update_projectiles` — advance, resolve hits (damage/kill)
5. `_sync_sprites` — positions, walk animation, hit flash, HP bars, pulses
6. `_update_hud` — money, wave state, icon swap

Removing objects is always done with a backwards index loop
(`for i in range(size - 1, -1, -1)`) because handlers can remove the current
element. Sprites live in parallel dictionaries keyed by the core object
(`_drone_sprites[drone]`, `_drone_bars[drone]`, `_projectile_sprites[p]`) and
are freed together with it in `_remove_drone()` / `_remove_projectile()`.

## Event flows

```
Space ──► _start_wave(n)
            WaveGen.composition(n, GAME_SEED)
            queue = normals… + fast… + tanks…      (fixed order)
            telemetry "wave" · SFX "wave"

Wave cleared ──► _break_timer = BREAK_SECONDS      (HUD countdown)
            timeout or Space ──► _start_wave(n + 1)   (auto-chain)

spawner ──► _spawn_drone(kind)
            path = Pathfinder.find_path(spawn, base)   (snapshot)
            Drone.spawn(kind, cells, hp, speed) + sprite + HP bar

Drone.advance(dt) == true ──► _on_leak
            Economy.on_leak() · telemetry "leak" · SFX "leak"
            money < 0 ──► _end_run (game-over screen, "gameover" SFX, log flush)

R / restart button (game over) ──► _restart: reload_current_scene()

Gun.try_fire(dt) ──► _fire: tracer starts at muzzle (0.75 cells) + flash
Projectile hit ──► _resolve_hit
            kill: Economy.on_kill() · explosion · SFX "kill"
            else: hit flash + impact spark · SFX "hit"

Left click ──► spend(25) → reject? refund + SFX "denied"
                        → build: Gun + tower sprite + telemetry "build" + SFX "build"
Right click ─► Maze.sell → refund 12 + telemetry "sell" + SFX "sell"
Both        ──► _reroute_drones(): every drone re-paths from its current cell
```

## Invariants

- Spawn↔base is always connected: `Maze.can_build()` blocks a cell
  hypothetically and checks `Pathfinder.has_path()` before committing.
- A tower can never be built on the cell a drone currently occupies
  (`_drone_on_cell()`); therefore a reroute never starts from a solid cell.
- A drone always has a non-empty path. `reroute()` keeps the position (no
  teleport) and treats a path that is only the base cell as a leak.
- A drone that reaches the base leaks exactly once and is removed immediately
  (kills likewise), so `_drones` never contains dead or leaked drones.
- Game over is strictly `money < 0`, checked only after a leak.
- Wave phases are mutually exclusive: running (`_wave_running`), break
  (`_break_timer > 0`), or idle before wave 1. A wave only ends when its
  spawn queue and the field are both empty.
- After game over `_process` stops (frozen world); the only accepted input
  is restart.
- Money changes only through `Economy`; the HUD reads it, never writes it.

## Determinism

Gameplay RNG exists in exactly one place: `WaveGen`, seeded with
`game_seed + n * 7919`. Everything else is derived:

- spawn order: normals, then fast, then tanks (fixed)
- floor variety: `(x * 7 + y * 13) % 3` (stable pattern)
- explosion frame alternation: `_fx_counter`
- animation phases: spawn index (`_drone_phase`), not RNG

Same seed + same build/sell sequence ⇒ identical run. `GAME_SEED` is
currently fixed to `1` in `game.gd`; the run seed is written into the
telemetry log for replay/analysis (T05).

## Rendering & z-order

Floor tiles, spawn/base markers, towers and drones are `Sprite2D` children of
`Main` (default z 0) in creation order. HP bars use `z_index = 1` so they
stay above all drones. The live path preview is drawn in `Main._draw()`
(behind all children). The HUD is a `CanvasLayer` and therefore unaffected by
world coordinates.

## Testing

- Core classes: pure GUT tests in `tests/test_*.gd`, no scene needed
  (`test_maze`, `test_path`, `test_economy`, `test_wave`, `test_drone`,
  `test_gun`, `test_projectile`).
- Scene: `tests/test_game_scene.gd` instantiates `Main.tscn` and steps
  `_process(1.0 / 60.0)` manually, so assertions are frame-rate independent;
  it covers spawn → walk → shoot → kill, the payout, break → auto-chain,
  the Space skip, and the game-over screen incl. telemetry flush and
  restart-button wiring (writes only `user://`, then deletes the file).
- Gotcha: GUT's GUI panel covers the right half of the window, so
  screenshots taken from a GUT run are cropped. For visual QA use a
  temporary `SceneTree` script (`godot --path . -s tools/x.gd`,
  `root.add_child(main_scene)`, `await process_frame` before touching nodes).

## Extension points

- **New tower type**: `Gun` is parameter-free by design; either add a
  constructor/params or a subclass with different `RANGE`/`DAMAGE`/`INTERVAL`
  and a new sprite set. `_guns` maps cell → `Gun`, so multiple types only
  need a type field.
- **New drone kind**: add multipliers to `Drone.KIND_MODS` and a frame array
  to `game.gd` `DRONE_TEX`; `WaveGen` decides counts.
- **New wave shape**: `WaveGen.composition()` is the single source; keep it
  seeded and deterministic and update `tests/test_wave.gd`.
- **Balance changes**: constants are documented in `docs/BALANCE.md`; run the
  suite after touching them (`test_wave`/`test_economy` pin several values).
