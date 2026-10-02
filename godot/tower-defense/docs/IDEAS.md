# Ideas (post-prototype)

Directions beyond the T01–T07 prototype. Nothing here is committed scope —
each item needs a slice decision (analyze → plan → implement) before it lands.
Captured so future sessions start from the maintainer's intent, not guesses.

## Multi-entry / multi-exit sides (maze pressure)

*Source: maintainer, 2026-10-02. Design decided and implemented as T08 the
same day (see the git log); kept here as the design record.*

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
- **Per spawn** (seeded RNG, deterministic per run seed/wave/index): the
  drone draws its entry cell (scatter along the side) and its assigned exit.
- Re-path walks to the **assigned exit**; if a build cuts that exit off, the
  drone falls back to the **nearest reachable exit** (assignment holds only
  while valid). If no exit is reachable at all (rare pocket case), a re-path
  keeps the old path and a spawn is dropped — both prevented by validation
  in the default config.
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

## Xeno Tactic homages (reference-driven candidates)

*Source: maintainer, 2026-10-02 — Xeno Tactic was the original inspiration.
Each needs a slice decision; vision check noted.*

- **Wall piece** (cheap blocker): a dedicated wall tower without attack
  (e.g. 8–10 money) deepens mazing without paying the full gun price. XT
  added walls late and players loved them. Vision: pure P1 (workbench).
- **Splitters**: on death spawn two weak offspring — a new enemy behavior
  (Discovery) that rewards wide kill coverage. Caution: keep counterplay
  open (no lock & key).
- **Slow/Frost tower**: the second tower type — a slow field instead of
  damage; XT's freeze was the answer to "reversing the field". Vision: P1/P3.
- **Endurance goals**: XT missions ran 20–100 waves. Ours could offer light
  "containment milestones" (e.g. clear N waves without a leak) — goals, not
  a meta grind.
- **Fliers — only with care**: XT's biggest design trap was the anti-air
  lock & key. If flying/escapee types ever come, they need at least two
  viable counters and must respect the maze in some way.

## Known open items (from the prototype docs)

- ~~**Seed flow**~~: done (T09) — random run seed, logged as
  `run_start.seed`; `seed_override` pins it for replay/tests.
- **Skill tree**: `SkillStub` is a placeholder; real meta UI later, never
  in-match — see `AGENTS.md` "Meta".
- **More tower types / drone kinds**: extension points documented in
  `docs/ARCHITECTURE.md`.
- **Telemetry enrichment**: e.g. kill events or per-wave money snapshots if
  `tools/analyze_run.py` needs more than leak/build/wave/run_end.
