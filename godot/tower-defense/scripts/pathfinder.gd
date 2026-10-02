class_name Pathfinder
extends RefCounted
## Thin wrapper around AStarGrid2D: 4-directional maze pathfinding.
## Diagonal moves are disabled so corridors stay honest (no corner slipping).
## `reachable_from()` is a multi-source BFS for the maze build rule.

const NEIGHBORS: Array[Vector2i] = [
	Vector2i(1, 0), Vector2i(-1, 0), Vector2i(0, 1), Vector2i(0, -1),
]

var map_size: Vector2i
var _grid: AStarGrid2D


func setup(p_size: Vector2i, cell_size: int = 32) -> void:
	map_size = p_size
	_grid = AStarGrid2D.new()
	_grid.region = Rect2i(Vector2i.ZERO, p_size)
	_grid.cell_size = Vector2(cell_size, cell_size)
	_grid.diagonal_mode = AStarGrid2D.DIAGONAL_MODE_NEVER
	_grid.update()


func set_solid(cell: Vector2i, solid: bool) -> void:
	_grid.set_point_solid(cell, solid)


func is_solid(cell: Vector2i) -> bool:
	return _grid.is_point_solid(cell)


func find_path(from_cell: Vector2i, to_cell: Vector2i) -> PackedVector2Array:
	return _grid.get_id_path(from_cell, to_cell)


func has_path(from_cell: Vector2i, to_cell: Vector2i) -> bool:
	return not find_path(from_cell, to_cell).is_empty()


func reachable_from(sources: Array[Vector2i]) -> Dictionary:
	## Multi-source BFS over free cells: {cell: true} for everything reachable
	## from any source without crossing solid points. Backs the maze rule
	## "every entry reaches at least one exit".
	var seen := {}
	var frontier: Array[Vector2i] = []
	for source in sources:
		if not _is_inside(source) or is_solid(source) or seen.has(source):
			continue
		seen[source] = true
		frontier.append(source)
	while not frontier.is_empty():
		var cell: Vector2i = frontier.pop_back()
		for offset in NEIGHBORS:
			var next := cell + offset
			if not _is_inside(next) or seen.has(next) or is_solid(next):
				continue
			seen[next] = true
			frontier.append(next)
	return seen


func _is_inside(cell: Vector2i) -> bool:
	return cell.x >= 0 and cell.y >= 0 and cell.x < map_size.x and cell.y < map_size.y
