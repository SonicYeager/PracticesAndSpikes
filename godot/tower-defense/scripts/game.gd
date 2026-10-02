extends Node2D
## Prototype entry: variable grid, click-to-build gun maze, sprite visuals.
## Combat (T03): deterministic waves, drones walk the live path, guns fire
## homing tracers, kills earn / leaks cost. T04: waves auto-chain after a
## BREAK_SECONDS intermission (Space skips it), game over shows a full-screen
## summary with restart (R or button). T06: trauma screen shake (Camera2D),
## vignette overlay, barrel recoil, muzzle/impact polish. T08: whole-side
## entries/exits, seeded scatter spawn, assigned exit per drone with a
## nearest-exit fallback.

## Wave phase: exactly one state is active at a time.
enum Phase { IDLE, RUNNING, BREAK, GAME_OVER }

## Map edges used as entry/exit zones (T08). The lists stay generic — more
## sides later, segments are a future config detail.
enum Side { LEFT, RIGHT, TOP, BOTTOM }

const TILE := 32
const ENTRY_SIDES: Array = [Side.LEFT]
const EXIT_SIDES: Array = [Side.RIGHT]
const MAP_SIZE := Vector2i(20, 12)
const SELL_REFUND := Economy.GUN_COST / 2
const ART_SCALE := Vector2(2, 2)
const GAME_SEED := 1
const SCATTER_SEED_MUL := 1000003
const SCATTER_WAVE_MUL := 104729
const SPAWN_INTERVAL := 0.7
const BREAK_SECONDS := 5.0
const SHAKE_DECAY := 1.6         # trauma lost per second
const SHAKE_MAX_OFFSET := 9.0    # px at full trauma
const SHAKE_KILL := 0.12
const SHAKE_LEAK := 0.3
const SHAKE_GAME_OVER := 0.7
const RECOIL_PX := 3.0
const DRONE_FRAME_TIME := 0.15
const HIT_FLASH_TIME := 0.07
const MUZZLE_OFFSET := 0.75
const BAR_WIDTH := 22.0
const BAR_HEIGHT := 3.0
const BAR_OFFSET := Vector2(0, -20)
const TELEMETRY_PATH := "user://run_%d.jsonl"

const FLOOR_TEX: Array = [
	preload("res://art/floor_0.png"),
	preload("res://art/floor_1.png"),
	preload("res://art/floor_2.png"),
]
const GUN_BASE_TEX := preload("res://art/gun_base.png")
const GUN_BARREL_TEX := preload("res://art/gun_barrel.png")
const SPAWN_TEX := preload("res://art/spawn.png")
const BASE_TEX := preload("res://art/base.png")
const PROJECTILE_TEX := preload("res://art/projectile.png")
const MUZZLE_TEX := preload("res://art/muzzle.png")
const IMPACT_TEX := preload("res://art/impact.png")
const EXPLOSION_TEX: Array = [
	preload("res://art/explosion_0.png"),
	preload("res://art/explosion_1.png"),
]
const DRONE_TEX := {
	"normal": [preload("res://art/drone_0.png"), preload("res://art/drone_1.png")],
	"fast": [preload("res://art/drone_fast_0.png"), preload("res://art/drone_fast_1.png")],
	"tank": [preload("res://art/drone_tank.png")],
}
const SFX := {
	"shoot": preload("res://audio/shoot.wav"),
	"hit": preload("res://audio/hit.wav"),
	"kill": preload("res://audio/kill.wav"),
	"leak": preload("res://audio/leak.wav"),
	"build": preload("res://audio/build.wav"),
	"sell": preload("res://audio/sell.wav"),
	"denied": preload("res://audio/denied.wav"),
	"wave": preload("res://audio/wave.wav"),
	"gameover": preload("res://audio/gameover.wav"),
}
const SFX_DB := {
	"shoot": -15.0,
	"hit": -16.0,
	"kill": -8.0,
	"leak": -4.0,
	"build": -8.0,
	"sell": -8.0,
	"denied": -10.0,
	"wave": -6.0,
	"gameover": -4.0,
}

var maze: Maze
var economy: Economy
var telemetry: Telemetry
var origin := Vector2.ZERO

var _tower_nodes: Dictionary = {}
var _guns: Dictionary = {}
var _drones: Array[Drone] = []
var _drone_sprites: Dictionary = {}
var _drone_bars: Dictionary = {}
var _hit_flash: Dictionary = {}
var _drone_phase: Dictionary = {}
var _projectiles: Array[Projectile] = []
var _projectile_sprites: Dictionary = {}
var _sfx: Dictionary = {}
var _bar_tex: ImageTexture
var _entry_nodes: Array[Sprite2D] = []
var _exit_nodes: Array[Sprite2D] = []
var _spawn_counter := 0
var _wave := 0
var _wave_comp: Dictionary = {}
var _spawn_queue: Array[String] = []
var _spawn_timer := 0.0
var _phase: Phase = Phase.IDLE
var _break_timer := 0.0  # remaining intermission seconds; only used in Phase.BREAK
var _shake_trauma := 0.0
var _jitter_phase := 0
var _anim_time := 0.0
var _fx_counter := 0

@onready var _camera: Camera2D = $Camera
@onready var _money_label: Label = $Hud/Panel/Rows/Row1/Money
@onready var _gun_cost_label: Label = $Hud/Panel/Rows/Row1/GunCost
@onready var _wave_label: Label = $Hud/Panel/Rows/Row2/Wave
@onready var _space_icon: TextureRect = $Hud/Panel/Rows/Row2/Space
@onready var _wave_icon: TextureRect = $Hud/Panel/Rows/Row2/WaveIcon
@onready var _game_over_screen: Control = $Hud/GameOver
@onready var _game_over_stats: Label = $Hud/GameOver/Center/Stats
@onready var _restart_button: Button = $Hud/GameOver/Center/Restart


func _ready() -> void:
	maze = Maze.new(MAP_SIZE, _sides_to_cells(ENTRY_SIDES), _sides_to_cells(EXIT_SIDES))
	economy = Economy.new(100)
	telemetry = Telemetry.new(GAME_SEED)
	origin = (Vector2(get_viewport_rect().size) - Vector2(MAP_SIZE) * TILE) * 0.5
	_camera.position = Vector2(get_viewport_rect().size) * 0.5
	_camera.make_current()
	_game_over_screen.hide()
	_make_bar_texture()
	_setup_audio()
	_draw_floor()
	_entry_nodes = _draw_markers(maze.entries, SPAWN_TEX)
	_exit_nodes = _draw_markers(maze.exits, BASE_TEX)
	_update_hud()
	queue_redraw()


func _notification(what: int) -> void:
	if what == NOTIFICATION_WM_CLOSE_REQUEST and telemetry != null and _phase != Phase.GAME_OVER:
		telemetry.flush(TELEMETRY_PATH % GAME_SEED)


func _process(delta: float) -> void:
	_anim_time += delta
	_update_shake(delta)
	if _phase == Phase.GAME_OVER:
		# Frozen world; the shake still decays.
		return
	_update_spawner(delta)
	_update_drones(delta)
	if _phase == Phase.GAME_OVER:
		# A leak ended the run mid-frame: freeze the rest of the simulation.
		return
	_update_guns(delta)
	_update_projectiles(delta)
	_sync_sprites(delta)
	_update_hud()


func _unhandled_input(event: InputEvent) -> void:
	if _phase == Phase.GAME_OVER:
		if event is InputEventKey and event.pressed and not event.echo and event.keycode == KEY_R:
			_restart()
		return
	if event is InputEventMouseButton and event.pressed:
		# Camera-aware: maps to the cell under the cursor even while shaking.
		var cell := _to_cell(get_global_mouse_position())
		if event.button_index == MOUSE_BUTTON_LEFT:
			# Rejected builds (maze would disconnect, drone in the way) refund.
			if economy.spend(Economy.GUN_COST):
				if not _drone_on_cell(cell) and maze.build(cell):
					_guns[cell] = Gun.new(Drone.center_of(cell))
					_show_tower(cell)
					telemetry.event("build", {"cell": [cell.x, cell.y]})
					_reroute_drones()
					queue_redraw()
					_play("build")
				else:
					economy.earn(Economy.GUN_COST)
					_play("denied")
			else:
				_play("denied")
		elif event.button_index == MOUSE_BUTTON_RIGHT:
			if maze.sell(cell):
				_guns.erase(cell)
				_hide_tower(cell)
				economy.earn(SELL_REFUND)
				telemetry.event("sell", {"cell": [cell.x, cell.y]})
				_reroute_drones()
				queue_redraw()
				_play("sell")
		_update_hud()
	elif event is InputEventKey and event.pressed and not event.echo and event.keycode == KEY_SPACE:
		if _phase == Phase.IDLE or _phase == Phase.BREAK:
			# IDLE: start wave 1; BREAK: skip the intermission.
			_start_wave(_wave + 1)


func _start_wave(n: int) -> void:
	_wave = n
	_wave_comp = WaveGen.composition(n, GAME_SEED)
	_phase = Phase.RUNNING
	_break_timer = 0.0
	_spawn_timer = 0.0
	_spawn_queue.clear()
	var normals: int = int(_wave_comp["count"]) - int(_wave_comp["fast"]) - int(_wave_comp["tanks"])
	for i in normals:
		_spawn_queue.append("normal")
	for i in int(_wave_comp["fast"]):
		_spawn_queue.append("fast")
	for i in int(_wave_comp["tanks"]):
		_spawn_queue.append("tank")
	telemetry.event("wave", {"wave": n, "count": _wave_comp["count"], "hp": _wave_comp["hp"]})
	_play("wave")
	_update_hud()


func _update_spawner(delta: float) -> void:
	if _phase == Phase.BREAK:
		_break_timer = maxf(_break_timer - delta, 0.0)
		if _break_timer == 0.0:
			_start_wave(_wave + 1)
		return
	if _phase != Phase.RUNNING:
		return
	if _spawn_queue.is_empty():
		if _drones.is_empty():
			_phase = Phase.BREAK
			_break_timer = BREAK_SECONDS
			_update_hud()
		return
	_spawn_timer -= delta
	if _spawn_timer > 0.0:
		return
	_spawn_timer = SPAWN_INTERVAL
	_spawn_drone(_spawn_queue.pop_front())


func _spawn_drone(kind: String) -> void:
	# Seeded scatter: entry cell and assigned exit are deterministic per run.
	# The index advances per spawn attempt, so the stream never depends on
	# pathfinding success.
	var index := _spawn_counter
	_spawn_counter += 1
	var rng := RandomNumberGenerator.new()
	rng.seed = GAME_SEED * SCATTER_SEED_MUL + _wave * SCATTER_WAVE_MUL + index
	var entry: Vector2i = maze.entries[rng.randi_range(0, maze.entries.size() - 1)]
	var exit: Vector2i = maze.exits[rng.randi_range(0, maze.exits.size() - 1)]
	var cells := _cells_from_path(maze.pathfinder.find_path(entry, exit))
	if cells.is_empty():
		# Sealed exit (possible with future segments): take the nearest one.
		exit = _nearest_exit(entry)
		if exit != Vector2i(-1, -1):
			cells = _cells_from_path(maze.pathfinder.find_path(entry, exit))
	if cells.is_empty():
		return
	var speed: float = float(_wave_comp["speed"]) / TILE
	var d := Drone.spawn(kind, cells, _wave_comp["hp"], speed)
	_drones.append(d)
	var sprite := Sprite2D.new()
	sprite.texture = DRONE_TEX[kind][0]
	sprite.scale = ART_SCALE
	_drone_sprites[d] = sprite
	add_child(sprite)
	var bar_bg := Sprite2D.new()
	bar_bg.texture = _bar_tex
	bar_bg.scale = Vector2(BAR_WIDTH, BAR_HEIGHT)
	bar_bg.modulate = Color(0.08, 0.06, 0.05, 0.9)
	bar_bg.z_index = 1
	var bar_fill := Sprite2D.new()
	bar_fill.texture = _bar_tex
	bar_fill.scale = Vector2(BAR_WIDTH, BAR_HEIGHT)
	bar_fill.z_index = 1
	add_child(bar_bg)
	add_child(bar_fill)
	_drone_bars[d] = {"bg": bar_bg, "fill": bar_fill}
	_drone_phase[d] = float(index) * 0.9
	queue_redraw()


func _update_drones(delta: float) -> void:
	for i in range(_drones.size() - 1, -1, -1):
		var d := _drones[i]
		# Rerouting onto the base cell sets `finished` without `advance()`.
		if d.advance(delta) or d.finished:
			_on_leak(d)
			if _phase == Phase.GAME_OVER:
				break


func _on_leak(d: Drone) -> void:
	economy.on_leak()
	var exit := d.exit_cell()
	telemetry.event("leak", {"wave": _wave, "money": economy.money, "exit": [exit.x, exit.y]})
	_play("leak")
	_add_shake(SHAKE_LEAK)
	_remove_drone(d)
	if economy.is_game_over():
		_end_run()


func _update_guns(delta: float) -> void:
	for cell in _guns:
		var gun: Gun = _guns[cell]
		var target := gun.acquire(_drones)
		if target != null:
			_aim_barrel(cell, target)
		var fired := gun.try_fire(delta, _drones)
		if fired != null:
			_fire(cell, gun, fired)


func _aim_barrel(cell: Vector2i, target: Drone) -> void:
	var root := _tower_nodes.get(cell) as Node2D
	if root == null:
		return
	var barrel := root.get_node_or_null("Barrel") as Node2D
	if barrel == null:
		return
	barrel.rotation = (target.position - Drone.center_of(cell)).angle() + PI / 2


func _fire(cell: Vector2i, gun: Gun, target: Drone) -> void:
	var direction := target.position - gun.position
	if direction.length() < 0.001:
		direction = Vector2.RIGHT
	direction = direction.normalized()
	var muzzle := gun.position + direction * MUZZLE_OFFSET
	var p := Projectile.new(muzzle, target)
	_projectiles.append(p)
	var sprite := Sprite2D.new()
	sprite.texture = PROJECTILE_TEX
	sprite.scale = ART_SCALE
	_projectile_sprites[p] = sprite
	add_child(sprite)
	_recoil(cell, direction)
	_spawn_muzzle_flash(muzzle, direction)
	_play("shoot")


func _update_projectiles(delta: float) -> void:
	for i in range(_projectiles.size() - 1, -1, -1):
		var p := _projectiles[i]
		var hit := p.advance(delta)
		if hit:
			_resolve_hit(p)
		if not p.alive:
			_remove_projectile(p)


func _resolve_hit(p: Projectile) -> void:
	var target := p.target
	if target.take_damage(Gun.DAMAGE):
		economy.on_kill()
		_spawn_explosion(target.position)
		_play("kill")
		_add_shake(SHAKE_KILL)
		_remove_drone(target)
	else:
		_hit_flash[target] = HIT_FLASH_TIME
		_spawn_impact(target.position)
		_play("hit")


func _remove_drone(d: Drone) -> void:
	_drones.erase(d)
	queue_redraw()
	var sprite := _drone_sprites.get(d) as Node
	if sprite != null:
		sprite.queue_free()
	_drone_sprites.erase(d)
	var bars: Dictionary = _drone_bars.get(d, {})
	for node in bars.values():
		(node as Node).queue_free()
	_drone_bars.erase(d)
	_hit_flash.erase(d)
	_drone_phase.erase(d)


func _remove_projectile(p: Projectile) -> void:
	_projectiles.erase(p)
	var sprite := _projectile_sprites.get(p) as Node
	if sprite != null:
		sprite.queue_free()
	_projectile_sprites.erase(p)


func _end_run() -> void:
	if _phase == Phase.GAME_OVER:
		return
	_phase = Phase.GAME_OVER
	_game_over_stats.text = "Welle %d — Geld %d" % [_wave, economy.money]
	_game_over_screen.show()
	_play("gameover")
	_add_shake(SHAKE_GAME_OVER)
	telemetry.event("run_end", {"wave": _wave, "money": economy.money})
	telemetry.flush(TELEMETRY_PATH % GAME_SEED)
	_update_hud()


func _restart() -> void:
	# Full scene reload: no state to unwind by hand.
	get_tree().reload_current_scene()


func _sync_sprites(delta: float) -> void:
	for d in _drones:
		var sprite := _drone_sprites.get(d) as Sprite2D
		if sprite == null:
			continue
		var facing := d.facing()
		var world := _grid_to_world(d.position)
		var phase := float(_drone_phase.get(d, 0.0))
		match d.kind:
			"fast":
				sprite.rotation = facing.angle()
				sprite.flip_h = false
			"tank":
				sprite.flip_h = facing.x < -0.01
				sprite.rotation = sin(_anim_time * 3.0 + phase) * 0.05
			_:
				sprite.flip_h = facing.x < -0.01
				sprite.rotation = 0.0
				world.y += sin(_anim_time * 9.0 + phase) * 1.5
		sprite.position = world
		var frames: Array = DRONE_TEX[d.kind]
		sprite.texture = frames[int(_anim_time / DRONE_FRAME_TIME + phase) % frames.size()]
		if _hit_flash.has(d):
			_hit_flash[d] = maxf(_hit_flash[d] - delta, 0.0)
			sprite.modulate = Color(2.5, 2.5, 2.5) if _hit_flash[d] > 0.0 else Color.WHITE
		_update_hp_bar(d, _grid_to_world(d.position))
	for p in _projectiles:
		var sprite := _projectile_sprites.get(p) as Sprite2D
		if sprite == null:
			continue
		sprite.position = _grid_to_world(p.position)
		var direction := p.target.position - p.position
		if direction.length() > 0.001:
			sprite.rotation = direction.angle()
	for i in _entry_nodes.size():
		_entry_nodes[i].scale = ART_SCALE * (1.0 + 0.05 * sin(_anim_time * 3.2 + i * 0.8))
	for i in _exit_nodes.size():
		_exit_nodes[i].scale = ART_SCALE * (1.0 + 0.03 * sin(_anim_time * 4.0 + i * 0.8))


func _update_hp_bar(d: Drone, world: Vector2) -> void:
	var bars: Dictionary = _drone_bars.get(d, {})
	var bg := bars.get("bg") as Sprite2D
	var fill := bars.get("fill") as Sprite2D
	if bg == null or fill == null:
		return
	var ratio := clampf(d.hp / d.max_hp, 0.0, 1.0)
	var bar_pos := world + BAR_OFFSET
	var width := BAR_WIDTH * ratio
	bg.position = bar_pos
	fill.scale = Vector2(width, BAR_HEIGHT)
	fill.position = bar_pos + Vector2((width - BAR_WIDTH) * 0.5, 0.0)
	fill.modulate = _hp_color(ratio)


func _hp_color(ratio: float) -> Color:
	if ratio > 0.5:
		return Color(0.49, 0.87, 0.39)
	if ratio > 0.25:
		return Color(1.0, 0.82, 0.4)
	return Color(1.0, 0.23, 0.19)


func _spawn_muzzle_flash(grid_pos: Vector2, direction: Vector2) -> void:
	var s := Sprite2D.new()
	s.texture = MUZZLE_TEX
	s.scale = ART_SCALE
	s.position = _grid_to_world(grid_pos)
	s.rotation = direction.angle() + _jitter(0.3)
	add_child(s)
	var t := create_tween()
	t.tween_property(s, "scale", ART_SCALE * 1.35, 0.05)
	t.parallel().tween_property(s, "modulate:a", 0.0, 0.05)
	t.tween_callback(s.queue_free)


func _spawn_impact(grid_pos: Vector2) -> void:
	var s := Sprite2D.new()
	s.texture = IMPACT_TEX
	s.scale = ART_SCALE * 0.7
	s.position = _grid_to_world(grid_pos)
	add_child(s)
	var t := create_tween()
	t.tween_property(s, "scale", ART_SCALE * 1.5, 0.12)
	t.parallel().tween_property(s, "modulate:a", 0.0, 0.12)
	t.tween_callback(s.queue_free)


func _add_shake(amount: float) -> void:
	_shake_trauma = minf(_shake_trauma + amount, 1.0)


func _update_shake(delta: float) -> void:
	_shake_trauma = maxf(_shake_trauma - SHAKE_DECAY * delta, 0.0)
	# Deterministic pseudo-noise: no RNG in the scene layer.
	var amount := _shake_trauma * _shake_trauma
	_camera.offset = Vector2(
		sin(_anim_time * 37.0) + 0.5 * sin(_anim_time * 61.0),
		cos(_anim_time * 43.0) + 0.5 * cos(_anim_time * 71.0)
	) * (SHAKE_MAX_OFFSET * amount)


func _jitter(radians: float) -> float:
	# Deterministic per-effect variation; separate counter so the explosion
	# frame alternation stays strict.
	_jitter_phase += 1
	return (float(_jitter_phase % 3) - 1.0) * radians


func _recoil(cell: Vector2i, direction: Vector2) -> void:
	var root := _tower_nodes.get(cell) as Node2D
	if root == null:
		return
	var barrel := root.get_node_or_null("Barrel") as Node2D
	if barrel == null:
		return
	# Gun cadence (0.6 s) is far above the tween time (0.08 s): shots never
	# overlap, so resetting the position per shot is safe.
	barrel.position = -direction * RECOIL_PX
	var t := create_tween()
	t.tween_property(barrel, "position", Vector2.ZERO, 0.08)


func _spawn_explosion(grid_pos: Vector2) -> void:
	var s := Sprite2D.new()
	s.texture = EXPLOSION_TEX[_fx_counter % EXPLOSION_TEX.size()]
	_fx_counter += 1
	s.position = _grid_to_world(grid_pos)
	s.scale = ART_SCALE
	add_child(s)
	var t := create_tween()
	t.tween_property(s, "scale", ART_SCALE * 1.8, 0.18)
	t.parallel().tween_property(s, "modulate:a", 0.0, 0.18)
	t.tween_callback(s.queue_free)


func _make_bar_texture() -> void:
	var image := Image.create_empty(1, 1, false, Image.FORMAT_RGBA8)
	image.fill(Color.WHITE)
	_bar_tex = ImageTexture.create_from_image(image)


func _setup_audio() -> void:
	for key in SFX:
		var player := AudioStreamPlayer.new()
		player.stream = SFX[key]
		player.volume_db = SFX_DB.get(key, -10.0)
		player.max_polyphony = 8
		add_child(player)
		_sfx[key] = player


func _play(name: String) -> void:
	var player := _sfx.get(name) as AudioStreamPlayer
	if player != null:
		player.play()


func _drone_on_cell(cell: Vector2i) -> bool:
	for d in _drones:
		if d.cell() == cell:
			return true
	return false


func _reroute_drones() -> void:
	for d in _drones:
		var cells := _cells_from_path(maze.pathfinder.find_path(d.cell(), d.exit_cell()))
		if cells.is_empty():
			# Assigned exit cut off: fall back to the nearest reachable exit.
			var alt := _nearest_exit(d.cell())
			if alt != Vector2i(-1, -1):
				cells = _cells_from_path(maze.pathfinder.find_path(d.cell(), alt))
		if not cells.is_empty():
			d.reroute(cells)


func _nearest_exit(from: Vector2i) -> Vector2i:
	# Shortest path wins; ties keep the first exit in maze.exits order (stable).
	var best := Vector2i(-1, -1)
	var best_length := INF
	for exit in maze.exits:
		var path := maze.pathfinder.find_path(from, exit)
		if not path.is_empty() and path.size() < best_length:
			best_length = path.size()
			best = exit
	return best


func _cells_from_path(path: PackedVector2Array) -> Array[Vector2i]:
	var cells: Array[Vector2i] = []
	for p in path:
		cells.append(Vector2i(p))
	return cells


func _grid_to_world(grid_pos: Vector2) -> Vector2:
	return origin + grid_pos * TILE


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
			# Bottom layer: keeps the path preview (Main._draw) visible above it.
			tile.z_index = -1
			add_child(tile)


func _draw_marker(cell: Vector2i, tex: Texture2D) -> Sprite2D:
	var marker := Sprite2D.new()
	marker.texture = tex
	marker.position = _cell_center(cell)
	marker.scale = ART_SCALE
	add_child(marker)
	return marker


func _draw_markers(cells: Array[Vector2i], tex: Texture2D) -> Array[Sprite2D]:
	var nodes: Array[Sprite2D] = []
	for cell in cells:
		nodes.append(_draw_marker(cell, tex))
	return nodes


func _sides_to_cells(sides: Array) -> Array[Vector2i]:
	var cells: Array[Vector2i] = []
	var seen := {}
	for side in sides:
		for cell in _side_cells(side):
			if not seen.has(cell):
				seen[cell] = true
				cells.append(cell)
	return cells


func _side_cells(side: int) -> Array[Vector2i]:
	var cells: Array[Vector2i] = []
	match side:
		Side.LEFT:
			for y in MAP_SIZE.y:
				cells.append(Vector2i(0, y))
		Side.RIGHT:
			for y in MAP_SIZE.y:
				cells.append(Vector2i(MAP_SIZE.x - 1, y))
		Side.TOP:
			for x in MAP_SIZE.x:
				cells.append(Vector2i(x, 0))
		Side.BOTTOM:
			for x in MAP_SIZE.x:
				cells.append(Vector2i(x, MAP_SIZE.y - 1))
	return cells


func _show_tower(cell: Vector2i) -> void:
	var root := Node2D.new()
	root.position = _cell_center(cell)
	var base := Sprite2D.new()
	base.name = "Base"
	base.texture = GUN_BASE_TEX
	base.scale = ART_SCALE
	var barrel := Sprite2D.new()
	barrel.name = "Barrel"
	barrel.texture = GUN_BARREL_TEX
	barrel.scale = ART_SCALE
	# Pivot at the barrel base: texture (8, 12) sits on the tower center.
	barrel.offset = Vector2(0, -4)
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
	_money_label.text = str(economy.money)
	_gun_cost_label.text = str(Economy.GUN_COST)
	match _phase:
		Phase.RUNNING:
			_space_icon.hide()
			_wave_icon.show()
			_wave_label.text = "Welle %d: %d unterwegs, %d in Warteschlange" % [_wave, _drones.size(), _spawn_queue.size()]
		Phase.BREAK:
			_space_icon.show()
			_wave_icon.hide()
			_wave_label.text = "Welle %d in %.1f s" % [_wave + 1, _break_timer]
		Phase.IDLE:
			_space_icon.show()
			_wave_icon.hide()
			_wave_label.text = "Welle %d starten" % (_wave + 1)
		_:
			pass  # GAME_OVER: the overlay covers the HUD


func _draw() -> void:
	if maze == null:
		return
	# Live routes: each drone's assigned path, dimmed so towers stay readable.
	for d in _drones:
		for p in d.path:
			draw_circle(_cell_center(p), 2.0, Color(0.90, 0.90, 0.40, 0.30))
