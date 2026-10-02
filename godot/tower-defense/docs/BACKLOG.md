# Backlog — next slices (post-T11)

Candidate queue for the workspace `/next` + `/loop` pipeline. An entry is a slice
*candidate* — `/analyze` still makes the slice decision (prototype question + kill
criterion, per `docs/VISION.md` and the workspace GAMEDEV guide).

**Item format** (read by `/next`; keep the annotation tokens on the item's first line):

`- [ ] **<task-id>** · priority: low|medium|high · effort: low|medium|large|xl · area: logic|visual — <goal>`

followed by indented `Vision:` (which pillar/prototype question), `Kill:` (what would end
it) and `Surfaces:` (expected files) lines.

- `area: logic` = backend/core work (loop-suitable); `area: visual` = needs the visual
  session (scenes, art, audio, HUD) — `/loop` filters these in backend-only mode.
- File order is the tie-break when priority and effort are equal.
- Done items: `- [x] **<task-id>** … — done YYYY-MM-DD`.

## Open

- [ ] **tower-defense-telemetry-enrichment** · priority: medium · effort: medium · area: logic — Add `kill` events (cell, kind, wave) and a per-wave money/kill/leak summary to the JSONL stream, and extend `tools/analyze_run.py` with kill-spread + money-curve tables.
  - Vision: question 2 (pressure curve needs its data basis first); P3.
  - Kill: after the data lands, no new actionable analyzer read is possible with it.
  - Surfaces: `scripts/telemetry.gd`, `scripts/game.gd`, `tools/analyze_run.py`, `tools/test_analyze_run.py`, `docs/ARCHITECTURE.md`, `docs/BALANCE.md`.
- [ ] **tower-defense-entry-exit-segments** · priority: medium · effort: medium · area: logic — Generalize T08 entries/exits from whole sides to `(side, cell range)` segments (default stays full side); validation, scatter and nearest-exit fallback stay deterministic.
  - Vision: P1 (the maze is the workbench — several fronts).
  - Kill: segments add config surface without enabling a new maze decision in a playtest.
  - Surfaces: `scripts/maze.gd`, `scripts/game.gd`, `tests/`, `docs/ARCHITECTURE.md`.
- [ ] **tower-defense-splitter-drone** · priority: medium · effort: large · area: logic — New drone kind "splitter": on death it spawns two weak children (seeded/deterministic); `WaveGen` + death handling + telemetry counting; placeholder sprite via `tools/make_placeholders.py` (sequence with the graphics-resolution work).
  - Vision: P3 (discovery — new behavior, not more hp).
  - Kill: reads as more quantity instead of a new decision; counterplay must stay open (no lock & key).
  - Surfaces: `scripts/drone.gd`, `scripts/wave.gd`, `scripts/game.gd`, `tools/make_placeholders.py`, `tests/`.

## Done

(none yet)

## Notes

- Loop clean-scope: `git status --porcelain -- godot/tower-defense` must be empty when a
  loop run starts; unrelated dirt elsewhere in `repos/Repository` (hot-chocolate PoC,
  repo-root docs) is ignored but never staged.
- This file + `context/tasks/tower-defense-*` are the record — the practice project has
  no GitHub issues (see `/next`'s "Project: tower-defense" profile).
- Ideas that still need a slice decision live in `docs/IDEAS.md` / `docs/VISION.md`;
  they graduate into this file when they are decision-ready.
