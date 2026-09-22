extends Node2D
## Prototype entry: variable grid, click-to-build gun maze, sprite visuals.
## Art: 16px Ember Foundry at 2x (TILE 32). Barrel pivot via offset (T03).
## Combat (T03), wave loop (T04) and juice (T06) come later.

const TILE := 32
const MAP_SIZE := Vector2i(20, 12)
const SELL_REFUND := Economy.GUN_COST / 2
const ART_SCALE := Vector2(2, 2)

const FLOOR_TEX: Array = [
	preload("res://art/floor_0.png"),
	preload("res://art/floor_1.png"),
	preload("res://art/floor_2.png"),
]
const GUN_BASE_TEX := preload("res://art/gun_base.png")
const GUN_BARREL_TEX := preload("res://art/gun_barrel.png")
const SPAWN_TEX := preload("res://art/spawn.png")
const BASE_TEX := preload("res://art/base.png")

var maze: Maze
var economy: Economy
var origin := Vector2.ZERO
var _tower_nodes: Dictionary = {}

@onready var _money_label: Label = $Hud/Money


func _ready() -> void:
	var spawn := Vector2i(0, MAP_SIZE.y / 2)
	var base := Vector2i(MAP_SIZE.x - 1, MAP_SIZE.y / 2)
	maze = Maze.new(MAP_SIZE, spawn, base)
	economy = Economy.new(100)
	origin = (Vector2(get_viewport_rect().size) - Vector2(MAP_SIZE) * TILE) * 0.5
	_draw_floor()
	_draw_marker(spawn, SPAWN_TEX)
	_draw_marker(base, BASE_TEX)
	_update_hud()
	queue_redraw()


func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventMouseButton and event.pressed:
		var cell := _to_cell(get_global_mouse_position())
		if event.button_index == MOUSE_BUTTON_LEFT:
			# Rejected builds (maze would disconnect) refund immediately.
			if economy.spend(Economy.GUN_COST):
				if maze.build(cell):
					_show_tower(cell)
				else:
					economy.earn(Economy.GUN_COST)
		elif event.button_index == MOUSE_BUTTON_RIGHT:
			if maze.sell(cell):
				_hide_tower(cell)
				economy.earn(SELL_REFUND)
		_update_hud()
		queue_redraw()


func _to_cell(pos: Vector2) -> Vector2i:
	return Vector2i(((pos - origin) / TILE).floor())


func _cell_center(cell: Vector2i) -> Vector2:
	return origin + (Vector2(cell) + Vector2(0.5, 0.5)) * TILE


func _draw_floor() -> void:
	for y in MAP_SIZE.y:
		for x in MAP_SIZE.x:
			var tile := Sprite2D.new()
			# Deterministic variety: stable pattern, no RNG.
			tile.texture = FLOOR_TEX[(x * 7 + y * 13) % FLOOR_TEX.size()]
			tile.position = _cell_center(Vector2i(x, y))
			tile.scale = ART_SCALE
			add_child(tile)


func _draw_marker(cell: Vector2i, tex: Texture2D) -> void:
	var marker := Sprite2D.new()
	marker.texture = tex
	marker.position = _cell_center(cell)
	marker.scale = ART_SCALE
	add_child(marker)


func _show_tower(cell: Vector2i) -> void:
	var root := Node2D.new()
	root.position = _cell_center(cell)
	var base := Sprite2D.new()
	base.texture = GUN_BASE_TEX
	base.scale = ART_SCALE
	var barrel := Sprite2D.new()
	barrel.texture = GUN_BARREL_TEX
	barrel.scale = ART_SCALE
	barrel.position = Vector2(0, -4)
	root.add_child(base)
	root.add_child(barrel)
	add_child(root)
	_tower_nodes[cell] = root


func _hide_tower(cell: Vector2i) -> void:
	var node := _tower_nodes.get(cell) as Node
	if node != null:
		node.queue_free()
		_tower_nodes.erase(cell)


func _update_hud() -> void:
	_money_label.text = "Geld (= Leben): %d   |   Gun: %d (Links bauen, Rechts verkaufen)" % [economy.money, Economy.GUN_COST]


func _draw() -> void:
	if maze == null:
		return
	# Live path preview; floor/towers/markers are sprites now.
	var path := maze.pathfinder.find_path(maze.spawn_cell, maze.base_cell)
	for p in path:
		draw_circle(_cell_center(p), 2.0, Color(0.90, 0.90, 0.40, 0.70))
