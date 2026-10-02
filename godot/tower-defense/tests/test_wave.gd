extends GutTest
## Wave determinism + scaling (Vision: endless deterministic waves).


func test_same_seed_same_composition() -> void:
	var a := WaveGen.composition(5, 12345)
	var b := WaveGen.composition(5, 12345)
	assert_eq(a, b, "Same (wave, seed) must yield identical composition")


func test_different_seeds_may_differ() -> void:
	var seen := {}
	for s in 50:
		var w := WaveGen.composition(6, s)
		seen[str(w["tanks"]) + "/" + str(w["fast"])] = true
	assert_gt(seen.size(), 1, "Seed should influence the fast/tank split")


func test_waves_scale_up() -> void:
	var w1 := WaveGen.composition(1, 7)
	var w9 := WaveGen.composition(9, 7)
	assert_gt(w9["count"], w1["count"], "Count grows per wave")
	assert_gt(w9["hp"], w1["hp"], "HP grows per wave")


func test_wave_one_defaults() -> void:
	var w := WaveGen.composition(1, 1)
	assert_eq(w["count"], 4, "Wave 1 spawns the base count")
	assert_eq(w["wave"], 1)


func test_modifiers_are_deterministic_and_gated() -> void:
	for n in [1, 2]:
		assert_eq(WaveGen.composition(n, 1)["modifier"], "", "No modifiers before wave 3")
	var seen := false
	for n in range(3, 30):
		var a := WaveGen.composition(n, 42)
		var b := WaveGen.composition(n, 42)
		assert_eq(a["modifier"], b["modifier"], "Same seed → same modifier")
		assert_true(
			a["modifier"] == "" or WaveGen.MODIFIERS.has(a["modifier"]),
			"Modifier comes from the known set"
		)
		seen = seen or a["modifier"] != ""
	assert_true(seen, "Modifiers actually occur after wave 3")
