class_name Maze
extends RefCounted
## Buildable grid: tracks towers and rejects any build that would leave an
## entry without a reachable exit. Entry/exit cells and pre-placed terrain
## blockers are never buildable; blockers are solid from the start.
## Any number of entries/exits is supported (side cells by default); a sealed
## exit is inert as long as every entry keeps at least one open exit.

var size: Vector2i
var entries: Array[Vector2i]
var exits: Array[Vector2i]
var blockers: Array[Vector2i]
var built: Dictionary = {}
var pathfinder: Pathfinder


func _init(
	p_size: Vector2i,
	p_entries: Array[Vector2i],
	p_exits: Array[Vector2i],
	p_blockers: Array[Vector2i] = [],
) -> void:
	size = p_size
	entries = p_entries
	exits = p_exits
	blockers = p_blockers
	pathfinder = Pathfinder.new()
	pathfinder.setup(p_size)
	for cell in blockers:
		pathfinder.set_solid(cell, true)


func is_inside(cell: Vector2i) -> bool:
	return cell.x >= 0 and cell.y >= 0 and cell.x < size.x and cell.y < size.y


func is_reserved(cell: Vector2i) -> bool:
	return entries.has(cell) or exits.has(cell) or blockers.has(cell)


func can_build(cell: Vector2i) -> bool:
	if not is_inside(cell):
		return false
	if is_reserved(cell):
		return false
	if built.has(cell):
		return false
	# Hypothetical block: every entry must still reach at least one exit.
	var was_solid := pathfinder.is_solid(cell)
	pathfinder.set_solid(cell, true)
	var reachable := pathfinder.reachable_from(exits)
	var ok := true
	for entry in entries:
		if not reachable.has(entry):
			ok = false
			break
	pathfinder.set_solid(cell, was_solid)
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
