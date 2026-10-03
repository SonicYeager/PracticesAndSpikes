# Architecture

Prototype architecture: one scene, pure-logic core classes, a thin scene
controller for presentation. The split exists so gameplay rules are
unit-testable without rendering (see *Testing*).

## Layers

```
scenes/Main.tscn
  └─ Main (Node2D, scripts/game.gd)      ← scene controller
       ├─ Board (scripts/board_view.gd)  ← board rendering + grid math
       │    ├─ floor/terrain/decor sprites (z −1) + decals (z −1)
       │    ├─ entry/exit markers + ambient emitters
       │    └─ _draw(): live route preview
       ├─ Fx (scripts/fx.gd)             ← combat FX + screen shake
       ├─ tower/drone/projectile sprites (created at runtime)
       └─ Hud (CanvasLayer)
            ├─ Vignette (full-screen overlay)
            └─ HudRoot (scenes/Hud.tscn, scripts/hud.gd)   ← UI boundary
                 ├─ LeakFlash overlay (first child, under the panels)
                 └─ panels (status/build/wave/game-over)

scripts/*.gd (class_name RefCounted)     ← core logic, no scene access
  Maze · Pathfinder · Economy · WaveGen · WaveDirector · Drone · Gun
  GunUpgrades · Pieces · Projectile · Telemetry · SkillStub

UI scripts (Control-based, no gameplay access)
  GameHud (scripts/hud.gd) · GameHudBar (scripts/hud_bar.gd)

View classes (Node2D, presentation only, state pushed in)
  BoardView (scripts/board_view.gd) · Fx (scripts/fx.gd)
```

Core classes never reference nodes, `origin`, `TILE` or textures. The scene
controller owns units and projectiles (sprites, tweens, audio); `Fx` owns the
combat effects and the screen shake; `BoardView` owns the board (tiles,
terrain, markers, decals, ambient, route preview) and the grid math; the HUD
owns its panels and only sees pushed state.

## Core classes

| Class | File | Responsibility |
|---|---|---|
| `Maze` | `scripts/maze.gd` | Buildable grid (`built: cell → tower_type`), entry/exit reservation, `can_build()` via hypothetical block + multi-source BFS (every entry keeps an exit), `build()`, `sell()`, `clear_blocker()` |
| `Pathfinder` | `scripts/pathfinder.gd` | `AStarGrid2D` wrapper; `DIAGONAL_MODE_NEVER`, solid points, `find_path()` returns cell ids, `has_path()`, `reachable_from()` multi-source BFS |
| `Economy` | `scripts/economy.gd` | Money-as-health: `on_kill(amount)`, `on_leak()`, `spend()`, `is_game_over()` (strictly `< 0`) |
| `WaveGen` | `scripts/wave.gd` | Static `composition(n, seed)` → `{wave, count, hp, speed, tanks, fast, modifier}` (T10 events from wave 3) |
| `WaveDirector` | `scripts/wave_director.gd` | Wave flow state machine (T12.5): phase, composition, spawn queue, spawn/break timers, per-wave modifier knobs; `start()`, `begin_break()`, `tick_spawn()`, `tick_break()`, `end_run()` |
| `TerrainGen` | `scripts/terrain_gen.gd` | Static `generate(size, entries, exits, seed)` → `{blockers, decor}`; greedy placement that never seals an entry |
| `Drone` | `scripts/drone.gd` | Grid-space walker: `advance(dt)` (returns `true` on leak), `take_damage()`, `reroute()`, `facing()`, `exit_cell()`, `distance_to_exit()`, `KIND_MODS` |
| `Gun` | `scripts/gun.gd` | Level-based range/cadence/damage (`GunUpgrades`, ADR 0012) + targeting (`range_bonus` hook for wave modifiers); `acquire()` picks the drone closest to its exit, `try_fire(dt, targets)` returns the target when a shot is due |
| `GunUpgrades` | `scripts/gun_upgrades.gd` | Five-level upgrade table (ADR 0012): cumulative prices, deltas, half refunds, names/descriptions; static helpers only |
| `Pieces` | `scripts/pieces.gd` | Buildable kinds (T17): `Kind {GUN, WALL}` → cost/label/telemetry name; static, no instances |
| `Projectile` | `scripts/projectile.gd` | Homing tracer carrying its damage (level-aware); `advance(dt)` returns `true` on hit, fizzles when the target dies or leaks |
| `Telemetry` | `scripts/telemetry.gd` | Buffers JSON events (wave/build/sell/clear/upgrade/leak/kill/send/overcharge + `wave_end` summaries), `flush(path)` writes JSONL; `run_start` carries `source`/`harness` provenance |
| `SkillStub` | `scripts/skill_stub.gd` | Meta stub: one bonus persisted via `ConfigFile` |

## Coordinates

Core logic uses **grid space**: 1 unit = 1 cell, cell center is
`Vector2(cell) + Vector2(0.5, 0.5)`, speed is cells/second. `BoardView`
converts at the boundary:

```gdscript
world = origin + grid_pos * TILE        # BoardView.grid_to_world()
cell  = Vector2i((world - origin) / TILE).floor()   # BoardView.to_cell()
```

`WaveGen` speeds are px/s (author-friendly); `game.gd` divides by `TILE`
before handing them to `Drone`. `Pathfinder` works in integer cells and is
independent of `origin`/`TILE` (its `cell_size` is cosmetic for path length
only).

## Frame order (`game.gd _process`)

0. `_fx.update` — trauma decay + camera offset (also runs after game over)
1. `_update_spawner` — asks `WaveDirector` for the next spawn / break timeout
   (`tick_break`, `tick_spawn`); `_spawn_drone` scatters the entry cell and
   draws the assigned exit; empty queue + clear field → `begin_break()`
2. `_update_drones` — advance along the path, handle leaks
3. `_update_guns` — acquire target, aim barrel, fire (muzzle tracer + flash)
4. `_update_projectiles` — advance, resolve hits (damage/kill)
5. `_sync_sprites` — positions, walk animation, hit flash, HP bars, pulses
6. `_update_hud` — pushes money/wave/progress state into `GameHud.update_state`
   (labels, bar, chip, button states)
7. `BoardView._process` — its own update: entry/exit marker pulse and the
   vent-only ambient emitters (presentation only, runs in every phase)

Removing objects is always done with a backwards index loop
(`for i in range(size - 1, -1, -1)`) because handlers can remove the current
element. Sprites live in parallel dictionaries keyed by the core object
(`_drone_sprites[drone]`, `_drone_bars[drone]`, `_projectile_sprites[p]`) and
are freed together with it in `_remove_drone()` / `_remove_projectile()`.

## Event flows

```
Space/HUD button ──► _on_wave_pressed: telemetry "send" · _start_wave(n)
            WaveDirector.start(n): WaveGen.composition → modifier knobs
            (rush/swarm/blackout/bounty; knobs reset every wave)
            queue = normals… + fast… + tanks…      (fixed order)
            scene: gun range ← director · telemetry "wave" · SFX "wave"

Wave cleared ──► telemetry "wave_end" · WaveDirector.begin_break()  (HUD countdown)
            timeout (tick_break) or Space ──► _start_wave(n + 1)   (auto-chain)

spawner ──► _spawn_drone(kind)
            entry + assigned exit drawn from the seeded scatter RNG
            path = Pathfinder.find_path(entry, exit)        (snapshot)
            Drone.spawn(kind, cells, hp, speed) + sprite + HP bar

Drone.advance(dt) == true ──► _on_leak
            Economy.on_leak() · telemetry "leak" (+ exit cell) · SFX "leak"
            skid decal at the leak cell
            money < 0 ──► _end_run (game-over screen, "gameover" SFX, log flush)

R / restart button (game over) ──► _restart: reload_current_scene()
HUD wave button ─► _on_wave_pressed (IDLE/BREAK only) → _start_wave(n + 1)
HUD sell toggle ─► _sell_mode: LMB sells towers (RMB always sells)
HUD state       ◄─ GameHud.update_state({money, wave, phase, alive, queued,
                    total, break_left, modifier_id, modifier_label, selected,
                    build_cost, build_label, build_kind})
                    phase: "idle" | "running" | "break" | "game_over"

Gun.try_fire(dt) ──► _fire: tracer starts at muzzle (0.75 cells) + flash
Projectile hit ──► _resolve_hit
            kill: Economy.on_kill() · explosion + shockwave ring · SFX "kill" · scorch decal · telemetry "kill"
            else: hit flash + impact spark · SFX "hit" · debris decal
Fx ──► muzzle/impact/explosion/ember bursts + barrel recoil (world space)
            taken from the same call sites; Fx.update decays the shake trauma

Left click ──► _dispatch_primary(cell): sell-mode → sell · vent → overcharge
                (_try_overcharge: spend(20) · cooldown 6 s · ember burst ·
                damage 15 in 2.5 cells · telemetry "overcharge")
              tower? → select (HUD row: name · +delta, UPGRADE button)
              wall? → denied feedback (no spend, no deselect)
              else → _try_build(kind): spend(Pieces.cost) → reject? refund +
                        SFX "denied" → build: Gun/Wall + sprite + dust puff +
                        telemetry "build" {kind} + SFX "build"
              UPGRADE button ─► _try_upgrade: spend(delta) · level+1 · pips/tint
                        · dust puff · telemetry "upgrade" · SFX "build"
              B ──► _toggle_build_kind (HUD caption/cost/icon follow)
              ESC ──► _deselect_tower()
Right click ─► _try_sell → Maze.sell → refund half (gun per level / wall 5) + dust puff + telemetry "sell" {kind}
              else _try_clear: blocker (non-vent)? spend(15) → clear_blocker
                        + sprite removed + dust puff + telemetry "clear"
Both        ──► _reroute_drones(): each drone re-paths to its assigned exit,
                or to the nearest reachable exit when that one is cut off
            ──► _sync_sprites pushes the routes (BoardView.set_routes) each
                frame, so the preview follows the new maze
```

## Invariants

- Every entry always reaches at least one exit: `Maze.can_build()` blocks a
  cell hypothetically and checks the multi-source BFS from all exits.
  Sealed exits are inert; creative splits (left→top, right→bottom) are fine.
- Terrain blockers are solid and reserved from the start; `TerrainGen`
  applies the same greedy rule, so the map is playable before the first
  build. A blocker can never be built or sold over; rock/rubble can be
  cleared for 15 (right click) — vents stay as the money sink.
- A tower can never be built on the cell a drone currently occupies
  (`_drone_on_cell()`); therefore a reroute never starts from a solid cell.
- A drone's path always ends at its assigned exit. `reroute()` keeps the
  position (no teleport) and treats a path that is only the exit cell as a
  leak; if the assigned exit is cut off, the scene falls back to the nearest
  reachable exit (assignment holds only while valid).
- A drone that reaches the base leaks exactly once and is removed immediately
  (kills likewise), so `_drones` never contains dead or leaked drones.
- Game over is strictly `money < 0`, checked only after a leak.
- The wave phase is a single explicit state (`WaveDirector.Phase`: IDLE →
  RUNNING → BREAK → RUNNING … → GAME_OVER); the director owns the queue and
  the BREAK countdown. A wave only ends when its spawn queue and the field
  are both empty.
- After game over `_process` stops (frozen world); the only accepted input
  is restart.
- Money changes only through `Economy`; the HUD reads it, never writes it.
- Telemetry is write-only: events never feed back into the simulation;
  `run_source` only tags the log (harness vs human provenance).
- Wave-modifier knobs (`_spawn_interval`, `_range_bonus`, `_kill_reward`)
  reset at every `_start_wave`; only `swarm` mutates the composition itself.
- Battle decals are capped per cell (`DECAL_CELL_CAP`) and globally
  (`DECAL_CAP`); the per-cell cap keeps kill zones readable.

## Determinism

Gameplay RNG exists in exactly three places, all seeded from the run seed:
`WaveGen` composition (`game_seed + n * 7919`), the per-spawn scatter
(`game_seed * 1000003 + wave * 104729 + spawn index`, entry cell + exit) and
`TerrainGen` dressing. Everything else is derived:

- run seed: random per run (`_random_seed()`), logged as `run_start.seed`;
  `seed_override` pins it for tests/editor — a logged seed replays a run
- spawn order: normals, then fast, then tanks (fixed)
- spawn scatter: per-spawn RNG, deterministic given the run seed
- wave modifiers: rolled after the tanks/fast split, so those stay stable
  per seed; the modifier itself is part of the composition
- terrain: deterministic given the run seed (greedy placement)
- floor variety: `(x * 7 + y * 13) % 3` (stable pattern)
- explosion frame alternation: `_fx_counter`
- animation phases: spawn index (`_drone_phase`), not RNG
- screen shake: sine pseudo-noise of `_anim_time` (presentation only)
- ambient particles: internal particle RNG (presentation only, not part of
  the replay guarantee)

Same seed + same build/sell/clear/upgrade sequence ⇒ identical run. The run seed is
written into the telemetry log for replay/analysis (`tools/analyze_run.py`).

## Rendering & z-order

Floor tiles, decor and decals are `Sprite2D` children of `Board`
(`z_index = -1`); entry/exit markers and the vent ambient emitters also live
there.
Towers, walls, drones and projectiles are `Sprite2D` children of `Main` (default
z 0) in creation order; FX sprites are added under `Fx` (`z_index = 1`, so
effects cover units). HP bars also use `z_index = 1` and are appended after
`Fx`, so they stay above the effects. The live path preview is drawn
in `BoardView._draw()` (above floor/decor/decals, below units). The HUD is a
`CanvasLayer` and therefore unaffected by world coordinates.

Floor tiles, cosmetic decor and battle decals use `z_index = -1`, so
`BoardView._draw()` — the live route preview tracing each active drone's
assigned path — stays visible above the grime and below blockers/towers/
drones (z 0; HP bars z 1). A `Camera2D` centered on the viewport carries the
screen shake; CanvasLayer content is not affected by it, so the HUD stays
fixed. The vignette is the first child of the HUD layer; inside HudRoot the
leak-flash overlay draws beneath the panels (world → vignette → leak flash →
panels → game-over overlay).

## Testing

- Core classes: pure GUT tests in `tests/test_*.gd`, no scene needed
  (`test_maze`, `test_path`, `test_economy`, `test_wave`,
  `test_wave_director`, `test_drone`, `test_gun`, `test_gun_upgrades`,
  `test_pieces`, `test_projectile`, `test_terrain_gen`).
- HUD: `tests/test_hud.gd` instantiates `Hud.tscn` and drives the state API
  and intents (labels, chip, button gating, sell toggle, upgrade row,
  game-over overlay, bar freeze on game over) without the game scene.
- Scene: `tests/test_game_scene.gd` instantiates `Main.tscn` (with
  `seed_override` pinned) and steps `_process(1.0 / 60.0)` manually, so
  assertions are frame-rate independent; it covers spawn → walk → shoot →
  kill with scattered entries and terrain, the payout, break → auto-chain,
  the Space skip, scatter/terrain determinism, the nearest-exit fallback,
  overcharge, modifier knobs, decal caps + ambient setup, terrain clearing,
  upgrades + selection + level pips, wall builds + build-kind toggle, the
  game-over
  screen incl. telemetry flush and restart-button wiring, and the camera
  shake offset/decay (writes only `user://`, then deletes the file).
- Gotcha: GUT's GUI panel covers the right half of the window, so
  screenshots taken from a GUT run are cropped. For visual QA use
  `tools/shot.gd` (windowed; headless is refused): `Godot --path . -s
  tools/shot.gd -- --states idle,running --towers "8,3;9,6"` →
  `<out-prefix>_<state>.png` (default `reports/shot`).

## Extension points

- **New combat effect**: add a spawn method to `Fx` (world-space positions)
  and call it from the event site in `game.gd`; shake intensities stay
  per-event constants in `game.gd`.
- **New piece/tower type**: kinds live in `Pieces` (T17: cost/label/telemetry
  name; costs in `Economy`); `_try_build(cell, kind)` branches on the kind and
  `_guns`/`_walls` map cells to their bookkeeping. The HUD slot follows the
  current kind (caption/cost/icon); a clickable two-slot selector is a queued
  visual slice (BACKLOG `tower-defense-build-selector`).
- **New drone kind**: add multipliers to `Drone.KIND_MODS` and a frame array
  to `game.gd` `DRONE_TEX`; `WaveGen` decides counts.
- **New wave shape**: `WaveGen.composition()` is the single source; keep it
  seeded and deterministic and update `tests/test_wave.gd`.
- **More entry/exit sides**: `ENTRY_SIDES` / `EXIT_SIDES` in `game.gd` (the
  `Side` enum) are deduplicated into cell lists. Segments (side + cell
  range) are a future config detail — validation and the nearest-exit
  fallback already handle non-contiguous exits.
- **Terrain density/types**: `TerrainGen` constants (cluster count/size,
  decor count); a new blocker art is a `TERRAIN_TEX` entry in `BoardView`.
  Battle decals are capped by `BoardView.DECAL_CAP` (FIFO).
- **Balance changes**: constants are documented in `docs/BALANCE.md`; run the
  suite after touching them (`test_wave`/`test_economy` pin several values).
