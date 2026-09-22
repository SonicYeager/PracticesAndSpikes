class_name WaveGen
extends RefCounted
## Deterministic wave composition: same (wave_n, game_seed) always yields
## the same wave. No unseeded RNG anywhere — replays share seed + builds.

const BASE_COUNT := 4
const COUNT_PER_WAVE := 2
const BASE_HP := 20.0
const HP_GROWTH := 1.15
const BASE_SPEED := 42.0
const MAX_SPEED_BONUS := 40.0


static func composition(wave_n: int, game_seed: int) -> Dictionary:
	var n := maxi(wave_n, 1)
	var rng := RandomNumberGenerator.new()
	rng.seed = game_seed + n * 7919
	var count := BASE_COUNT + (n - 1) * COUNT_PER_WAVE
	var hp := BASE_HP * pow(HP_GROWTH, n - 1)
	var speed := BASE_SPEED + minf(float((n - 1) * 2), MAX_SPEED_BONUS)
	# Deterministic fast/tank split, grows with wave number.
	var tanks := rng.randi_range(0, n / 2)
	var fast := rng.randi_range(0, n / 3)
	return {
		"wave": n,
		"count": count,
		"hp": hp,
		"speed": speed,
		"tanks": mini(tanks, count),
		"fast": mini(fast, count),
	}
