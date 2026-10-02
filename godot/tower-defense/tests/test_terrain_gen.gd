extends GutTest
## TerrainGen: deterministic dressing, never seals entries, decor is cosmetic.

const SIZE := Vector2i(20, 12)
const ENTRIES: Array[Vector2i] = [Vector2i(0, 2), Vector2i(0, 6), Vector2i(0, 10)]
const EXITS: Array[Vector2i] = [Vector2i(19, 2), Vector2i(19, 6), Vector2i(19, 10)]


func test_same_seed_same_dressing() -> void:
	var a := TerrainGen.generate(SIZE, ENTRIES, EXITS, 42)
	var b := TerrainGen.generate(SIZE, ENTRIES, EXITS, 42)
	assert_eq(a["blockers"], b["blockers"])
	assert_eq(a["decor"], b["decor"])


func test_different_seeds_dress_differently() -> void:
	var a := TerrainGen.generate(SIZE, ENTRIES, EXITS, 1)
	var b := TerrainGen.generate(SIZE, ENTRIES, EXITS, 2)
	assert_ne(a["blockers"], b["blockers"])


func test_never_seals_entries_and_avoids_specials() -> void:
	for seed_value in [1, 2, 3, 4, 5]:
		var terrain := TerrainGen.generate(SIZE, ENTRIES, EXITS, seed_value)
		var blockers: Array = terrain["blockers"]
		var decor: Array = terrain["decor"]
		assert_eq(decor.size(), TerrainGen.DECOR_COUNT, "Default map reaches the decor count")
		var pathfinder := Pathfinder.new()
		pathfinder.setup(SIZE)
		for cell in blockers:
			assert_false(ENTRIES.has(cell), "No blocker on an entry")
			assert_false(EXITS.has(cell), "No blocker on an exit")
			assert_false(decor.has(cell), "Blockers and decor are disjoint")
			pathfinder.set_solid(cell, true)
		var reachable := pathfinder.reachable_from(EXITS)
		for entry in ENTRIES:
			assert_true(
				reachable.has(entry),
				"Entry %s keeps an exit (seed %d)" % [str(entry), seed_value]
			)
