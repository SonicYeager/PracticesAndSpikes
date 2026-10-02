class_name BoardView
extends Node2D
## Board rendering (T13.5): floor tiles, terrain blockers/decor, entry/exit
## markers, battle decals, ambient emitters, the live route preview and the
## grid<->world math. `game.gd` pushes gameplay state in (routes, vent
## readiness, decals); the view never reads gameplay objects.

const TILE := 32
const ART_SCALE := Vector2(1, 1)
const DECAL_CAP := 300
const DECAL_CELL_CAP := 2

const FLOOR_TEX: Array = [
	preload("res://art/floor_0.png"),
	preload("res://art/floor_1.png"),
	preload("res://art/floor_2.png"),
]
const TERRAIN_TEX := {
	"rock": preload("res://art/rock.png"),
	"rubble": preload("res://art/rubble.png"),
	"vent": preload("res://art/vent.png"),
}
const DECOR_TEX := {
	"crack": preload("res://art/decor_crack.png"),
	"stain": preload("res://art/decor_stain.png"),
}
const SPAWN_TEX := preload("res://art/spawn.png")
const BASE_TEX := preload("res://art/base.png")
const EMBER_TEX := preload("res://art/ember.png")

var origin := Vector2.ZERO

var _entry_nodes: Array[Sprite2D] = []
var _exit_nodes: Array[Sprite2D] = []
var _blocker_sprites: Dictionary = {}
var _vent_sprites: Dictionary = {}
var _decals: Array[Sprite2D] = []
var _decals_by_cell: Dictionary = {}
var _routes: Array = []
var _anim_time := 0.0
var _jitter_phase := 0


func setup(map_size: Vector2i, entries: Array[Vector2i], exits: Array[Vector2i], terrain: Dictionary) -> void:
	origin = (Vector2(get_viewport_rect().size) - Vector2(map_size) * TILE) * 0.5
	_draw_floor(map_size)
	_draw_terrain(terrain)
	_entry_nodes = _draw_markers(entries, SPAWN_TEX)
	_exit_nodes = _draw_markers(exits, BASE_TEX)
	_setup_ambient()


func grid_to_world(grid_pos: Vector2) -> Vector2:
	return origin + grid_pos * TILE


func to_cell(pos: Vector2) -> Vector2i:
	return Vector2i(((pos - origin) / TILE).floor())


func cell_center(cell: Vector2i) -> Vector2:
	return origin + (Vector2(cell) + Vector2(0.5, 0.5)) * TILE


func vent_cells() -> Array:
	return _vent_sprites.keys()


func set_vent_ready(cell: Vector2i, ready: bool) -> void:
	var node := _vent_sprites.get(cell) as Sprite2D
	if node == null:
		return
	node.modulate = Color(1.15, 1.05, 0.9) if ready else Color(0.55, 0.55, 0.6)


func remove_blocker(cell: Vector2i) -> void:
	# Defensive: vents are the money sink and stay (the game layer guards too).
	if _vent_sprites.has(cell):
		return
	var node := _blocker_sprites.get(cell) as Sprite2D
	if node == null:
		return
	_blocker_sprites.erase(cell)
	node.queue_free()


func set_routes(routes: Array) -> void:
	_routes = routes
	queue_redraw()


func add_decal(tex: Texture2D, grid_pos: Vector2) -> void:
	# Per-cell cap plus jitter: battle history stays readable in kill zones.
	var cell := Vector2i(grid_pos.floor())
	var s := Sprite2D.new()
	s.texture = tex
	s.position = grid_to_world(grid_pos) + Vector2(_jitter(5.0), _jitter(5.0))
	s.scale = ART_SCALE * (1.0 + _jitter(0.12))
	s.rotation = _jitter(0.4)
	s.z_index = -1  # above floor/decor, below the route preview
	s.set_meta("cell", cell)
	add_child(s)
	_decals.append(s)
	var cell_decals: Array = _decals_by_cell.get(cell, [])
	cell_decals.append(s)
	_decals_by_cell[cell] = cell_decals
	if cell_decals.size() > DECAL_CELL_CAP:
		_remove_decal(cell_decals.pop_front())
	if _decals.size() > DECAL_CAP:
		_remove_decal(_decals.pop_front())


func _process(delta: float) -> void:
	_anim_time += delta
	for i in _entry_nodes.size():
		_entry_nodes[i].scale = ART_SCALE * (1.0 + 0.05 * sin(_anim_time * 3.2 + i * 0.8))
	for i in _exit_nodes.size():
		_exit_nodes[i].scale = ART_SCALE * (1.0 + 0.03 * sin(_anim_time * 4.0 + i * 0.8))


func _draw() -> void:
	# Live routes: each drone's assigned path, dimmed so towers stay readable.
	for route in _routes:
		for p in route:
			draw_circle(cell_center(p), 2.0, Color(0.90, 0.90, 0.40, 0.30))


func _draw_floor(map_size: Vector2i) -> void:
	for y in map_size.y:
		for x in map_size.x:
			var tile := Sprite2D.new()
			# Deterministic variety: stable pattern, no RNG.
			tile.texture = FLOOR_TEX[(x * 7 + y * 13) % FLOOR_TEX.size()]
			tile.position = cell_center(Vector2i(x, y))
			tile.scale = ART_SCALE
			# Bottom layer: keeps the path preview visible above it.
			tile.z_index = -1
			add_child(tile)


func _draw_terrain(terrain: Dictionary) -> void:
	var nodes: Array[Sprite2D] = []
	for cell in terrain["blockers"]:
		var type := _blocker_type(cell)
		var s := Sprite2D.new()
		s.texture = TERRAIN_TEX[type]
		s.position = cell_center(cell)
		s.scale = ART_SCALE
		add_child(s)
		nodes.append(s)
		_blocker_sprites[cell] = s
		if type == "vent":
			_vent_sprites[cell] = s
	# The money sink must exist: promote the first blocker if no vent rolled.
	if _vent_sprites.is_empty() and not terrain["blockers"].is_empty():
		var vent_cell: Vector2i = terrain["blockers"][0]
		nodes[0].texture = TERRAIN_TEX["vent"]
		_vent_sprites[vent_cell] = nodes[0]
	for cell in terrain["decor"]:
		var d := Sprite2D.new()
		d.texture = DECOR_TEX[_decor_type(cell)]
		d.position = cell_center(cell)
		d.scale = ART_SCALE
		d.z_index = -1  # floor grime: above the floor, below the route preview
		add_child(d)


func _draw_marker(cell: Vector2i, tex: Texture2D) -> Sprite2D:
	var marker := Sprite2D.new()
	marker.texture = tex
	marker.position = cell_center(cell)
	marker.scale = ART_SCALE
	add_child(marker)
	return marker


func _draw_markers(cells: Array[Vector2i], tex: Texture2D) -> Array[Sprite2D]:
	var nodes: Array[Sprite2D] = []
	for cell in cells:
		nodes.append(_draw_marker(cell, tex))
	return nodes


func _setup_ambient() -> void:
	# Presentation-only particle emitters (internal randomness, never gameplay).
	# Playtest 2026-10-02: vents only — crack/stain stay static so the map
	# reads quieter and action effects stay the loudest thing on screen.
	for cell in _vent_sprites:
		_add_embers(cell)


func _add_embers(cell: Vector2i) -> void:
	var p := CPUParticles2D.new()
	p.position = cell_center(cell)
	p.texture = EMBER_TEX
	p.amount = 10
	p.lifetime = 1.8
	p.preprocess = 1.5
	p.emission_shape = CPUParticles2D.EMISSION_SHAPE_RECTANGLE
	p.emission_rect_extents = Vector2(9.0, 7.0)
	p.direction = Vector2(0, -1)
	p.spread = 18.0
	p.initial_velocity_min = 9.0
	p.initial_velocity_max = 20.0
	p.gravity = Vector2(0, -4)
	p.scale_amount_min = 0.25
	p.scale_amount_max = 0.55
	p.color = Color(1.0, 0.7, 0.3, 0.85)
	add_child(p)


func _remove_decal(s: Sprite2D) -> void:
	_decals.erase(s)
	var cell: Vector2i = s.get_meta("cell")
	var cell_decals: Array = _decals_by_cell.get(cell, [])
	cell_decals.erase(s)
	if cell_decals.is_empty():
		_decals_by_cell.erase(cell)
	s.queue_free()


func _variant(cell: Vector2i, count: int) -> int:
	# Stable pattern, no RNG (same trick as the floor variety).
	return (cell.x * 7 + cell.y * 13) % count


func _blocker_type(cell: Vector2i) -> String:
	var types: Array = TERRAIN_TEX.keys()
	return types[_variant(cell, types.size())]


func _decor_type(cell: Vector2i) -> String:
	var types: Array = DECOR_TEX.keys()
	return types[_variant(cell, types.size())]


func _jitter(radians: float) -> float:
	# Deterministic per-decal variation; presentation only.
	_jitter_phase += 1
	return (float(_jitter_phase % 3) - 1.0) * radians
