# Balance

All tunable gameplay numbers in one place. Values are the current prototype
defaults; every one of them lives in code (source of truth), this table is
the map to them.

## Where the numbers live

| Area | File |
|---|---|
| Economy (costs, rewards, game-over rule, incl. `WALL_COST`) | `scripts/economy.gd` |
| Gun levels (prices, stats, refunds) | `scripts/gun_upgrades.gd` (`GunUpgrades`, ADR 0012) |
| Piece kinds (cost/label/telemetry mapping) | `scripts/pieces.gd` (`Pieces`, T17) |
| Run frame (goal wave, result) | `scripts/run_state.gd` (`RunState`, T18) |
| Time speeds (pause/speed) | `scripts/time_control.gd` (`SPEEDS`, T19) |
| Drone kinds (hp/speed multipliers) | `scripts/drone.gd` (`KIND_MODS`) |
| Wave scaling + composition | `scripts/wave.gd` (`WaveGen`) |
| Wave pacing (spawn/break, modifier knobs) | `scripts/wave_director.gd` (`WaveDirector`) |
| Combat FX (shake, recoil) | `scripts/fx.gd` (`Fx`) |
| Scene pacing / layout | `scripts/game.gd` (top constants) |

## Economy (`economy.gd`)

| Constant | Value | Meaning |
|---|---|---|
| start money | 100 | `Economy.new(100)` in `game.gd` |
| `GUN_COST` | 25 | left-click build (= level-1 price) |
| `WALL_COST` | 10 | wall build (`B` toggles the kind); refund 5 |
| sell refund | `GunUpgrades.refund(level)` | half of the cumulative invest (L1 → 12) |
| `KILL_REWARD` | 6 | per drone killed |
| `LEAK_COST` | 10 | per drone reaching the base |
| game over | `money < 0` | zero is still alive |

Derived: one gun costs ~4.2 kills; a wall costs ~1.7 kills (2 walls < 1 gun);
one leak eats ~1.7 kills. Selling returns half of the cumulative invest
(L1 → 48% of the build cost; wall refund 5), so maze rebuilding is cheap but
not free — wall churn (5) is cheaper than gun churn (13).

Run frame: mission goal = wave 20 (≈ 42 drones / ~285 HP at that point);
win-rate and endless depth are derivable from `mission_cleared` /
`run_end.result` — the tuning signal for the goal.

## Gun levels (`gun_upgrades.gd`, ADR 0012)

Cumulative prices — an upgrade pays the delta to the next level; selling
refunds half of the current price. First balance pass; tune via `upgrade`
telemetry.

| Level | Price (total) | Upgrade delta | Damage | Range | Interval | DPS | Name |
|---|---|---|---|---|---|---|---|
| 1 | 25 | — | 8.0 | 3.5 | 0.6 s | 13.3 | KANONE |
| 2 | 45 | 20 | 11.0 | 3.5 | 0.6 s | 18.3 | KANONE 2 |
| 3 | 80 | 35 | 15.0 | 3.5 | 0.55 s | 27.3 | KANONE 3 |
| 4 | 140 | 60 | 21.0 | 3.8 | 0.5 s | 42.0 | KANONE 4 |
| 5 | 250 | 110 | 30.0 | 4.5 | 0.5 s | 60.0 | LANZE |

Derived: total invest to max = 250 (refund 125); refunds 12/22/40/70/125 are
always ≤ the invest. Targeting is always the drone closest to the base; guns
do not lead their shots (homing tracers make that unnecessary).

## Drone kinds (`drone.gd`)

Multipliers applied to the wave's base hp/speed by `Drone.spawn()`:

| Kind | HP × | Speed × | Wave-1 hp | Wave-1 speed |
|---|---|---|---|---|
| normal | 1.0 | 1.0 | 20.0 | 1.31 cells/s |
| fast | 0.6 | 1.6 | 12.0 | 2.10 cells/s |
| tank | 2.4 | 0.55 | 48.0 | 0.72 cells/s |

Speed is cells/second; `WaveGen` authors px/s and `game.gd` divides by
`TILE` (32). Fast drones are therefore hard to catch with a single gun, tanks
soak ~6 hits at wave 1.

## Waves (`wave.gd`)

```
count = 4 + (n - 1) * 2
hp    = 20 * 1.15^(n - 1)
speed = 42 + min((n - 1) * 2, 40)          # px/s, capped at 82
tanks = randi_range(0, n / 2)              # seeded
fast  = randi_range(0, n / 3)              # seeded
seed  = game_seed + n * 7919
```

Spawn pacing: one drone every 0.7 s (`SPAWN_INTERVAL`), order normals → fast
→ tanks. Each spawn scatters along the entry side and draws a random exit
(seeded per spawn, see the scene pacing table). Between waves: once the
spawn queue and the field are empty, a `BREAK_SECONDS` (5 s) intermission
runs with a HUD countdown, then the next wave auto-starts; Space skips the
wait. Wave 1 is started manually. The state machine (queue, timers, knobs)
lives in `WaveDirector`; the scene spawns and renders.

Composition for seed 1 (reproducible via `seed_override = 1`), verified
by running `WaveGen` directly:

| Wave | Count | HP | Speed (px/s) | Tanks | Fast |
|---|---|---|---|---|---|
| 1 | 4 | 20.0 | 42 | 0 | 0 |
| 2 | 6 | 23.0 | 44 | 1 | 0 |
| 3 | 8 | 26.4 | 46 | 0 | 1 |
| 4 | 10 | 30.4 | 48 | 0 | 1 |
| 5 | 12 | 35.0 | 50 | 2 | 1 |
| 6 | 14 | 40.2 | 52 | 2 | 0 |
| 7 | 16 | 46.3 | 54 | 3 | 1 |
| 8 | 18 | 53.2 | 56 | 2 | 0 |

The tanks/fast split changes with the seed; count/hp/speed do not.

### Wave modifiers (T10)

From wave 3, 40% of waves carry one event — deterministic per seed (the roll
comes after the tanks/fast split, so those stay stable):

| Modifier | Effect |
|---|---|
| `rush` | `SPAWN_INTERVAL` ×0.6 for the wave |
| `swarm` | count ×1.5, hp ×0.7 |
| `blackout` | gun range −1 cell for the wave |
| `bounty` | kill reward +2 for the wave |

Telegraphed in the HUD (during the wave and in the break preview) and logged
in the `wave` telemetry event (effective post-swarm count; normals =
count − fast − tanks). Overcharge (below) is the money sink that answers the
modifiers.

## Wave pacing (`WaveDirector`)

| Constant | Value | Meaning |
|---|---|---|
| `SPAWN_INTERVAL` | 0.7 s | between two drones of a wave |
| `BREAK_SECONDS` | 5.0 s | intermission between waves (Space skips it) |
| modifier knobs | reset per wave | rush: interval ×0.6 · swarm: count ×1.5, hp ×0.7 · blackout: range −1 · bounty: reward +2 |

## Scene pacing (`game.gd`)

| Constant | Value | Meaning |
|---|---|---|
| `MAP_SIZE` | 20×12 | cells |
| `TILE` | 32 | px per cell (32 px art, 1:1) |
| run seed | random per run | `_random_seed()`; logged as `run_start.seed`; `seed_override` >= 0 pins it (default -1) |
| `SCATTER_SEED_MUL` / `SCATTER_WAVE_MUL` | 1000003 / 104729 | scatter seed: `game_seed * MUL + wave * MUL2 + index` (entry + exit) |
| `BLOCKER_CLUSTERS` / `CLUSTER_MIN..MAX` | 7 / 1..3 | terrain clusters (`TerrainGen`) |
| `DECOR_COUNT` | 26 | cosmetic decor cells |
| `DECAL_CAP` / `DECAL_CELL_CAP` | 300 / 2 | battle decals per run (FIFO) / per cell |
| `OVERCHARGE_COST` / `OVERCHARGE_COOLDOWN` | 20 / 6 s | vent overcharge cost and cooldown |
| `OVERCHARGE_DAMAGE` / `OVERCHARGE_RADIUS` | 15 / 2.5 cells | vent burst damage and radius |
| `ROCK_CLEAR_COST` | 15 | right-click removes a rock/rubble blocker (vents stay) |
| `SHAKE_KILL` / `SHAKE_LEAK` / `SHAKE_OVERCHARGE` / `SHAKE_GAME_OVER` | 0.12 / 0.3 / 0.35 / 0.7 | trauma per event (amounts live here, decay in `Fx`) |
| `DRONE_FRAME_TIME` | 0.15 s | 2-frame gait |
| `HIT_FLASH_TIME` | 0.07 s | white hit flash |
| `MUZZLE_OFFSET` | 0.75 cells | tracer spawn at the barrel tip |
| `BAR_WIDTH` / `BAR_HEIGHT` | 22 / 3 px | HP bar |
| `BAR_OFFSET` | (0, −20) px | HP bar above the drone |

## Fx (`fx.gd`)

| Constant | Value | Meaning |
|---|---|---|
| `SHAKE_DECAY` / `SHAKE_MAX_OFFSET` | 1.6 /s · 9 px | trauma decay / max camera offset |
| `RECOIL_PX` | 3.0 | barrel kick per shot (tween back, 0.08 s) |

## Tuning workflow

1. Change the constant in its class (keep `docs/BALANCE.md` in sync).
2. Run the suite — `test_economy` and `test_wave` pin several values on
   purpose; update the tests if the change is intended.
3. Play a few waves, then run `python tools/analyze_run.py` for per-wave
   kill/leak/money tables (exact kills from `kill`/`wave_end` events, each
   wave's money `start->end`, plus top kill zones; it scans `telemetry_local/`,
   then the Godot user dir — copy `user://run_<seed>.jsonl` into
   `telemetry_local/` to keep it; pre-provenance runs live in
   `telemetry_local/legacy/` and can be analyzed by passing that dir).
4. For controlled questions ("hält Build X Welle N?"), run the headless
   harness: `godot --headless --path . -s tools/harness.gd -- --seed 7
   --waves 30 --towers "9,3;10,3"` — pre-wave-1 builds, fixed cadence,
   `--repeat 2` checks determinism, the log renders as `[harness]`.
   Balance claims need ≥8 harness + 3 human runs (README working rules).
