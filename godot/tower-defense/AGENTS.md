# Tower Defense — Project Agent Notes

Entry point for any session continuing this prototype. Repo-level rules in
`../../AGENTS.md` still apply (README convention, scoped commits, no secrets).

## What this is

Sci-Fi 2D maze tower defense, Godot 4.7.2 + GDScript (no .NET flow).
Location: `repos/Repository/godot/tower-defense/`, new top-level `godot/`
category (GDScript does not belong under `dotnet/godot/`, which is the C#
"Squash the Creeps" tutorial).

## Docs map

- `README.md` — human entry: what it is, controls, structure, roadmap.
- `docs/ARCHITECTURE.md` — classes, frame order, coordinates, invariants,
  determinism, test strategy, extension points.
- `docs/BALANCE.md` — every tunable constant + derived numbers + tuning flow.
- `docs/AUDIO.md` — sound list, synth recipes, volumes, replacing SFX.
- `docs/VISION.md` — workshop result: pillars, values, anti-pillars — the
  north star for every slice (check ideas against P1–P3 first).
- `docs/IDEAS.md` — post-prototype directions (candidates, not committed scope).
- `docs/BACKLOG.md` — next slices: candidate queue for the workspace `/next` +
  `/loop` pipeline (open/done, `area: logic|visual`, priorities).
- `art/STYLEGUIDE.md` — XT steel/lab palette, sprite specs + inventory.
- This file — decisions, slice status, commands, gotchas (keep it lean; put
  detail in the docs above instead of growing this list).

## Fixed decisions (Vision V1, 2026-09-21)

- **Grid**: variable map size (default 20×12, TILE 32 = 32 px art at 1:1).
- **Maze**: free building; any build that leaves an entry without a
  reachable exit is rejected with instant refund (multi-source BFS).
  Entry/exit tiles never buildable; sealed exits are inert.
- **Entries/exits** (T08/T21): `entry_segments`/`exit_segments` in `game.gd`
  (`SideSegments`: side + optional inclusive `from`/`to` range; defaults stay
  the full sides; empty configs fall back to the full side with a warning).
  Drones spawn scattered along the entry cells and draw a seeded random exit
  per spawn; re-path goes to the assigned exit, with a nearest-exit fallback
  when it is cut off. Exits are escape zones (reaching one = leak), no home
  core.
- **Run seed** (T09): random per run (`_random_seed()` from the clock),
  logged in `run_start.seed`; `seed_override` (>= 0) pins it for
  tests/editor/replay (default -1 = random).
- **Terrain / Dirty World** (T09): `TerrainGen.generate(size, entries,
  exits, seed)` places rock/rubble/vent blockers (solid, reserved, never
  sealing an entry — same greedy rule as builds) plus cosmetic decor.
  Battle decals: kill → scorch, leak → skid, hit → debris (cap 300, FIFO).
  Ambient emitters (vents only → embers; crack/stain stay static since the
  2026-10-02 playtest) are presentation-only (internal particle RNG, never
  gameplay). Rock/rubble clears for 15 via right-click (`Maze.clear_blocker`);
  vents stay.
- **Pathfinding**: `AStarGrid2D`, 4-directional (`DIAGONAL_MODE_NEVER`),
  plus a multi-source BFS (`Pathfinder.reachable_from`) for the build rule.
- **Economy**: money IS health. Kill +6, leak −10, gun 25 (= level-1 price),
  wall 10, rock clear 15; selling refunds half (gun: `GunUpgrades.refund` per
  level; wall: `WALL_COST / 2`). Game over strictly below zero (`money < 0`, not `<= 0`).
- **Towers**: placement first; in-match upgrades since ADR 0012 (five-level
  `GunUpgrades` table, cumulative prices, delta buys, half refund, L5 = LANZE).
  Gun first; wall piece (T17: 10, blocker, no attack, `B` toggles the build
  kind) — the gun stays the only tower.
- **Combat** (T03): gun auto-fires at the drone closest to the base within
  its level-based range (L1: 3.5 cells, 8 dmg / 0.6 s; T16/ADR 0012), homing
  tracer (`Projectile`, carries its damage). Tracers
  spawn at the barrel muzzle (0.75 cells) with a muzzle flash. Drones walk
  the live path and re-path when the maze changes; building on a cell a
  drone currently occupies is rejected. Kinds via `Drone.KIND_MODS`
  (fast 0.6 hp/1.6 speed, tank 2.4 hp/0.55 speed). Splitter (T22): 1.0/0.85,
  splits on death into two children (0.4/1.25) at the death cell with the
  parent's exit; no split on leak or at the doorstep (1-cell path); children
  pay `Economy.CHILD_KILL_REWARD` (2).
- **Feedback** (juice pass + 2026-10-02 polish): hit = white flash + impact
  spark + SFX, kill = explosion + shockwave ring + SFX, build/sell/clear =
  dust puff, leak = red edge flash + SFX, denied/wave/game-over all have SFX.
  HP bars (22×3 px, green/yellow/red) float above every drone. Walk
  animation: normal/tank flip + waddle, fast rotates along the path.
  Spawn portal and base core pulse (scale) so the two stay distinct.
- **Audio**: generated SFX via `tools/make_sounds.py` (stdlib synth, no
  samples) into `audio/*.wav`; one `AudioStreamPlayer` per sound with
  `max_polyphony = 8`, volumes in `game.gd` `SFX_DB`. Hand sounds drop in
  under the same filenames, no code changes.
- **Waves**: deterministic: `WaveGen.composition(n, seed)`, no unseeded RNG.
  Same seed + same builds = same run (replay via log). Finite mission (T18):
  clearing the goal wave (`RunState.DEFAULT_GOAL` 20) wins; the win screen
  offers WEITER (endless segment) or restart. T04: waves auto-chain — after a
  wave is cleared, a `BREAK_SECONDS` (5 s) intermission runs with a HUD
  countdown; Space skips it. Wave 1 stays manual (build phase); restart draws
  a fresh run seed (T09).
- **Run end** (T04/T18): dimmed full-screen overlay with the run summary
  (wave, money, kills, leaks); title SIEG on mission clear, GAME OVER
  otherwise; restart via R or the button = `get_tree().reload_current_scene()`;
  after a win, WEITER continues into the endless segment.
- **Time control** (T19): `TimeControl` — P pauses, T cycles ×1/×2/×3 via
  `Engine.time_scale`; planning actions (build/sell/clear/upgrade/select) stay
  available while paused, wave start and vent overcharge are denied; the HUD
  shows PAUSE/×N and locks the wave button; `time_control` telemetry per
  change; `_ready` resets the scale.
- **Polish** (T06): trauma-based screen shake on kill/leak/game-over
  (Camera2D offset, deterministic sine noise, still decays after game over),
  generated vignette overlay (`art/vignette.png`, linear filter), barrel
  recoil (3 px tween), muzzle scale-pop + rotation jitter, impact pop.
  Presentation-only, no RNG.
- **Pulse** (T10): wave modifiers from wave 3 (40% chance, deterministic in
  `WaveGen`: rush/swarm/blackout/bounty), telegraphed in the HUD (running +
  break preview) and logged in the wave event; vent overcharge as money sink
  (20 money, 15 dmg in 2.5 cells, 6 s cooldown, ready-glow); decal per-cell
  cap (2) keeps kill zones readable. Playtest note: the bright "decal smears"
  were a fast-forward harness artifact (un-faded effects), not real decals.
- **Meta**: skill tree is a stub (`SkillStub`, one dummy bonus,
  `user://skill_stub.cfg`). Real tree UI later, never in-match.
- **Telemetry**: local JSONL writer (`Telemetry`), event-based only
  (build/sell/clear/upgrade/wave/leak/kill/send + wave summaries; build/sell
  carry `kind`; `mission_cleared` marks the win, `run_end` carries
  `result`/`endless`, `time_control` logs pause/speed, `harness_start`/`harness_end` mark tool runs), no per-frame logging, flushed on run end
  and on window close. Analysis: `tools/analyze_run.py` (stdlib) — per-run
  wave tables + aggregate; scans `telemetry_local/`, then the Godot user dir.
- **Art (T11/T14)**: XT steel/lab look (see `art/STYLEGUIDE.md`): dark
  blue-grey metal tiles, plasma-cannon turrets (side prongs, blue mount, cyan
  core), bug-style drones (head + legs, red eyes; white/acid/grey-blue coded),
  textured floors, hazard-strip containment base. UI font VT323 (OFL,
  `fonts/`) via `gui/theme/custom_font`, HUD panels dark blue/steel, gold
  money. 32×32 (fx 16×16, ring 32×32) with the Nearest filter from
  `project.godot`; the 640×360 vignette/leak-flash overlays set Linear on
  their nodes. Placeholders generated
  by `tools/make_placeholders.py` (stdlib-only, XT palette); hand art drops
  in under the same filenames, no code changes needed. Study reference
  (`reference/xt/`) is gitignored, never shipped.

## Slice status

- Done: T01 scaffold, T02 maze+validation+click build/sell, GUT setup,
  16 placeholder sprites wired into Main, STYLEGUIDE, this file. T03 combat:
  `Drone`/`Gun`/`Projectile` core classes + tests, spawner on Space, drone
  walk/animation, barrel aiming (pivot via `Sprite2D.offset`), tracers,
  explosions, kill/leak economy, drone-aware build rejection, re-path on
  maze change, telemetry events (build/sell/wave/leak/kill/send + wave_end
  summaries) flushed to
  `user://run_<seed>.jsonl`, minimal game-over label. Juice pass: 9
  synthesized SFX (`tools/make_sounds.py` → `audio/`), muzzle-tip tracers +
  flash, hit spark/flash, HP bars, walk animation, spawn/base pulse + art
  redesign, HUD icon panel (`tools/make_placeholders.py` icons).
  Suite: 30 tests / 91 asserts green (incl. a scene integration smoke test).
  T04: break countdown + auto-chaining (Space skips), game-over overlay
  (run summary, restart via scene reload, button wired in `Main.tscn`).
  Phase enum refactor (IDLE/RUNNING/BREAK/GAME_OVER). Suite: 33 tests / 104
  asserts green. T05: `tools/analyze_run.py` — per-wave leak/kill/money/
  build tables + aggregate, discovery of `telemetry_local/` then the user
  dir, 10 stdlib regression tests (`tools/test_analyze_run.py`); verified
  against a real wave-7 game-over run. T06: trauma screen shake (Camera2D,
  deterministic sines, decays during game over), vignette overlay
  (`vignette.png` via `make_placeholders.py`), barrel recoil, muzzle/impact
  pops. Suite: 34 tests / 111 asserts green. T07: docs pass — roadmap
  closed, `docs/IDEAS.md` collects post-prototype directions. T08:
  whole-side entries/exits, seeded scatter spawn, assigned exit + fallback,
  per-entry reachability validation, route preview fix (floor z-index +
  redraw). Suite: 40 tests / 145 asserts green. T09: random run seed +
  `seed_override`, `TerrainGen` blockers/decor (greedy, never sealing),
  battle decals (scorch/skid/debris, cap 300), ambient emitters.
  Suite: 49 tests / 384 asserts green. T10 (Pulse): wave modifiers
  (rush/swarm/blackout/bounty, HUD-telegraphed, logged in the wave event),
  vent overcharge (money sink with ember burst + ready-glow), decal
  per-cell cap (2), overcharge SFX. Suite: 58 tests / 472 asserts green.
  T11 (XT-Pass): XT steel/lab look (palette, sprites, UI font VT323, drier
  shoot/hit/kill/denied SFX), reference study gitignored. T12 (HUD):
  `scenes/Hud.tscn` + `scripts/hud.gd` (status/build panels, wave bar with
  start button, sell toggle, game-over overlay); `game.gd` pushes state, the
  HUD emits intents. T12.5 (Refactor): the wave flow (phase, queue, timers,
  modifier knobs) lives in `scripts/wave_director.gd`; the scene orchestrates.
  T13 (Art): all sprites redrawn at 32×32 (fx 16×16) with the procedural
  generator, displayed 1:1 (`ART_SCALE` 1). T13.5 (Refactor): board rendering
  (floor/terrain/markers/decals/ambient/routes) + grid math moved into
  `scripts/board_view.gd`; the scene pushes routes and vent state in.
  Suite: 68 tests / 510 asserts green. T14: plasma-cannon turret + bug-style
  drones (generator redraw) and combat FX extracted into `scripts/fx.gd`
  (shake, muzzle/impact/explosion, ember bursts, recoil). T15 (Telemetry):
  kill/send/wave_end events + run_start provenance; the analyzer renders exact
  kills, money start→end and kill zones (legacy logs stay readable). Suite:
  75 tests / 533 asserts green. Polish (2026-10-02, playtest feedback):
  vent-only ambient, kill ring/build puff/leak flash, rocks clearable via
  right-click (15); suite: 84 tests / 595 asserts green. T16 (Upgrades,
  2026-10-03, ADR 0012): `GunUpgrades` five-level path (cumulative prices,
  delta buys, half refund), selection + UPGRADE panel, level pips/signature
  tint, `upgrade` telemetry; suite: 103 tests / 710 asserts green. T17 (Wall,
  2026-10-03): wall piece (10, blocker, no attack) as the second buildable
  role, `B` toggles the build kind, `Pieces` kind table, `build/sell {kind}`
  telemetry + analyzer `walls`; suite: 115 tests / 761 asserts green. T18
  (Run-Frame, 2026-10-03): finite mission — goal wave 20 (`RunState`), SIEG/
  GAME OVER, `mission_cleared` + `run_end {result,endless}`, win screen with
  WEITER (endless segment), run summary (kills/leaks); suite: 125 tests / 821
  asserts green. T19 (Time control, 2026-10-04): `TimeControl` (P pause,
  T ×1/×2/×3), Engine.time_scale + explicit paused guard, planning actions
  stay available, wave start/overcharge denied, HUD indicator + wave-button
  lock, analyzer `time` marker; suite: 136 tests / 875 asserts green. T20
  (Balance harness, 2026-10-04): `HarnessRun`-Kern + headless `tools/harness.gd`
  (Seed + Pre-Wave-1-Builds, Cap/Guard, Auto-Endlos, Determinismus- und
  Log-vs-Memory-Invarianten, `harness_start`/`harness_end`, Analyzer-Render +
  Aggregat); suite: 143 tests / 910 asserts green. T21 (Entry/exit segments,
  2026-10-04): `SideSegments` (side + inclusive range → deterministic cell
  lists), `@export` `entry_segments`/`exit_segments` (defaults full sides),
  empty-config guard with full-side fallback; suite: 156 tests / 985 asserts
  green. T22 (Splitter drone, 2026-10-04): `KIND_MODS` splitter/child, WaveGen
  `splitters` from wave 8 (draw after the modifier roll), Director queue,
  `_spawn_split_children` with doorstep guard + `_add_drone` refactor,
  `CHILD_KILL_REWARD`, leak `kind` + analyzer `kinds` line, placeholder
  sprites; suite: 166 tests / 1035 asserts green.
- Vision (2026-10-02): `docs/VISION.md` — pillars P1–P3 (workbench, living
  foundry, curiosity), values, anti-pillars; calibrations + next prototype
  questions (epilog twist, pressure curve, decal readability, meta, time
  control, loop maintenance).
- Open: epilog twist prototype, meta calibration, pressure tuning via
  telemetry (harness available, T20), wave escalation to extra sides (see `docs/IDEAS.md`;
  candidate queue with priorities: `docs/BACKLOG.md`).

## Commands

Godot lives machine-local: `D:\Tools\godot\` (`%GODOT_BIN%` user env var).
Project has no CI; run from the project dir:

- Import: `godot --headless --path . --import --quit`
- Run: `godot --path .`
- Tests: `godot --headless --path . -s res://addons/gut/gut_cmdln.gd -gdir=res://tests -ginclude_subdirs -gexit`
  (+ `GODOT_DISABLE_LEAK_CHECKS=1` so exit code reflects tests, not leaks)
- Assets: `python tools/make_placeholders.py` (art), `python tools/make_sounds.py` (SFX)
- Analysis: `python tools/analyze_run.py [file ...]` (run logs → per-wave tables); parser tests: `python tools/test_analyze_run.py`
- Balance harness: `godot --headless --path . -s tools/harness.gd -- --seed 7 --waves 30 --towers "9,3;10,3"` (add `--repeat 2`, `--out reports/x.jsonl`)

## Conventions

- Core logic as `class_name` RefCounteds (`Maze`, `Pathfinder`, `Economy`,
  `WaveGen`, `RunState`, `Drone`, `Gun`, `GunUpgrades`, `Pieces`, `Projectile`, `Telemetry`, `SkillStub`) —
  unit-testable without scenes; grid-space coordinates, see ARCHITECTURE.md.
- GUT tests in `tests/test_*.gd`, pure asserts, no FS writes except
  `user://` (telemetry/skill tests if added).
- Test framework pinned: GUT 9.6.1 vendored in `addons/gut/` (9.7.1 exists,
  do not upgrade without running the suite).
- Git: `.godot/`, `reports/`, `telemetry_local/` ignored. Never commit
  `.godot/`. Keep commits scoped to `godot/` only.
- Reference material: `reference/` (gitignored) holds study-only material —
  e.g. the extracted Xeno Tactic SWF (`reference/xt/`) for look/sound
  reference. Never ship or commit it; own assets stay self-generated.

## Gotchas

- `Object.is_connected()` exists natively — custom connectivity is named
  `has_path()`, never `is_connected` (fails as warning-as-error).
- GUT CLI works without enabling the editor plugin; editor shows it as
  disabled until one-click enable — expected, not broken.
- `const X := Economy.GUN_COST / 2` cross-class const expr is fine.
- Sub-repo worktree may contain unrelated dirty files (e.g. hot-chocolate
  PoC) — do not touch, do not bundle into commits.
- Visual QA: GUT's GUI panel covers the right half of the window, so a
  screenshot taken from a GUT run is cropped. Use the reusable tool instead
  (windowed — headless runs are refused):
  `Godot --path . -s tools/shot.gd -- --states idle,running --towers "8,3;9,6" --spawn fast,tank`
  → writes `<out-prefix>_<state>.png` (default prefix `reports/shot`), logs
  `SHOT <path> <WxH>`; full flag list in the script header. Bad args/states
  fail loudly (exit 1).
- Windows: run the Python tools as `pythonw tools/<script>.py > <temp>/out.txt 2>&1`
  — bare `python` from agent shells flashes terminal windows on the user's
  desktop (workspace rule in `../../AGENTS.md`).
