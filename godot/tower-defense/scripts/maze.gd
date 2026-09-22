class_name Maze
extends RefCounted
## Buildable grid: tracks towers and rejects any build that would
## disconnect spawn from base. Spawn/base cells are never buildable.

var size: Vector2i
var spawn_cell: Vector2i
var base_cell: Vector2i
var built: Dictionary = {}
var pathfinder: Pathfinder


func _init(p_size: Vector2i, p_spawn: Vector2i, p_base: Vector2i) -> void:
	size = p_size
	spawn_cell = p_spawn
	base_cell = p_base
	pathfinder = Pathfinder.new()
	pathfinder.setup(p_size)


func is_inside(cell: Vector2i) -> bool:
	return cell.x >= 0 and cell.y >= 0 and cell.x < size.x and cell.y < size.y


func is_reserved(cell: Vector2i) -> bool:
	return cell == spawn_cell or cell == base_cell


func can_build(cell: Vector2i) -> bool:
	if not is_inside(cell):
		return false
	if is_reserved(cell):
		return false
	if built.has(cell):
		return false
	# Hypothetical block: keep the maze solvable or reject the build.
	pathfinder.set_solid(cell, true)
	var ok := pathfinder.has_path(spawn_cell, base_cell)
	pathfinder.set_solid(cell, false)
	return ok


func build(cell: Vector2i, tower_type: String = "gun") -> bool:
	if not can_build(cell):
		return false
	built[cell] = tower_type
	pathfinder.set_solid(cell, true)
	return true


func sell(cell: Vector2i) -> bool:
	if not built.has(cell):
		return false
	built.erase(cell)
	pathfinder.set_solid(cell, false)
	return true


func tower_at(cell: Vector2i) -> String:
	return str(built.get(cell, ""))
