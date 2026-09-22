class_name Pathfinder
extends RefCounted
## Thin wrapper around AStarGrid2D: 4-directional maze pathfinding.
## Diagonal moves are disabled so corridors stay honest (no corner slipping).

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
