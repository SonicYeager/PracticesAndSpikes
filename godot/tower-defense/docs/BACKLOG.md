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
- [ ] **tower-defense-build-selector** · priority: low · effort: low · area: visual — Klickbarer Zwei-Slot-Selektor (KANONE/MAUER) im Build-Panel ersetzt den `B`-Toggle als Primär-Eingabe; Icon+Caption+Cost je Slot.
  - Vision: P1 (workbench) / M1; Kill: Selektor fügt Fläche hinzu, ohne eine Entscheidung zu ändern.
  - Surfaces: `scenes/Hud.tscn`, `scripts/hud.gd`, `tests/`.

## Done

- [x] **tower-defense-telemetry-enrichment** · priority: medium · effort: medium · area: logic — Add `kill` events (cell, kind, wave), a per-wave summary (kills/leaks/money start→end), `send`/`time_control` events, and a provenance marker in `run_start` (`source`, `harness` true/false); extend `tools/analyze_run.py`; move the legacy harness logs to `telemetry_local/legacy/` — done 2026-10-02 (`time_control` emission deferred to the TimeControl slice; the analyzer already accepts it).
- [x] **tower-defense-effect-budget** · priority: high · effort: low · area: visual — Playtest-Feedback: Aktion aufwerten, Karte beruhigen — done 2026-10-02 via `tower-defense-feedback-polish` (vent-only ambient; kill shockwave ring; build/sell/clear dust puff; leak edge flash; tracer trail deliberately skipped — muzzle/impact already cover shots, add later if it still feels quiet).
- [x] **tower-defense-removable-rocks** · priority: high · effort: medium · area: logic — Playtest-Feedback: Felsen/Geröll per Rechtsklick (15) entfernbar, Vents ausgenommen — done 2026-10-02 via `tower-defense-feedback-polish` (`Maze.clear_blocker`, BoardView sprite removal, dust puff, `clear` telemetry + analyzer, tests).
- [x] **tower-defense-loop-extension** · priority: high · effort: xl · area: logic — Playtest-Befund: mit zugebauter Map endet die Entscheidung — Richtung entschieden 2026-10-03 (Upgrades) und Upgrade-Slice geliefert via `tower-defense-map-boredom`/T16 (ADR 0012: `GunUpgrades` 5 Stufen, Delta-Kauf, Halb-Refund, Upgrade-Panel + Pips, `upgrade`-Telemetrie); Terrain-Disruptionen/Commander-Fähigkeiten bleiben IDEAS-Kandidaten.
- [x] **tower-defense-wall-piece** · priority: medium · effort: low · area: logic — Cheap wall blocker als zweite baubare Rolle — done 2026-10-03 via `tower-defense-wall-piece`/T17 (`Pieces`-Tabelle, `WALL_COST` 10, `B`-Toggle, `_walls`/`_wall_nodes`, HUD-Caption/Cost/Icon, `build/sell {kind}`-Telemetrie + Analyzer `walls`/`n(wm)`, Platzhalter `wall.png`; **kein ADR** — Breadth-first war in README/VISION entschieden, kein Musterwechsel). Kill bleibt Playtest-Frage (Substitution, nicht Nutzung).
- [x] **tower-defense-run-frame** · priority: medium · effort: medium · area: logic — Finite run frame — done 2026-10-03 via `tower-defense-run-frame`/T18 (`RunState`: Ziel-Welle 20, SIEG/GAME OVER, optionale Endlos-Verlängerung; Ein-`run_end`-Modell + `mission_cleared`; Run-Summary mit Kills/Leaks; Analyzer-Render-Formen + Aggregat-`cleared`-Fix). Kill offen: Playtest (fühlt sich der endliche Run wie ein Spiel an?).
- [x] **tower-defense-time-control** · priority: medium · effort: medium · area: logic — done 2026-10-04 via `tower-defense-time-control`/T19 (`TimeControl`: P Pause/T ×1–×3, `Engine.time_scale` + Paused-Guard, Planungs-Aktionen im Pause erlaubt, Wellenstart/Overcharge denied, HUD-Indicator + Button-Lock, Analyzer ` - time`-Marker). Surfaces: `scripts/time_control.gd`, `scripts/game.gd`, `scripts/hud.gd`/`scenes/Hud.tscn`, `tools/analyze_run.py`.

## Notes

- Loop clean-scope: `git status --porcelain -- godot/tower-defense` must be empty when a
  loop run starts; unrelated dirt elsewhere in `repos/Repository` (hot-chocolate PoC,
  repo-root docs) is ignored but never staged.
- This file + `context/tasks/tower-defense-*` are the record — the practice project has
  no GitHub issues (see `/next`'s "Project: tower-defense" profile).
- Ideas that still need a slice decision live in `docs/IDEAS.md` / `docs/VISION.md`;
  they graduate into this file when they are decision-ready. The tower-upgrade path
  shipped 2026-10-03 (ADR 0012, T16); the loop-extension candidates (terrain
  disruptions, commander abilities) remain in `docs/IDEAS.md`.
