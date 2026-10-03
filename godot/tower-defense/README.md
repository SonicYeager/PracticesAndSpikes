# tower-defense

## About

Sci-Fi 2D maze tower defense prototype (GDScript, Godot 4.7.2). Free maze
building with AStarGrid2D pathfinding, money-is-HP economy (game over below
zero), a finite mission (goal wave 20) with an endless mode, meta skill-tree
stub, local JSONL telemetry for data-driven balancing.

- Stack: Godot 4.7.2, GDScript (no .NET flow)
- Entrypoint: `scenes/Main.tscn` (open/import the folder in the Godot editor)
- Status: prototype complete + sides + dirty world + pulse + XT look + HUD + telemetry + upgrades + wall + run frame + time control (T01–T19) — see *Controls* and *Roadmap*

## Controls

| Input | Action |
|---|---|
| Left click | Build a gun (25) or wall (10) — rejected builds refund instantly |
| B | Toggle the build kind (KANONE ↔ MAUER); the HUD slot follows (caption, cost, icon) |
| P | Pause/resume — planning actions stay available |
| T | Cycle speed ×1 → ×2 → ×3 |
| Left click on a tower | Select it; the HUD shows name, next level and an UPGRADE button (ESC deselects) |
| UPGRADE (HUD) | Upgrade the selected tower one level (delta price; level 5 = LANZE) |
| Left click on a vent | Overcharge (20): ember burst, 15 dmg in 2.5 cells, 6 s cooldown |
| Right click | Sell a gun or wall (refund = half; L1: 12, wall: 5) · clear a rock/rubble blocker for 15 (vents stay); the HUD VERKAUFEN toggle sells via left click while active |
| Space (or the wave button) | Start wave 1 / skip the break between waves |
| R (or the button) | Restart after game over |
| WEITER (win screen) | Continue into the endless segment after clearing the mission |

Rules: money **is** health — kills earn +6, leaks cost −10, game over strictly
below zero. Clearing the goal wave (20) wins the run; the win screen can
continue into endless. Selling refunds half of the cumulative tower invest
(level 1 → 12;
wall → 5). Walls block like guns but never shoot.
Builds that would leave an entry without a reachable exit (or
land on a tile a drone currently occupies) are rejected. Drones re-route when
the maze changes. Rock/rubble blockers can be cleared for 15 (right click);
vents stay.
After a wave is cleared, a 5 s break runs (`BREAK_SECONDS`); the next wave
then auto-starts. The end screen shows a run summary (win or loss) and
restarts the scene; a win can continue endless.

## Features (current)

- Variable grid (20×12, TILE 32), free maze building, live route preview of
  active drones.
- 4-directional AStarGrid2D pathfinding, no corner slipping.
- Deterministic waves: `WaveGen.composition(n, seed)`, no unseeded
  RNG anywhere in gameplay — same seed + same builds = same run.
- Wave flow: auto-chaining after a 5 s break with HUD countdown (Space
  skips); game-over screen with run summary + restart.
- Gun auto-targeting (closest to base; L1: 3.5 cells, 8 dmg / 0.6 s,
  level-scaled) with homing tracers fired from the barrel muzzle.
- Upgrades (T16, ADR 0012): five-level gun path (KANONE → LANZE) with
  cumulative prices and delta buys; click a tower, then UPGRADE in the HUD;
  level pips + signature tint.
- Wall piece (T17): a 10-money blocker with no attack — cheap maze shaping;
  `B` toggles the build kind and the HUD slot follows (caption, cost, icon).
- Finite mission (T18): clear the goal wave (20) to win — SIEG screen with
  the run summary (wave, money, kills, leaks); WEITER continues endless.
- Time control (T19): P pauses (planning actions stay available), T cycles
  ×1 → ×2 → ×3; the HUD shows PAUSE/×N and the wave button locks while paused.
- Three drone kinds (shape + color coded): normal, fast, tank.
- Feedback: 10 synthesized SFX, muzzle flash, hit sparks, explosions +
  kill shockwave ring, build/sell/clear dust puffs, leak edge flash, HP bars,
  walk animation, HUD icon panel, pulsing spawn portal/base bunker.
- Polish (T06): trauma-based screen shake (Camera2D, deterministic sines),
  vignette overlay, barrel recoil, muzzle/impact pops.
- Sides (T08): enemies enter scattered along the left side and each draws a
  seeded random exit on the right side; reaching an exit is a leak. Builds
  must keep every entry connected to at least one exit.
- Dirty World (T09): random run seed (logged for replay), seeded terrain
  blockers + decor, battle decals (scorch/skid/debris) that accumulate
  during the run, vent embers only (crack/stain stay static — 2026-10-02
  playtest); rock/rubble is clearable for 15.
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
- T14: plasma-cannon turret (side prongs, cyan core) and bug-style drones
  (head + legs, 2-frame gait); combat FX (shake, muzzle/impact/explosion,
  ring/puff, ember bursts, recoil) live in `scripts/fx.gd`.
- Local telemetry: build/sell/clear/upgrade/wave/leak/kill/send/time_control events +
  per-wave summaries (`wave_end`), `mission_cleared` and `run_end`
  (`result`/`endless`) → `user://run_<seed>.jsonl` (analysis:
  `tools/analyze_run.py`); `run_start` carries provenance (`source`,
  `harness`) so harness runs stay distinguishable from human ones.
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

GUT 9.6.1 is vendored under `addons/gut/`. The suite (136 tests / 875 asserts)
covers every core class plus one scene integration smoke test that steps the
real `Main.tscn` (spawn → walk → shoot → kill with scattered entries and
terrain, break → auto-chain, game over + log flush, restart wiring, camera
shake decay, scatter determinism, nearest-exit fallback, overcharge,
modifiers, decal caps, terrain clearing, build/sell FX wiring, upgrades +
selection, wall builds, run frame, time control).

## Project structure

| Path | Purpose |
|---|---|
| `scenes/Main.tscn` | Root scene: game node, camera, vignette + HUD instance |
| `scenes/Hud.tscn` | HUD layout: status, build/sell panel, wave bar, game-over overlay |
| `scripts/hud.gd` | HUD state API + intents (wave/sell/restart signals), chip colors |
| `scripts/hud_bar.gd` | Segmented wave-progress bar |
| `scripts/game.gd` | Scene controller: input, orchestration, sprites, effects, audio, HUD push |
| `scripts/wave_director.gd` | Wave flow state machine: phase, queue, timers, modifier knobs |
| `scripts/run_state.gd` | Finite-run frame: goal, result, endless flag, summary counters (T18) |
| `scripts/time_control.gd` | Pause/speed state: `SPEEDS`, scale, toggle/cycle (T19) |
| `scripts/board_view.gd` | Board rendering: floor/terrain/markers/decals, ambient, route preview, grid math |
| `scripts/fx.gd` | Combat FX: screen shake, muzzle/impact/explosion, ring/puff, ember bursts, recoil |
| `scripts/maze.gd` | Buildable grid + connectivity validation (`Maze`) |
| `scripts/pathfinder.gd` | AStarGrid2D wrapper, 4-directional (`Pathfinder`) |
| `scripts/economy.gd` | Money-is-HP rules (`Economy`) |
| `scripts/wave.gd` | Deterministic wave composition (`WaveGen`) |
| `scripts/terrain_gen.gd` | Seeded terrain dressing: blockers + decor (`TerrainGen`) |
| `scripts/drone.gd` | Grid-space walker: path, hp, kinds (`Drone`) |
| `scripts/gun.gd` | Tower targeting + level stats (`Gun`) |
| `scripts/gun_upgrades.gd` | Five-level upgrade table (prices/stats/refunds, ADR 0012) |
| `scripts/pieces.gd` | Buildable kinds (cost/label/telemetry mapping, T17) |
| `scripts/projectile.gd` | Homing tracer (`Projectile`) |
| `scripts/telemetry.gd` | JSONL event writer (`Telemetry`) |
| `scripts/skill_stub.gd` | Meta-progression stub (`SkillStub`) |
| `tests/` | GUT tests (`test_*.gd`) |
| `art/` | Placeholder sprites + `STYLEGUIDE.md` |
| `audio/` | Synthesized SFX (generated by `tools/make_sounds.py`) |
| `tools/` | stdlib-only Python tools (asset generators, telemetry analysis) + `tools/shot.gd` (visual-QA screenshots) |

## Assets

Both asset sets are generated by stdlib-only Python scripts and can be
replaced by hand-made files under the same names — no code changes needed:

```bash
python tools/make_placeholders.py   # art/*.png (sprites + vignette + flash edge)
python tools/make_sounds.py         # audio/*.wav (22050 Hz mono)
```

## Telemetry analysis

`tools/analyze_run.py` (stdlib only) reads the run logs and prints per-wave
kill/leak/money/build tables plus an aggregate across runs:

```bash
python tools/analyze_run.py                      # scans telemetry_local/, then the Godot user dir
python tools/analyze_run.py path/to/run_1.jsonl  # or explicit files / directories
python tools/analyze_run.py telemetry_local/legacy   # pre-provenance runs live here
```

Kills are exact — from `kill` events and `wave_end` summaries, not a
`count − leaks` estimate — the money column shows each wave's `start->end`, and
a `kill zones` line names the top kill cells. Legacy logs without the newer
events keep the old derivation, and a non-local `source` shows as a
`[harness]` marker in the header.

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
| T13 | Art pass: all sprites 32×32 at 1:1, fx 16×16 | done |
| T13.5 | Refactor: extract BoardView (board rendering + grid math) | done |
| T14 | Turret & enemy redesign, Fx extraction | done |
| T15 | Telemetry enrichment: kill/send/wave_end events, run provenance, analyzer | done |
| T16 | Upgrades: five-level `GunUpgrades` path, panel/pips, `upgrade` telemetry (ADR 0012) | done |
| T17 | Wall piece: second buildable role, `B` toggle, `Pieces` table, analyzer `walls` | done |
| T18 | Run frame: goal wave 20, SIEG/GAME OVER, endless continue, `mission_cleared` | done |
| T19 | Time control: `TimeControl`, P pause, T ×1–×3, planning while paused, HUD indicator | done |

All slices done (T01–T19). Post-prototype directions are collected in `docs/IDEAS.md`;
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
slow) plus the upgrade path (T16, ADR 0012 — shipped 2026-10-03) ·
epilogue = own M3 slice (the run frame comes first).

**Working rules:** one prototype question + kill criterion per slice
(`docs/BACKLOG.md`); logic-first slices extract new `RefCounted` classes instead of
growing `game.gd`; balance claims come from the harness + telemetry (min. 8 harness +
3 human runs), not from feel; every milestone ends with a human playtest (3 Wow / 3 Meh)
and a readability check (P2).
