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
- `art/STYLEGUIDE.md` — Ember Foundry palette, sprite specs + inventory.
- This file — decisions, slice status, commands, gotchas (keep it lean; put
  detail in the docs above instead of growing this list).

## Fixed decisions (Vision V1, 2026-09-21)

- **Grid**: variable map size (default 20×12, TILE 32 = 16px art at 2×).
- **Maze**: free building; any build that leaves an entry without a
  reachable exit is rejected with instant refund (multi-source BFS).
  Entry/exit tiles never buildable; sealed exits are inert.
- **Entries/exits** (T08): whole sides by default (`ENTRY_SIDES`/`EXIT_SIDES`
  in `game.gd`, `Side` enum — more sides later, segments are a future
  detail). Drones spawn scattered along the entries and draw a seeded random
  exit per spawn; re-path goes to the assigned exit, with a nearest-exit
  fallback when it is cut off. Exits are escape zones (reaching one = leak),
  no home core.
- **Run seed** (T09): random per run (`_random_seed()` from the clock),
  logged in `run_start.seed`; `seed_override` (>= 0) pins it for
  tests/editor/replay (default -1 = random).
- **Terrain / Dirty World** (T09): `TerrainGen.generate(size, entries,
  exits, seed)` places rock/rubble/vent blockers (solid, reserved, never
  sealing an entry — same greedy rule as builds) plus cosmetic decor.
  Battle decals: kill → scorch, leak → skid, hit → debris (cap 300, FIFO).
  Ambient emitters (vent → embers, crack → sparks, stain → smoke) are
  presentation-only (internal particle RNG, never gameplay).
- **Pathfinding**: `AStarGrid2D`, 4-directional (`DIAGONAL_MODE_NEVER`),
  plus a multi-source BFS (`Pathfinder.reachable_from`) for the build rule.
- **Economy**: money IS health. Kill +6, leak −10, gun 25, sell refund 12.
  Game over strictly below zero (`money < 0`, not `<= 0`).
- **Towers**: placement only — no in-match leveling. Gun first, only tower.
- **Combat** (T03): gun auto-fires at the drone closest to the base within
  range 3.5 cells, 8 dmg every 0.6 s, homing tracer (`Projectile`). Tracers
  spawn at the barrel muzzle (0.75 cells) with a muzzle flash. Drones walk
  the live path and re-path when the maze changes; building on a cell a
  drone currently occupies is rejected. Kinds via `Drone.KIND_MODS`
  (fast 0.6 hp/1.6 speed, tank 2.4 hp/0.55 speed).
- **Feedback** (juice pass): hit = white flash + impact spark + SFX, kill =
  explosion + SFX, leak/build/sell/denied/wave/game-over all have SFX.
  HP bars (22×3 px, green/yellow/red) float above every drone. Walk
  animation: normal/tank flip + waddle, fast rotates along the path.
  Spawn portal and base core pulse (scale) so the two stay distinct.
- **Audio**: generated SFX via `tools/make_sounds.py` (stdlib synth, no
  samples) into `audio/*.wav`; one `AudioStreamPlayer` per sound with
  `max_polyphony = 8`, volumes in `game.gd` `SFX_DB`. Hand sounds drop in
  under the same filenames, no code changes.
- **Waves**: endless + deterministic: `WaveGen.composition(n, seed)`,
  no unseeded RNG. Same seed + same builds = same run (replay via log).
  T04: waves auto-chain — after a wave is cleared, a `BREAK_SECONDS` (5 s)
  intermission runs with a HUD countdown; Space skips it. Wave 1 stays
  manual (build phase). Restart reuses the fixed seed until the seed flow
  lands (still open).
- **Game over** (T04): dimmed full-screen overlay with run summary (wave,
  money); restart via R or the button = `get_tree().reload_current_scene()`.
- **Polish** (T06): trauma-based screen shake on kill/leak/game-over
  (Camera2D offset, deterministic sine noise, still decays after game over),
  generated vignette overlay (`art/vignette.png`, linear filter), barrel
  recoil (3 px tween), muzzle scale-pop + rotation jitter, impact pop.
  Presentation-only, no RNG.
- **Meta**: skill tree is a stub (`SkillStub`, one dummy bonus,
  `user://skill_stub.cfg`). Real tree UI later, never in-match.
- **Telemetry**: local JSONL writer (`Telemetry`), event-based only
  (build/sell/wave/leak/run_end), no per-frame logging, flushed on game over
  and on window close. Analysis: `tools/analyze_run.py` (stdlib) — per-run
  wave tables + aggregate; scans `telemetry_local/`, then the Godot user dir.
- **Art**: Ember Foundry (see `art/STYLEGUIDE.md`): warm near-black ground,
  orange blocky gun, green drones with red eye, shape+color coding
  (round/fast-dart/wide-tank), textured floors. 16×16 (fx 8×8) with the
  Nearest filter from `project.godot`; the 640×360 vignette overlay sets
  Linear on its node. Placeholders generated by
  `tools/make_placeholders.py` (stdlib-only); hand art drops in under the
  same filenames, no code changes needed.

## Slice status

- Done: T01 scaffold, T02 maze+validation+click build/sell, GUT setup,
  16 placeholder sprites wired into Main, STYLEGUIDE, this file. T03 combat:
  `Drone`/`Gun`/`Projectile` core classes + tests, spawner on Space, drone
  walk/animation, barrel aiming (pivot via `Sprite2D.offset`), tracers,
  explosions, kill/leak economy, drone-aware build rejection, re-path on
  maze change, telemetry events (build/sell/wave/leak/run_end) flushed to
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
  Suite: 49 tests / 384 asserts green.
- Vision (2026-10-02): `docs/VISION.md` — pillars P1–P3 (workbench, living
  foundry, curiosity), values, anti-pillars; calibrations + next prototype
  questions (epilog twist, pressure curve, decal readability, meta, time
  control).
- Open: epilog twist prototype, pressure curve, decal readability, meta
  calibration, segments/more entry/exit sides, telemetry enrichment
  (see `docs/IDEAS.md`).

## Commands

Godot lives machine-local: `D:\Tools\godot\` (`%GODOT_BIN%` user env var).
Project has no CI; run from the project dir:

- Import: `godot --headless --path . --import --quit`
- Run: `godot --path .`
- Tests: `godot --headless --path . -s res://addons/gut/gut_cmdln.gd -gdir=res://tests -ginclude_subdirs -gexit`
  (+ `GODOT_DISABLE_LEAK_CHECKS=1` so exit code reflects tests, not leaks)
- Assets: `python tools/make_placeholders.py` (art), `python tools/make_sounds.py` (SFX)
- Analysis: `python tools/analyze_run.py [file ...]` (run logs → per-wave tables); parser tests: `python tools/test_analyze_run.py`

## Conventions

- Core logic as `class_name` RefCounteds (`Maze`, `Pathfinder`, `Economy`,
  `WaveGen`, `Drone`, `Gun`, `Projectile`, `Telemetry`, `SkillStub`) —
  unit-testable without scenes; grid-space coordinates, see ARCHITECTURE.md.
- GUT tests in `tests/test_*.gd`, pure asserts, no FS writes except
  `user://` (telemetry/skill tests if added).
- Test framework pinned: GUT 9.6.1 vendored in `addons/gut/` (9.7.1 exists,
  do not upgrade without running the suite).
- Git: `.godot/`, `reports/`, `telemetry_local/` ignored. Never commit
  `.godot/`. Keep commits scoped to `godot/` only.

## Gotchas

- `Object.is_connected()` exists natively — custom connectivity is named
  `has_path()`, never `is_connected` (fails as warning-as-error).
- GUT CLI works without enabling the editor plugin; editor shows it as
  disabled until one-click enable — expected, not broken.
- `const X := Economy.GUN_COST / 2` cross-class const expr is fine.
- Sub-repo worktree may contain unrelated dirty files (e.g. hot-chocolate
  PoC) — do not touch, do not bundle into commits.
- Visual QA: GUT's GUI panel covers the right half of the window, so a
  screenshot taken from a GUT run is cropped. For a full/zoomed capture,
  run a temporary `SceneTree` script instead (`godot --path . -s tools/x.gd`,
  `root.add_child(main_scene)`, `await process_frame` before touching nodes,
  save `root.get_texture().get_image()`); delete it afterwards.
