class_name TerrainGen
extends RefCounted
## Seeded map dressing for the Dirty World pass: blocking terrain clusters
## (rock/rubble/vent) and cosmetic decor (cracks/stains). Blockers are placed
## greedily and skipped whenever they would leave an entry without a reachable
## exit, so the T08 maze rule holds before the first player build.

const BLOCKER_CLUSTERS := 7
const CLUSTER_MIN := 1
const CLUSTER_MAX := 3
const DECOR_COUNT := 26
const NEIGHBORS: Array[Vector2i] = [
	Vector2i(1, 0), Vector2i(-1, 0), Vector2i(0, 1), Vector2i(0, -1),
]


static func generate(
	size: Vector2i,
	entries: Array[Vector2i],
	exits: Array[Vector2i],
	seed_value: int,
) -> Dictionary:
	var rng := RandomNumberGenerator.new()
	rng.seed = seed_value
	var pathfinder := Pathfinder.new()
	pathfinder.setup(size)
	var blockers: Array[Vector2i] = []
	for i in BLOCKER_CLUSTERS:
		var origin := Vector2i(rng.randi_range(1, size.x - 2), rng.randi_range(1, size.y - 2))
		var cluster: Array[Vector2i] = [origin]
		for j in rng.randi_range(CLUSTER_MIN, CLUSTER_MAX) - 1:
			var base: Vector2i = cluster[rng.randi_range(0, cluster.size() - 1)]
			cluster.append(base + NEIGHBORS[rng.randi_range(0, NEIGHBORS.size() - 1)])
		for cell in cluster:
			if not _is_inside(cell, size):
				continue
			if entries.has(cell) or exits.has(cell) or blockers.has(cell):
				continue
			# Greedy rule: keep the blocker only if every entry keeps an exit.
			pathfinder.set_solid(cell, true)
			var reachable := pathfinder.reachable_from(exits)
			var ok := true
			for entry in entries:
				if not reachable.has(entry):
					ok = false
					break
			if ok:
				blockers.append(cell)
			else:
				pathfinder.set_solid(cell, false)
	var decor: Array[Vector2i] = []
	var attempts := 0
	while decor.size() < DECOR_COUNT and attempts < size.x * size.y * 4:
		attempts += 1
		var cell := Vector2i(rng.randi_range(0, size.x - 1), rng.randi_range(0, size.y - 1))
		if entries.has(cell) or exits.has(cell) or blockers.has(cell) or decor.has(cell):
			continue
		decor.append(cell)
	return {"blockers": blockers, "decor": decor}


static func _is_inside(cell: Vector2i, size: Vector2i) -> bool:
	return cell.x >= 0 and cell.y >= 0 and cell.x < size.x and cell.y < size.y
