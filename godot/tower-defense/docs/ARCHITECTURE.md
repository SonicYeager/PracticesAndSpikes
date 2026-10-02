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
| `Maze` | `scripts/maze.gd` | Buildable grid (`built: cell → tower_type`), entry/exit reservation, `can_build()` via hypothetical block + multi-source BFS (every entry keeps an exit), `build()`, `sell()` |
| `Pathfinder` | `scripts/pathfinder.gd` | `AStarGrid2D` wrapper; `DIAGONAL_MODE_NEVER`, solid points, `find_path()` returns cell ids, `has_path()`, `reachable_from()` multi-source BFS |
| `Economy` | `scripts/economy.gd` | Money-as-health: `on_kill()`, `on_leak()`, `spend()`, `is_game_over()` (strictly `< 0`) |
| `WaveGen` | `scripts/wave.gd` | Static `composition(n, seed)` → `{wave, count, hp, speed, tanks, fast}` |
| `TerrainGen` | `scripts/terrain_gen.gd` | Static `generate(size, entries, exits, seed)` → `{blockers, decor}`; greedy placement that never seals an entry |
| `Drone` | `scripts/drone.gd` | Grid-space walker: `advance(dt)` (returns `true` on leak), `take_damage()`, `reroute()`, `facing()`, `exit_cell()`, `distance_to_exit()`, `KIND_MODS` |
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

0. `_update_shake` — trauma decay + camera offset (also runs after game over)
1. `_update_spawner` — runs the `BREAK_SECONDS` countdown (auto-starts the
   next wave), pops one drone from the queue every `SPAWN_INTERVAL`
   (`_spawn_drone` scatters the entry cell and draws the assigned exit)
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
            WaveGen.composition(n, game_seed)
            queue = normals… + fast… + tanks…      (fixed order)
            telemetry "wave" · SFX "wave"

Wave cleared ──► _break_timer = BREAK_SECONDS      (HUD countdown)
            timeout or Space ──► _start_wave(n + 1)   (auto-chain)

spawner ──► _spawn_drone(kind)
            entry + assigned exit drawn from the seeded scatter RNG
            path = Pathfinder.find_path(entry, exit)        (snapshot)
            Drone.spawn(kind, cells, hp, speed) + sprite + HP bar

Drone.advance(dt) == true ──► _on_leak
            Economy.on_leak() · telemetry "leak" (+ exit cell) · SFX "leak"
            skid decal at the leak cell
            money < 0 ──► _end_run (game-over screen, "gameover" SFX, log flush)

R / restart button (game over) ──► _restart: reload_current_scene()

Gun.try_fire(dt) ──► _fire: tracer starts at muzzle (0.75 cells) + flash
Projectile hit ──► _resolve_hit
            kill: Economy.on_kill() · explosion · SFX "kill" · scorch decal
            else: hit flash + impact spark · SFX "hit" · debris decal

Left click ──► spend(25) → reject? refund + SFX "denied"
                        → build: Gun + tower sprite + telemetry "build" + SFX "build"
Right click ─► Maze.sell → refund 12 + telemetry "sell" + SFX "sell"
Both        ──► _reroute_drones(): each drone re-paths to its assigned exit,
                or to the nearest reachable exit when that one is cut off
            ──► queue_redraw(): the route preview follows the new maze
```

## Invariants

- Every entry always reaches at least one exit: `Maze.can_build()` blocks a
  cell hypothetically and checks the multi-source BFS from all exits.
  Sealed exits are inert; creative splits (left→top, right→bottom) are fine.
- Terrain blockers are solid and reserved from the start; `TerrainGen`
  applies the same greedy rule, so the map is playable before the first
  build. A blocker can never be built or sold over.
- A tower can never be built on the cell a drone currently occupies
  (`_drone_on_cell()`); therefore a reroute never starts from a solid cell.
- A drone's path always ends at its assigned exit. `reroute()` keeps the
  position (no teleport) and treats a path that is only the exit cell as a
  leak; if the assigned exit is cut off, the scene falls back to the nearest
  reachable exit (assignment holds only while valid).
- A drone that reaches the base leaks exactly once and is removed immediately
  (kills likewise), so `_drones` never contains dead or leaked drones.
- Game over is strictly `money < 0`, checked only after a leak.
- The wave phase is a single explicit state (`Phase`: IDLE → RUNNING →
  BREAK → RUNNING … → GAME_OVER); `_break_timer` only carries the BREAK
  countdown. A wave only ends when its spawn queue and the field are both
  empty.
- After game over `_process` stops (frozen world); the only accepted input
  is restart.
- Money changes only through `Economy`; the HUD reads it, never writes it.

## Determinism

Gameplay RNG exists in exactly three places, all seeded from the run seed:
`WaveGen` composition (`game_seed + n * 7919`), the per-spawn scatter
(`game_seed * 1000003 + wave * 104729 + spawn index`, entry cell + exit) and
`TerrainGen` dressing. Everything else is derived:

- run seed: random per run (`_random_seed()`), logged as `run_start.seed`;
  `seed_override` pins it for tests/editor — a logged seed replays a run
- spawn order: normals, then fast, then tanks (fixed)
- spawn scatter: per-spawn RNG, deterministic given the run seed
- terrain: deterministic given the run seed (greedy placement)
- floor variety: `(x * 7 + y * 13) % 3` (stable pattern)
- explosion frame alternation: `_fx_counter`
- animation phases: spawn index (`_drone_phase`), not RNG
- screen shake: sine pseudo-noise of `_anim_time` (presentation only)
- ambient particles: internal particle RNG (presentation only, not part of
  the replay guarantee)

Same seed + same build/sell sequence ⇒ identical run. The run seed is
written into the telemetry log for replay/analysis (`tools/analyze_run.py`).

## Rendering & z-order

Floor tiles, spawn/base markers, towers and drones are `Sprite2D` children of
`Main` (default z 0) in creation order. HP bars use `z_index = 1` so they
stay above all drones. The live path preview is drawn in `Main._draw()`
(behind all children). The HUD is a `CanvasLayer` and therefore unaffected by
world coordinates.

Floor tiles, cosmetic decor and battle decals use `z_index = -1`, so
`Main._draw()` — the live route preview tracing each active drone's assigned
path — stays visible above the grime and below blockers/towers/drones
(z 0; HP bars z 1). A `Camera2D` centered on the viewport carries the
screen shake; CanvasLayer content is not affected by it, so the HUD stays
fixed. The vignette is the first child of the HUD layer (world → vignette →
panel → game-over overlay, in that draw order).

## Testing

- Core classes: pure GUT tests in `tests/test_*.gd`, no scene needed
  (`test_maze`, `test_path`, `test_economy`, `test_wave`, `test_drone`,
  `test_gun`, `test_projectile`, `test_terrain_gen`).
- Scene: `tests/test_game_scene.gd` instantiates `Main.tscn` (with
  `seed_override` pinned) and steps `_process(1.0 / 60.0)` manually, so
  assertions are frame-rate independent; it covers spawn → walk → shoot →
  kill with scattered entries and terrain, the payout, break → auto-chain,
  the Space skip, scatter/terrain determinism, the nearest-exit fallback,
  decals + ambient setup, the game-over screen incl. telemetry flush and
  restart-button wiring, and the camera shake offset/decay (writes only
  `user://`, then deletes the file).
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
- **More entry/exit sides**: `ENTRY_SIDES` / `EXIT_SIDES` in `game.gd` (the
  `Side` enum) are deduplicated into cell lists. Segments (side + cell
  range) are a future config detail — validation and the nearest-exit
  fallback already handle non-contiguous exits.
- **Terrain density/types**: `TerrainGen` constants (cluster count/size,
  decor count); a new blocker art is a `TERRAIN_TEX` entry. Battle decals
  are capped by `DECAL_CAP` (FIFO).
- **Balance changes**: constants are documented in `docs/BALANCE.md`; run the
  suite after touching them (`test_wave`/`test_economy` pin several values).
