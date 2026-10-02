# Ideas (post-prototype)

Directions beyond the T01–T07 prototype. Nothing here is committed scope —
each item needs a slice decision (analyze → plan → implement) before it lands.
Captured so future sessions start from the maintainer's intent, not guesses.

## Multi-entry / multi-exit sides (maze pressure)

*Source: maintainer, 2026-10-02.*

Today: one spawn cell (left edge) → one base cell (right edge). Inspiration:
a variant where a **whole side** releases enemies scattered along it, and
enemies want to reach **another side** to leave; a map can have several
ins/outs. The point is to push the player into creative mazing — funneling
enemies through longer routes instead of just walling the direct line.

Sketch of the implications:

- `Maze` takes `entries: Array[Vector2i]` / `exits: Array[Vector2i]` instead
  of `spawn_cell` / `base_cell` (a side = its edge cells, or a segment).
- Connectivity rule becomes "every entry reaches at least one exit" (or all
  entries/exits share one component); validate with a multi-source BFS from
  all exits instead of a single `has_path()`.
- Drones pick the **nearest exit** at spawn/re-path time. A* per exit is
  cheap on 20×12; a BFS distance field is the scalable option.
- Spawn cell per drone comes from a **seeded** stream (derived from
  `GAME_SEED` + wave + index) to keep the replay determinism guarantee.
- Visuals: portal marker per entry cell, gate/band markers per exit cell
  (art pass later; placeholders can reuse `spawn`/`base`).
- Telemetry: `leak` events gain the exit cell; the analyzer columns stay.

Open questions for the slice decision:

- Side selection: fixed per map, or waves attack from different sides over
  time (escalation)?
- Entries/exits as full sides or configurable segments?
- Do exits stay "escape zones" (reaching one = leak, no home core), or does
  each exit keep a base core to defend?
- Balance: more exits = more escape points; leak pressure needs retuning
  (BALANCE.md is the map).

## Known open items (from the prototype docs)

- **Seed flow**: random seed per run (logged for replay) instead of the
  fixed `GAME_SEED = 1` — see `AGENTS.md`/`BALANCE.md`.
- **Skill tree**: `SkillStub` is a placeholder; real meta UI later, never
  in-match — see `AGENTS.md` "Meta".
- **More tower types / drone kinds**: extension points documented in
  `docs/ARCHITECTURE.md`.
- **Telemetry enrichment**: e.g. kill events or per-wave money snapshots if
  `tools/analyze_run.py` needs more than leak/build/wave/run_end.
