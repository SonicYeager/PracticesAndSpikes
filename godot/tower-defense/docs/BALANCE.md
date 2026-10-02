# Balance

All tunable gameplay numbers in one place. Values are the current prototype
defaults; every one of them lives in code (source of truth), this table is
the map to them.

## Where the numbers live

| Area | File |
|---|---|
| Economy (costs, rewards, game-over rule) | `scripts/economy.gd` |
| Gun (range, damage, cadence) | `scripts/gun.gd` |
| Drone kinds (hp/speed multipliers) | `scripts/drone.gd` (`KIND_MODS`) |
| Wave scaling + composition | `scripts/wave.gd` (`WaveGen`) |
| Scene pacing / layout | `scripts/game.gd` (top constants) |

## Economy (`economy.gd`)

| Constant | Value | Meaning |
|---|---|---|
| start money | 100 | `Economy.new(100)` in `game.gd` |
| `GUN_COST` | 25 | left-click build |
| `SELL_REFUND` | 12 | `GUN_COST / 2` (integer division, `game.gd`) |
| `KILL_REWARD` | 6 | per drone killed |
| `LEAK_COST` | 10 | per drone reaching the base |
| game over | `money < 0` | zero is still alive |

Derived: one gun costs ~4.2 kills; one leak eats ~1.7 kills. Selling returns
48% of the build cost, so maze rebuilding is cheap but not free.

## Gun (`gun.gd`)

| Constant | Value | Meaning |
|---|---|---|
| `RANGE` | 3.5 cells | 112 px — covers 3 tiles in a straight line |
| `DAMAGE` | 8.0 | per hit |
| `INTERVAL` | 0.6 s | cadence |

Derived: 13.3 dps single-target; a wave-1 drone (20 hp) takes 3 hits
(24 damage) ≈ 1.2 s of fire. Targeting is always the drone closest to the
base; guns do not lead their shots (homing tracers make that unnecessary).

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
wait. Wave 1 is started manually.

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

## Scene pacing (`game.gd`)

| Constant | Value | Meaning |
|---|---|---|
| `MAP_SIZE` | 20×12 | cells |
| `TILE` | 32 | px per cell (16 px art at 2×) |
| run seed | random per run | `_random_seed()`; logged as `run_start.seed`; `seed_override` >= 0 pins it (default -1) |
| `SPAWN_INTERVAL` | 0.7 s | between two drones of a wave |
| `SCATTER_SEED_MUL` / `SCATTER_WAVE_MUL` | 1000003 / 104729 | scatter seed: `game_seed * MUL + wave * MUL2 + index` (entry + exit) |
| `BLOCKER_CLUSTERS` / `CLUSTER_MIN..MAX` | 7 / 1..3 | terrain clusters (`TerrainGen`) |
| `DECOR_COUNT` | 26 | cosmetic decor cells |
| `DECAL_CAP` / `DECAL_CELL_CAP` | 300 / 2 | battle decals per run (FIFO) / per cell |
| `OVERCHARGE_COST` / `OVERCHARGE_COOLDOWN` | 20 / 6 s | vent overcharge cost and cooldown |
| `OVERCHARGE_DAMAGE` / `OVERCHARGE_RADIUS` | 15 / 2.5 cells | vent burst damage and radius |
| `BREAK_SECONDS` | 5.0 s | intermission between waves (Space skips it) |
| `SHAKE_DECAY` / `SHAKE_MAX_OFFSET` | 1.6 /s · 9 px | trauma decay / max camera offset |
| `SHAKE_KILL` / `SHAKE_LEAK` / `SHAKE_GAME_OVER` | 0.12 / 0.3 / 0.7 | trauma per event |
| `RECOIL_PX` | 3.0 | barrel kick per shot (tween back, 0.08 s) |
| `DRONE_FRAME_TIME` | 0.15 s | 2-frame bob |
| `HIT_FLASH_TIME` | 0.07 s | white hit flash |
| `MUZZLE_OFFSET` | 0.75 cells | tracer spawn at the barrel tip |
| `BAR_WIDTH` / `BAR_HEIGHT` | 22 / 3 px | HP bar |
| `BAR_OFFSET` | (0, −20) px | HP bar above the drone |

## Tuning workflow

1. Change the constant in its class (keep `docs/BALANCE.md` in sync).
2. Run the suite — `test_economy` and `test_wave` pin several values on
   purpose; update the tests if the change is intended.
3. Play a few waves, then run `python tools/analyze_run.py` for per-wave
   leak/kill/money tables (it scans `telemetry_local/`, then the Godot user
   dir; copy `user://run_<seed>.jsonl` into `telemetry_local/` to keep it).
