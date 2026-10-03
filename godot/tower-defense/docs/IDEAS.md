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
  **Umgesetzt 2026-10-03** (T17: 10 money, `B`-Toggle, `Pieces`-Tabelle;
  HUD-Selektor als Folge-Item `tower-defense-build-selector`).
- **Splitters**: on death spawn two weak offspring — a new enemy behavior
  (Discovery) that rewards wide kill coverage. Caution: keep counterplay
  open (no lock & key).
- **Slow/Frost tower**: the second tower type — a slow field instead of
  damage; XT's freeze was the answer to "reversing the field". Vision: P1/P3.
- **Endurance goals**: XT missions ran 20–100 waves. Ours could offer light
  "containment milestones" (e.g. clear N waves without a leak) — goals, not
  a meta grind.
- **Send Next Wave (Early-Send-Bonus)**: XT paid money for calling the next
  wave early (25/35 + Level/5, cap 50) — an active pressure dial. We already
  skip breaks with Space; a bonus variant would make it a conscious "pulse"
  choice. Vision: P3; fits Time Control (M1). Tune only once the
  pressure-curve data exists.
- **Fliers — only with care**: if flying/escapee types ever come, they need
  at least two viable counters and must respect the maze in some way. (The
  code study showed XT's air answers were broad, not "only DCA" — the
  two-counter standard stays.)

## Loop-Erhalt nach dem Zubauen (Playtest 2026-10-02)

Befund: Ist das Maze optimal gebaut, endet die Entscheidung — mehr Content
(Gegner/Türme) vergrößert nur die Anfangswahl, nicht den laufenden Loop.

Kandidaten für *wiederkehrende* Entscheidungen (Analyse:
`context/tasks/tower-defense-map-boredom/`):

- **Wellen-getriebene Terrain-Disruptionen**: Krater zerstören Türme (mit
  Refund), Felsen wachsen, eine neue Entry-Front öffnet sich — das Maze muss
  neu gelöst werden (P1). Geseedet, an die Wave-Modifier andockbar,
  Telegraphing im Break.
- **Commander-Fähigkeiten** (Cooldowns): Repair/EMP/Rally o. ä. als
  Mikro-Entscheidungen im Puls, ohne Türme zu versetzen (P2/P3).
- **Turm-Upgrades** als laufende Geld-Senke — **umgesetzt 2026-10-03** (T16,
  ADR 0012: 5 Stufen, kumulative Preise, Delta-Kauf, Halb-Refund; Panel/Pips) —
  macht Optimieren dauerhaft, invalidiert das Layout aber nicht.
- **Vorerst verworfen**: Flieger/Immunpfade — Anti-Pillar „alles hängt an der
  Luftabwehr".

Nicht Teil dieser Liste: weitere Gegner-/Turm-Varianten ohne Loop-Mechanik.

## Known open items (from the prototype docs)

- ~~**Seed flow**~~: done (T09) — random run seed, logged as
  `run_start.seed`; `seed_override` pins it for replay/tests.
- **Skill tree**: `SkillStub` is a placeholder; real meta UI later, never
  in-match — see `AGENTS.md` "Meta".
- **More tower types / drone kinds**: extension points documented in
  `docs/ARCHITECTURE.md`.
- **Telemetry enrichment**: done (T15) — kill/send/wave_end events, run_start
  provenance, exact kills + money curves + kill zones in the analyzer. Open
  follow-ups (per-wave kill zones, terminal-wave summary on mid-wave death)
  sit with the balance-harness slice.
- **Late-wave perf sanity**: once runs scale past ~25 waves, have the balance
  harness (see `docs/BACKLOG.md`) watch entity/decal load — low risk at M1
  lengths.
