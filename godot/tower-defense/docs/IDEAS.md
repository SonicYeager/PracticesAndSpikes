# Ideas (post-prototype)

Directions beyond the T01–T07 prototype. Nothing here is committed scope —
each item needs a slice decision (analyze → plan → implement) before it lands.
Captured so future sessions start from the maintainer's intent, not guesses.

## Multi-entry / multi-exit sides (maze pressure)

*Source: maintainer, 2026-10-02. Design decisions taken the same day — ready
to be built as the next slice.*

Today: one spawn cell (left edge) → one base cell (right edge). Target: a
**whole side** releases enemies scattered along it, and enemies want to reach
**another side** to leave; a map can have several ins/outs. The point is to
push the player into creative mazing — funneling enemies through longer
routes instead of just walling the direct line.

**Decided:**

- `Maze` takes `entries: Array[Vector2i]` / `exits: Array[Vector2i]`; the
  default map keeps one side each (left in, right out), the code stays
  generic for more sides later. Wave escalation to extra sides is a separate
  feature.
- Exits are **escape zones**: reaching one = leak, no home core. Money-is-HP
  is unchanged.
- **Per spawn** (seeded RNG, deterministic per `GAME_SEED`/wave/index): the
  drone draws its entry cell (scatter along the side) and its assigned exit.
- Re-path walks to the **assigned exit**; if a build cuts that exit off, the
  drone falls back to the **nearest reachable exit** (assignment holds only
  while valid). If no exit is reachable at all (rare pocket case), the drone
  keeps its old path — documented edge case.
- Build validation: **every entry reaches at least one exit** (multi-source
  BFS from all exits). No single-component rule — creative splits (left→top,
  right→bottom) are allowed; a fully sealed exit is inert.
- Segments (side + cell range) are a later config detail; default is the
  full side. Placeholder art reuses `spawn`/`base` sprites; redesign later.
- Balance: implement first, then tune with `tools/analyze_run.py` on real
  runs (leak pressure grows with more escape points).

**Implementation sketch:** `Maze` (entries/exits + validation), `Pathfinder`
(multi-source BFS), `Drone` (assigned exit + fallback), `game.gd` (seeded
scatter spawn, markers per entry/exit cell, re-route/fallback, path
preview), telemetry (`leak` gains the exit cell), tests (validation, scatter
determinism, fallback, leak at exit), docs.

## Known open items (from the prototype docs)

- **Seed flow**: random seed per run (logged for replay) instead of the
  fixed `GAME_SEED = 1` — see `AGENTS.md`/`BALANCE.md`.
- **Skill tree**: `SkillStub` is a placeholder; real meta UI later, never
  in-match — see `AGENTS.md` "Meta".
- **More tower types / drone kinds**: extension points documented in
  `docs/ARCHITECTURE.md`.
- **Telemetry enrichment**: e.g. kill events or per-wave money snapshots if
  `tools/analyze_run.py` needs more than leak/build/wave/run_end.
