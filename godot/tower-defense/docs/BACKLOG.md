# Backlog — next slices (post-T11)

Candidate queue for the workspace `/next` + `/loop` pipeline; milestone mapping lives in
the README ("From prototype to game"). An entry is a slice *candidate* — `/analyze` still
makes the slice decision (prototype question + kill criterion, per `docs/VISION.md` and
the workspace GAMEDEV guide).

**Item format** (read by `/next`; keep the annotation tokens on the item's first line):

`- [ ] **<task-id>** · priority: low|medium|high · effort: low|medium|large|xl · area: logic|visual — <goal>`

followed by indented `Vision:` (pillar/prototype question + roadmap milestone), `Kill:`
(what would end it) and `Surfaces:` (expected files) lines.

- `area: logic` = backend/core work (loop-suitable); `area: visual` = needs the visual
  session (scenes, art, audio, HUD) — `/loop` filters these in backend-only mode.
- File order is the tie-break when priority and effort are equal.
- Logic-first rule (challenger 2026-10-02): slices extract new `RefCounted` classes
  instead of growing `game.gd`; visual integration stays a separate, smaller step.
- Done items: `- [x] **<task-id>** … — done YYYY-MM-DD`.

## Open

- [ ] **tower-defense-wall-piece** · priority: medium · effort: low · area: logic — Cheap wall blocker (no attack) as the second buildable role; build/economy logic + placeholder sprite + tests.
  - Vision: P1 (workbench); roadmap M1 (tower breadth first).
  - Kill: the wall adds no new decision (just a cheaper gun stand-in).
  - Surfaces: `scripts/economy.gd`, `scripts/game.gd`, `tools/make_placeholders.py`, `tests/`.
- [ ] **tower-defense-run-frame** · priority: medium · effort: medium · area: logic — Finite run frame: wave goal (M1: 15–25), win/lose states, `run_end{result}`, run-summary data; extracted as `RunState`/`WaveDirector` RefCounted classes (not more `game.gd`).
  - Vision: D1 decision (mission + optional endless after clear); roadmap M1.
  - Kill: a finite run still doesn't feel like a game → revisit endless/meta.
  - Surfaces: `scripts/wave_director.gd` (exists), new `scripts/run_state.gd`, `scripts/game.gd` (thin integration), `scripts/telemetry.gd`, `tests/`.
- [ ] **tower-defense-time-control** · priority: medium · effort: medium · area: logic — Pause/speed as a `TimeControl` RefCounted: engine time scale, actions allowed while paused, determinism preserved; tests.
  - Vision: VISION Q5 (Total Time Control); roadmap M1.
  - Kill: pause/speed goes unused in playtests → simplify.
  - Surfaces: new `scripts/time_control.gd`, `scripts/wave_director.gd` (break/spawn timing), `scripts/game.gd` (thin integration), `tests/`.
- [ ] **tower-defense-balance-harness** · priority: medium · effort: medium · area: logic — Headless fast-forward: pinned seed (`seed_override`) + build script, N waves, invariant + metric output (leaks/money/kills); basis for controlled balance questions.
  - Vision: pressure tuning via telemetry (VISION Q2); data-reality plan step 3.
  - Kill: harness results diverge from manual runs → fix or drop.
  - Surfaces: new `tools/` script + tests, thin `scripts/game.gd` headless hooks if needed.
- [ ] **tower-defense-entry-exit-segments** · priority: medium · effort: medium · area: logic — Generalize T08 entries/exits from whole sides to `(side, cell range)` segments (default stays full side); validation, scatter and nearest-exit fallback stay deterministic.
  - Vision: P1 (workbench — several fronts); roadmap M2 (difficulty topology).
  - Kill: segments add config surface without enabling a new maze decision in a playtest.
  - Surfaces: `scripts/maze.gd`, `scripts/game.gd`, `tests/`, `docs/ARCHITECTURE.md`.
- [ ] **tower-defense-splitter-drone** · priority: medium · effort: large · area: logic — New drone kind "splitter": on death it spawns two weak children (seeded/deterministic); `WaveGen` + death handling + telemetry counting; placeholder sprite via `tools/make_placeholders.py` (sequence with the graphics-resolution work).
  - Vision: P3 (discovery — new behavior, not more hp); roadmap M2.
  - Kill: reads as more quantity instead of a new decision; counterplay must stay open (no lock & key).
  - Surfaces: `scripts/drone.gd`, `scripts/wave.gd`, `scripts/game.gd`, `tools/make_placeholders.py`, `tests/`.
- [ ] **tower-defense-loop-extension** · priority: high · effort: xl · area: logic — Playtest-Befund: mit zugebauter Map endet die Entscheidung; Kandidaten (wellen-getriebene Terrain-Disruptionen, Commander-Fähigkeiten, Upgrades) in `docs/IDEAS.md` + `context/tasks/tower-defense-map-boredom/analysis.md` — **Richtungsentscheidung (VISION-Frage 6) steht vor dem Slice**.
  - Vision: P1/P3 (Frage 6 „Loop-Erhalt nach dem Zubauen").
  - Kill: drei Wellen ohne neue Re-Umplanung → Ansatz verwerfen.
  - Surfaces: neuer `RefCounted`-Director + `scripts/game.gd` (dünn), `scripts/board_view.gd`, `tests/`.

## Done

- [x] **tower-defense-telemetry-enrichment** · priority: medium · effort: medium · area: logic — Add `kill` events (cell, kind, wave), a per-wave summary (kills/leaks/money start→end), `send`/`time_control` events, and a provenance marker in `run_start` (`source`, `harness` true/false); extend `tools/analyze_run.py`; move the legacy harness logs to `telemetry_local/legacy/` — done 2026-10-02 (`time_control` emission deferred to the TimeControl slice; the analyzer already accepts it).
- [x] **tower-defense-effect-budget** · priority: high · effort: low · area: visual — Playtest-Feedback: Aktion aufwerten, Karte beruhigen — done 2026-10-02 via `tower-defense-feedback-polish` (vent-only ambient; kill shockwave ring; build/sell/clear dust puff; leak edge flash; tracer trail deliberately skipped — muzzle/impact already cover shots, add later if it still feels quiet).
- [x] **tower-defense-removable-rocks** · priority: high · effort: medium · area: logic — Playtest-Feedback: Felsen/Geröll per Rechtsklick (15) entfernbar, Vents ausgenommen — done 2026-10-02 via `tower-defense-feedback-polish` (`Maze.clear_blocker`, BoardView sprite removal, dust puff, `clear` telemetry + analyzer, tests).

## Notes

- Loop clean-scope: `git status --porcelain -- godot/tower-defense` must be empty when a
  loop run starts; unrelated dirt elsewhere in `repos/Repository` (hot-chocolate PoC,
  repo-root docs) is ignored but never staged.
- This file + `context/tasks/tower-defense-*` are the record — the practice project has
  no GitHub issues (see `/next`'s "Project: tower-defense" profile).
- Ideas that still need a slice decision live in `docs/IDEAS.md` / `docs/VISION.md`;
  they graduate into this file when they are decision-ready. The tower-upgrade path
  (hybrid direction) needs an ADR on the "placement only" rule before its slice starts.
