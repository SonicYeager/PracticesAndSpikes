# tower-defense

## About

Sci-Fi 2D maze tower defense prototype (GDScript, Godot 4.7.2). Free maze
building with AStarGrid2D pathfinding, money-is-HP economy (game over below
zero), endless deterministic waves, meta skill-tree stub, local JSONL
telemetry for data-driven balancing.

- Stack: Godot 4.7.2, GDScript (no .NET flow)
- Entrypoint: `scenes/Main.tscn` (open/import the folder in the Godot editor)
- Status: prototype complete + sides + dirty world + pulse + XT look + HUD (T01–T12) — see *Controls* and *Roadmap*

## Controls

| Input | Action |
|---|---|
| Left click | Build a gun (25) — rejected builds refund instantly |
| Left click on a vent | Overcharge (20): ember burst, 15 dmg in 2.5 cells, 6 s cooldown |
| Right click | Sell a gun (refund 12); the HUD VERKAUFEN toggle sells via left click while active |
| Space (or the wave button) | Start wave 1 / skip the break between waves |
| R (or the button) | Restart after game over |

Rules: money **is** health — kills earn +6, leaks cost −10, game over strictly
below zero. Builds that would leave an entry without a reachable exit (or
land on a tile a drone currently occupies) are rejected. Drones re-route when
the maze changes.
After a wave is cleared, a 5 s break runs (`BREAK_SECONDS`); the next wave
then auto-starts. Game over shows a run summary and restarts the scene.

## Features (current)

- Variable grid (20×12, TILE 32), free maze building, live route preview of
  active drones.
- 4-directional AStarGrid2D pathfinding, no corner slipping.
- Deterministic endless waves: `WaveGen.composition(n, seed)`, no unseeded
  RNG anywhere in gameplay — same seed + same builds = same run.
- Wave flow: auto-chaining after a 5 s break with HUD countdown (Space
  skips); game-over screen with run summary + restart.
- Gun auto-targeting (closest to base, range 3.5 cells, 8 dmg / 0.6 s) with
  homing tracers fired from the barrel muzzle.
- Three drone kinds (shape + color coded): normal, fast, tank.
- Feedback: 9 synthesized SFX, muzzle flash, hit sparks, HP bars, walk
  animation, explosions, HUD icon panel, pulsing spawn portal/base bunker.
- Polish (T06): trauma-based screen shake (Camera2D, deterministic sines),
  vignette overlay, barrel recoil, muzzle/impact pops.
- Sides (T08): enemies enter scattered along the left side and each draws a
  seeded random exit on the right side; reaching an exit is a leak. Builds
  must keep every entry connected to at least one exit.
- Dirty World (T09): random run seed (logged for replay), seeded terrain
  blockers + decor, battle decals (scorch/skid/debris) that accumulate
  during the run, ambient embers/smoke/sparks.
- Pulse (T10): per-wave events from wave 3 (Ansturm/Schwarm/Blackout/
  Kopfgeld) telegraphed in the HUD, vent overcharge as money sink with an
  ember burst, decal per-cell cap keeps kill zones readable.
- XT-Pass (T11): Xeno-Tactic look — dark blue-grey steel lab tiles, blue
  player turrets, white/acid-green alien drones, VT323 UI font (OFL,
  `fonts/`), drier SFX; reference study gitignored in `reference/xt/`.
- HUD (T12): XT-style panels — status (gold, wave, modifier chip), build/sell
  panel, segmented wave bar with a clickable SEND NEXT WAVE button; restyled
  game-over overlay. Owned by `scenes/Hud.tscn` + `scripts/hud.gd`; `game.gd`
  pushes state into it.
- Local telemetry: build/sell/wave/leak/run_end events →
  `user://run_<seed>.jsonl` (analysis: `tools/analyze_run.py`).
- Meta stub: one persistent bonus (`SkillStub` → `user://skill_stub.cfg`).

## Getting started

Prerequisite: Godot 4.7.x (machine-local, `%GODOT_BIN%`; on this machine
`D:\Tools\godot\`).

```bash
godot --headless --path . --import --quit   # import art/audio (first run)
godot --path .                              # play
```

## Tests

```bash
GODOT_DISABLE_LEAK_CHECKS=1 godot --headless --path . \
  -s res://addons/gut/gut_cmdln.gd -gdir=res://tests -ginclude_subdirs -gexit
```

GUT 9.6.1 is vendored under `addons/gut/`. The suite (58 tests / 472 asserts)
covers every core class plus one scene integration smoke test that steps the
real `Main.tscn` (spawn → walk → shoot → kill with scattered entries and
terrain, break → auto-chain, game over + log flush, restart wiring, camera
shake decay, scatter determinism, nearest-exit fallback, overcharge,
modifiers, decal caps).

## Project structure

| Path | Purpose |
|---|---|
| `scenes/Main.tscn` | Root scene: game node, camera, vignette + HUD instance |
| `scenes/Hud.tscn` | HUD layout: status, build/sell panel, wave bar, game-over overlay |
| `scripts/hud.gd` | HUD state API + intents (wave/sell/restart signals), chip colors |
| `scripts/hud_bar.gd` | Segmented wave-progress bar |
| `scripts/game.gd` | Scene controller: input, orchestration, sprites, effects, audio, HUD push |
| `scripts/wave_director.gd` | Wave flow state machine: phase, queue, timers, modifier knobs |
| `scripts/maze.gd` | Buildable grid + connectivity validation (`Maze`) |
| `scripts/pathfinder.gd` | AStarGrid2D wrapper, 4-directional (`Pathfinder`) |
| `scripts/economy.gd` | Money-is-HP rules (`Economy`) |
| `scripts/wave.gd` | Deterministic wave composition (`WaveGen`) |
| `scripts/terrain_gen.gd` | Seeded terrain dressing: blockers + decor (`TerrainGen`) |
| `scripts/drone.gd` | Grid-space walker: path, hp, kinds (`Drone`) |
| `scripts/gun.gd` | Tower targeting/cadence (`Gun`) |
| `scripts/projectile.gd` | Homing tracer (`Projectile`) |
| `scripts/telemetry.gd` | JSONL event writer (`Telemetry`) |
| `scripts/skill_stub.gd` | Meta-progression stub (`SkillStub`) |
| `tests/` | GUT tests (`test_*.gd`) |
| `art/` | Placeholder sprites + `STYLEGUIDE.md` |
| `audio/` | Synthesized SFX (generated by `tools/make_sounds.py`) |
| `tools/` | stdlib-only Python tools (asset generators, telemetry analysis) |

## Assets

Both asset sets are generated by stdlib-only Python scripts and can be
replaced by hand-made files under the same names — no code changes needed:

```bash
python tools/make_placeholders.py   # art/*.png (sprites + vignette)
python tools/make_sounds.py         # audio/*.wav (22050 Hz mono)
```

## Telemetry analysis

`tools/analyze_run.py` (stdlib only) reads the run logs and prints per-wave
leak/kill/money/build tables plus an aggregate across runs:

```bash
python tools/analyze_run.py                      # scans telemetry_local/, then the Godot user dir
python tools/analyze_run.py path/to/run_1.jsonl  # or explicit files / directories
```

Keep runs by copying `user://run_<seed>.jsonl` into `telemetry_local/`
(gitignored) — every run writes its own file (random seed per run).

The parser has stdlib regression tests: `python tools/test_analyze_run.py`.

## Documentation

| Doc | Content |
|---|---|
| `AGENTS.md` | Agent entry point: fixed decisions, slice status, commands, gotchas |
| `docs/ARCHITECTURE.md` | Classes, frame order, coordinates, invariants, determinism, tests |
| `docs/BALANCE.md` | Every tunable constant + derived numbers + tuning workflow |
| `docs/AUDIO.md` | Sound list, synthesis recipes, volume tuning, replacing SFX |
| `docs/VISION.md` | Workshop result: pillars, values, anti-pillars (north star) |
| `art/STYLEGUIDE.md` | XT steel/lab palette, sprite specs + inventory |
| `docs/IDEAS.md` | Post-prototype directions (multi-entry/exit sides, open items) |
| `docs/BACKLOG.md` | Next slices: candidate queue for `/next` + `/loop` (+ done log) |

## Roadmap

| Slice | Content | State |
|---|---|---|
| T01 | Scaffold, variable grid | done |
| T02 | Maze validation, click build/sell, GUT setup | done |
| T03 | Combat: drones, guns, projectiles, leak/kill economy, telemetry | done |
| Juice | SFX, muzzle/hit effects, HP bars, walk animation, HUD icons, spawn/base redesign | done |
| T04 | Wave chaining (auto-next + break), game-over screen + restart | done |
| T05 | Telemetry analysis script (Python) | done |
| T06 | Remaining polish (screen shake, vignette, …) | done |
| T07 | Docs pass | done |
| T08 | Multi-entry/exit sides, scatter spawn, nearest-exit fallback | done |
| T09 | Dirty World: run seed, terrain blockers/decor, decals, ambient | done |
| T10 | Pulse: wave events, vent overcharge, decal per-cell cap | done |
| T11 | XT pass: steel/lab look, VT323 UI font, drier SFX | done |
| T12 | HUD framework: status/build panels, wave bar, game-over restyle | done |
| T12.5 | Refactor: extract WaveDirector (wave state machine) | done |

All slices done (T01–T12.5). Post-prototype directions are collected in `docs/IDEAS.md`;
the concrete candidate queue is `docs/BACKLOG.md`.

### From prototype to game

**Target picture** — a self-contained session game: one run has a beginning, a pressure
arc and an ending (mission clear, game over, or the role-reversal epilogue); 2–3 clearly
different tower roles and 3–5 enemy behaviors that ask new questions instead of grinding;
light unlock meta (missions, behaviors, event pools); calm with a pulse — no twitch, no
treadmill.

| Milestone | Content | Prototype question / kill |
|---|---|---|
| **M1 — One complete run** | Time control (pause/speed, actions in pause); finite run (15–25 waves, win/lose, run summary); wave preview (data + HUD); wall piece; telemetry enrichment + balance harness; minimal help/legend | Does a finite run with planning tools feel like a game? · Kill: pause/speed unused, runs end without a felt arc |
| **M2 — Variety & identity** | Splitter drone; slow/frost tower (role #3); entry/exit segments + difficulty topology (more fronts, not just bigger numbers); 2–3 mission maps; light unlock meta (minimal save) | Do the new roles/behaviors change plans, not just DPS? · Kill: a single dominant strategy remains |
| **M3 — Resolution & v1** | Epilog twist (revenge wave, own prototype slice); onboarding/help completion; balance pass on real telemetry; polish. v1 = playable from the repo (no store release) | Does the role reversal feel like a new perspective, not a foreign body? |

**Decisions (2026-10-02):** run = finite mission with an optional endless mode after
clear (M1 length 15–25 waves) · meta = light unlocks · towers = breadth first (wall →
slow) plus one small upgrade-path slice, gated by an ADR on the "placement only" rule ·
epilogue = own M3 slice (the run frame comes first).

**Working rules:** one prototype question + kill criterion per slice
(`docs/BACKLOG.md`); logic-first slices extract new `RefCounted` classes instead of
growing `game.gd`; balance claims come from the harness + telemetry (min. 8 harness +
3 human runs), not from feel; every milestone ends with a human playtest (3 Wow / 3 Meh)
and a readability check (P2).
