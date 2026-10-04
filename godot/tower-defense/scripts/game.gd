extends Node2D
## Prototype entry: variable grid, click-to-build gun maze, sprite visuals.
## Combat (T03): deterministic waves, drones walk the live path, guns fire
## homing tracers, kills earn / leaks cost. T04: waves auto-chain after a
## BREAK_SECONDS intermission (Space skips it), game over shows a full-screen
## summary with restart (R or button). T06: trauma screen shake (Camera2D),
## vignette overlay, barrel recoil, muzzle/impact polish. T08: whole-side
## entries/exits, seeded scatter spawn, assigned exit per drone with a
## nearest-exit fallback. T09: random run seed (logged, overridable), seeded
## terrain blockers + decor, battle decals, ambient emitters. T12: HUD
## framework (scenes/Hud.tscn + scripts/hud.gd) with status, build/sell
## panel, segmented wave bar and game-over overlay. T12.5: the wave flow
## lives in `WaveDirector` (phase, queue, timers, modifier knobs); the scene
## orchestrates and owns the visuals. T13: all art 32×32 (fx 16×16), 1:1.
## T13.5: the board (floor/terrain/markers/decals/ambient/route preview) and
## the grid math live in `BoardView`. T14: combat FX (shake, muzzle/impact/
## explosion, ember bursts, recoil) live in `Fx`.

const TILE := BoardView.TILE
const MAP_SIZE := Vector2i(20, 12)
const ART_SCALE := BoardView.ART_SCALE
const SCATTER_SEED_MUL := 1000003
const SCATTER_WAVE_MUL := 104729
const OVERCHARGE_COST := 20
const OVERCHARGE_DAMAGE := 15.0
const OVERCHARGE_RADIUS := 2.5
const OVERCHARGE_COOLDOWN := 6.0
const ROCK_CLEAR_COST := 15
const MODIFIER_LABELS := {
	"rush": "Ansturm",
	"swarm": "Schwarm",
	"blackout": "Blackout",
	"bounty": "Kopfgeld",
}
const SHAKE_KILL := 0.12
const SHAKE_LEAK := 0.3
const SHAKE_OVERCHARGE := 0.35
const SHAKE_GAME_OVER := 0.7
const DRONE_FRAME_TIME := 0.15
const HIT_FLASH_TIME := 0.07
const MUZZLE_OFFSET := 0.75
const BAR_WIDTH := 22.0
const BAR_HEIGHT := 3.0
const BAR_OFFSET := Vector2(0, -20)
const TELEMETRY_PATH := "user://run_%d.jsonl"

const GUN_BASE_TEX := preload("res://art/gun_base.png")
const GUN_BARREL_TEX := preload("res://art/gun_barrel.png")
const WALL_TEX := preload("res://art/wall.png")
const PROJECTILE_TEX := preload("res://art/projectile.png")
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
	"overcharge": preload("res://audio/overcharge.wav"),
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
	"overcharge": -6.0,
}
const SCORCH_TEX := preload("res://art/scorch.png")
const SKID_TEX := preload("res://art/skid.png")
const DEBRIS_TEX := preload("res://art/debris.png")

## Seed override: -1 = random run seed, >= 0 pins it (tests/editor/replay).
@export var seed_override := -1

## Run provenance: "local" for normal sessions; the balance harness sets
## "harness" (logged in run_start, drives the analyzer's provenance marker).
@export var run_source := "local"
## Entry/exit topology (T21): side + optional inclusive `from`/`to` range;
## defaults stay full sides. Empty results fall back to the full side (warned).
@export var entry_segments: Array = [{"side": SideSegments.Side.LEFT}]
@export var exit_segments: Array = [{"side": SideSegments.Side.RIGHT}]
var game_seed := 1

var maze: Maze
var economy: Economy
var telemetry: Telemetry
var _vents: Dictionary = {}

var _tower_nodes: Dictionary = {}
var _wall_nodes: Dictionary = {}
var _guns: Dictionary = {}
var _walls: Dictionary = {}
var _drones: Array[Drone] = []
var _drone_sprites: Dictionary = {}
var _drone_bars: Dictionary = {}
var _hit_flash: Dictionary = {}
var _drone_phase: Dictionary = {}
var _projectiles: Array[Projectile] = []
var _projectile_sprites: Dictionary = {}
var _sfx: Dictionary = {}
var _bar_tex: ImageTexture
var _spawn_counter := 0
var _sell_mode := false
var _selected := Vector2i(-1, -1)
var _build_kind: int = Pieces.Kind.GUN
var _director: WaveDirector
var _run_state: RunState
var _time_control := TimeControl.new()
var _wave_kills := 0
var _wave_leaks := 0
var _wave_money_start := 0
var _anim_time := 0.0

@onready var _camera: Camera2D = $Camera
@onready var _board: BoardView = $Board
@onready var _fx: Fx = $Fx
@onready var _hud: GameHud = $Hud/HudRoot


func _ready() -> void:
	game_seed = seed_override if seed_override >= 0 else _random_seed()
	var entries := _segment_cells(entry_segments, SideSegments.Side.LEFT, "entry")
	var exits := _segment_cells(exit_segments, SideSegments.Side.RIGHT, "exit")
	var terrain := TerrainGen.generate(MAP_SIZE, entries, exits, game_seed)
	maze = Maze.new(MAP_SIZE, entries, exits, terrain["blockers"])
	economy = Economy.new(100)
	telemetry = Telemetry.new(game_seed, run_source)
	_director = WaveDirector.new(game_seed)
	_run_state = RunState.new()
	_apply_time_scale()
	_camera.position = Vector2(get_viewport_rect().size) * 0.5
	_camera.make_current()
	_fx.setup(_camera)
	_hud.wave_pressed.connect(_on_wave_pressed)
	_hud.sell_toggled.connect(_on_sell_toggled)
	_hud.upgrade_pressed.connect(_on_upgrade_pressed)
	_hud.restart_pressed.connect(_restart)
	_hud.continue_pressed.connect(_continue_endless)
	_make_bar_texture()
	_setup_audio()
	_board.setup(MAP_SIZE, maze.entries, maze.exits, terrain)
	for cell in _board.vent_cells():
		_vents[cell] = {"cooldown": 0.0}
	_update_hud()


func _random_seed() -> int:
	# Run seeds come from the clock; the seed is logged for replay and can be
	# pinned via `seed_override` (tests, editor).
	return int(Time.get_unix_time_from_system()) ^ Time.get_ticks_usec()


func _notification(what: int) -> void:
	if what == NOTIFICATION_WM_CLOSE_REQUEST and telemetry != null and _director.phase != WaveDirector.Phase.GAME_OVER:
		telemetry.flush(TELEMETRY_PATH % game_seed)


func _process(delta: float) -> void:
	if _time_control.is_paused():
		# Explicit freeze (tests + the delta-0 spawn edge); the engine scale
		# already yields delta 0 in the real loop.
		return
	_anim_time += delta
	_fx.update(delta)
	if _director.phase == WaveDirector.Phase.GAME_OVER:
		# Frozen world; the shake still decays.
		return
	_update_spawner(delta)
	_update_drones(delta)
	if _director.phase == WaveDirector.Phase.GAME_OVER:
		# A leak ended the run mid-frame: freeze the rest of the simulation.
		return
	_update_guns(delta)
	_update_projectiles(delta)
	_sync_sprites(delta)
	_update_hud()


func _unhandled_input(event: InputEvent) -> void:
	if _director.phase == WaveDirector.Phase.GAME_OVER:
		if event is InputEventKey and event.pressed and not event.echo and event.keycode == KEY_R:
			_restart()
		return
	if event is InputEventMouseButton and event.pressed:
		# Camera-aware: maps to the cell under the cursor even while shaking.
		var cell := _board.to_cell(get_global_mouse_position())
		if event.button_index == MOUSE_BUTTON_LEFT:
			_dispatch_primary(cell)
		elif event.button_index == MOUSE_BUTTON_RIGHT:
			if not _try_sell(cell):
				_try_clear(cell)
		_update_hud()
	elif event is InputEventKey and event.pressed and not event.echo and event.keycode == KEY_SPACE:
		# IDLE: start wave 1; BREAK: skip the intermission (same path as the button).
		_on_wave_pressed()
	elif event is InputEventKey and event.pressed and not event.echo and event.keycode == KEY_ESCAPE:
		_deselect_tower()
	elif event is InputEventKey and event.pressed and not event.echo and event.keycode == KEY_B:
		_toggle_build_kind()
	elif event is InputEventKey and event.pressed and not event.echo and event.keycode == KEY_P:
		_toggle_pause()
	elif event is InputEventKey and event.pressed and not event.echo and event.keycode == KEY_T:
		_cycle_speed()


func _dispatch_primary(cell: Vector2i) -> void:
	## LMB dispatch: sell-mode sells, vents overcharge, towers select, other
	## built cells deny, everything else builds the current kind.
	if _sell_mode:
		_try_sell(cell)
	elif _vents.has(cell):
		# Only during a wave: otherwise there are no drones to hit.
		if _director.phase != WaveDirector.Phase.RUNNING or not _try_overcharge(cell):
			_play("denied")
	elif _guns.has(cell):
		_select_tower(cell)
	elif maze.built.has(cell):
		# Wall: nothing to select or upgrade — feedback only, no spend.
		_play("denied")
	else:
		_deselect_tower()
		_try_build(cell, _build_kind)


func _try_build(cell: Vector2i, kind: int = Pieces.Kind.GUN) -> bool:
	## One build attempt: spend, validate (drone/maze), place; failures refund.
	var cost := Pieces.cost(kind)
	if not economy.spend(cost):
		_play("denied")
		return false
	if _drone_on_cell(cell) or not maze.build(cell, Pieces.telemetry_name(kind)):
		economy.earn(cost)
		_play("denied")
		return false
	if kind == Pieces.Kind.WALL:
		_walls[cell] = true
		_show_wall(cell)
	else:
		_guns[cell] = Gun.new(Drone.center_of(cell))
		_guns[cell].range_bonus = _director.range_bonus
		_show_tower(cell)
	_fx.puff(_board.grid_to_world(Drone.center_of(cell)))
	telemetry.event("build", {"cell": [cell.x, cell.y], "kind": Pieces.telemetry_name(kind)})
	_reroute_drones()
	_play("build")
	return true


func _try_upgrade(cell: Vector2i) -> bool:
	## Pay the level delta; mutate in place so range_bonus (blackout) stays.
	if _director.phase == WaveDirector.Phase.GAME_OVER:
		return false
	var gun: Gun = _guns.get(cell)
	if gun == null or gun.level >= GunUpgrades.MAX_LEVEL:
		_play("denied")
		return false
	var cost := GunUpgrades.upgrade_cost(gun.level)
	if not economy.spend(cost):
		_play("denied")
		return false
	gun.level += 1
	_refresh_tower(cell)
	_fx.puff(_board.grid_to_world(Drone.center_of(cell)))
	telemetry.event("upgrade", {
		"cell": [cell.x, cell.y],
		"from": gun.level - 1,
		"to": gun.level,
		"cost": cost,
		"money": economy.money,
	})
	_play("build")
	_update_hud()
	return true


func _select_tower(cell: Vector2i) -> void:
	_selected = cell
	_update_hud()


func _deselect_tower() -> void:
	if _selected == Vector2i(-1, -1):
		return
	_selected = Vector2i(-1, -1)
	_update_hud()


func _toggle_build_kind() -> void:
	## `B` flips the build kind; the HUD caption/cost follow via _update_hud.
	_build_kind = Pieces.Kind.WALL if _build_kind == Pieces.Kind.GUN else Pieces.Kind.GUN
	_update_hud()


func _refresh_tower(cell: Vector2i) -> void:
	## Level visuals: signature tint here, pips via `_refresh_pips` (visual step).
	var root := _tower_nodes.get(cell) as Node2D
	var gun: Gun = _guns.get(cell)
	if root == null or gun == null:
		return
	var base := root.get_node_or_null("Base") as Sprite2D
	if base != null:
		base.modulate = Color(1.0, 0.9, 0.6) if gun.level >= GunUpgrades.MAX_LEVEL else Color.WHITE
	_refresh_pips(root, gun.level)


func _refresh_pips(root: Node2D, level: int) -> void:
	## Level pips: MAX_LEVEL nodes exist, `level` of them are visible, centered.
	var pips := root.get_node_or_null("LevelPips") as Node2D
	if pips == null:
		pips = Node2D.new()
		pips.name = "LevelPips"
		root.add_child(pips)
		for i in GunUpgrades.MAX_LEVEL:
			var pip := Sprite2D.new()
			pip.texture = _bar_tex
			pip.scale = Vector2(3, 2)
			pip.modulate = Color(0.62, 0.85, 1.0)
			pips.add_child(pip)
	for i in pips.get_child_count():
		var pip := pips.get_child(i) as Sprite2D
		pip.visible = i < level
		pip.position = Vector2((i - (level - 1) * 0.5) * 4.0, 13.0)


func _try_sell(cell: Vector2i) -> bool:
	var kind := maze.tower_at(cell)
	if not maze.sell(cell):
		return false
	var refund := 0
	if kind == "wall":
		refund = Pieces.cost(Pieces.Kind.WALL) / 2
		_walls.erase(cell)
		_hide_wall(cell)
	else:
		var gun: Gun = _guns.get(cell)
		if gun != null:
			refund = GunUpgrades.refund(gun.level)
			_guns.erase(cell)
			_hide_tower(cell)
	_fx.puff(_board.grid_to_world(Drone.center_of(cell)), Color(0.66, 0.7, 0.78))
	economy.earn(refund)
	if _selected == cell:
		_deselect_tower()
	telemetry.event("sell", {"cell": [cell.x, cell.y], "kind": kind})
	_reroute_drones()
	_play("sell")
	return true


func _try_clear(cell: Vector2i) -> bool:
	## Pay to remove a non-vent terrain blocker; the cell becomes buildable.
	if _vents.has(cell):
		_play("denied")
		return false
	if not maze.blockers.has(cell):
		return false
	if not economy.spend(ROCK_CLEAR_COST):
		_play("denied")
		return false
	if not maze.clear_blocker(cell):
		economy.earn(ROCK_CLEAR_COST)
		_play("denied")
		return false
	_board.remove_blocker(cell)
	_fx.puff(_board.grid_to_world(Drone.center_of(cell)), Color(0.72, 0.66, 0.55))
	_reroute_drones()
	telemetry.event("clear", {"cell": [cell.x, cell.y], "money": economy.money})
	_play("build")
	return true


func _on_wave_pressed() -> void:
	if _time_control.is_paused():
		_play("denied")
		return
	if (
		_director.phase == WaveDirector.Phase.IDLE
		or _director.phase == WaveDirector.Phase.BREAK
	):
		telemetry.event("send", {"wave": _director.wave + 1})
		_start_wave(_director.wave + 1)


func _on_sell_toggled(active: bool) -> void:
	_sell_mode = active


func _on_upgrade_pressed() -> void:
	if _selected != Vector2i(-1, -1):
		_try_upgrade(_selected)


func _start_wave(n: int) -> void:
	_director.start(n)
	_wave_kills = 0
	_wave_leaks = 0
	_wave_money_start = economy.money
	_apply_wave_knobs()
	telemetry.event("wave", {
		"wave": n,
		"count": _director.composition["count"],
		"hp": _director.composition["hp"],
		"modifier": _director.composition["modifier"],
	})
	_play("wave")
	_update_hud()


func _apply_wave_knobs() -> void:
	## Pushes the director's per-wave knobs into the live guns (T10/T12.5).
	for gun in _guns.values():
		gun.range_bonus = _director.range_bonus


func _update_spawner(delta: float) -> void:
	if _director.tick_break(delta):
		_start_wave(_director.wave + 1)
		return
	if _director.phase != WaveDirector.Phase.RUNNING:
		return
	if _director.queue.is_empty() and _drones.is_empty():
		telemetry.event("wave_end", {
			"wave": _director.wave,
			"kills": _wave_kills,
			"leaks": _wave_leaks,
			"money_start": _wave_money_start,
			"money_end": economy.money,
		})
		if _run_state.register_wave_cleared(_director.wave):
			_win_run()
			return
		_director.begin_break()
		_update_hud()
		return
	var kind := _director.tick_spawn(delta)
	if kind != "":
		_spawn_drone(kind)


func _spawn_drone(kind: String) -> void:
	# Seeded scatter: entry cell and assigned exit are deterministic per run.
	# The index advances per spawn attempt, so the stream never depends on
	# pathfinding success.
	var index := _spawn_counter
	_spawn_counter += 1
	var rng := RandomNumberGenerator.new()
	rng.seed = game_seed * SCATTER_SEED_MUL + _director.wave * SCATTER_WAVE_MUL + index
	var entry: Vector2i = maze.entries[rng.randi_range(0, maze.entries.size() - 1)]
	var exit: Vector2i = maze.exits[rng.randi_range(0, maze.exits.size() - 1)]
	var cells := _cells_from_path(maze.pathfinder.find_path(entry, exit))
	if cells.is_empty():
		# Sealed exit: take the nearest one.
		exit = _nearest_exit(entry)
		if exit != Vector2i(-1, -1):
			cells = _cells_from_path(maze.pathfinder.find_path(entry, exit))
	if cells.is_empty():
		return
	var speed: float = float(_director.composition["speed"]) / TILE
	var d := Drone.spawn(kind, cells, _director.composition["hp"], speed)
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


func _update_drones(delta: float) -> void:
	for i in range(_drones.size() - 1, -1, -1):
		var d := _drones[i]
		# Rerouting onto the base cell sets `finished` without `advance()`.
		if d.advance(delta) or d.finished:
			_on_leak(d)
			if _director.phase == WaveDirector.Phase.GAME_OVER:
				break


func _on_leak(d: Drone) -> void:
	economy.on_leak()
	_wave_leaks += 1
	_run_state.register_leak()
	var exit := d.exit_cell()
	telemetry.event("leak", {"wave": _director.wave, "money": economy.money, "exit": [exit.x, exit.y]})
	_play("leak")
	_fx.shake(SHAKE_LEAK)
	_hud.flash_leak()
	_board.add_decal(SKID_TEX, d.position)
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
	var p := Projectile.new(muzzle, target, gun.damage())
	_projectiles.append(p)
	var sprite := Sprite2D.new()
	sprite.texture = PROJECTILE_TEX
	sprite.scale = ART_SCALE
	_projectile_sprites[p] = sprite
	add_child(sprite)
	_fx.recoil(_tower_nodes.get(cell) as Node2D, direction)
	_fx.muzzle(_board.grid_to_world(muzzle), direction)
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
	_apply_damage(p.target, p.damage)


func _apply_damage(target: Drone, amount: float) -> bool:
	## Shared kill/hit consequences; returns true when the drone died.
	if target.take_damage(amount):
		economy.on_kill(_director.kill_reward)
		_wave_kills += 1
		_run_state.register_kill()
		telemetry.event("kill", {
			"wave": _director.wave,
			"cell": [target.cell().x, target.cell().y],
			"kind": target.kind,
		})
		_fx.explosion(_board.grid_to_world(target.position))
		_fx.ring(_board.grid_to_world(target.position))
		_board.add_decal(SCORCH_TEX, target.position)
		_play("kill")
		_fx.shake(SHAKE_KILL)
		_remove_drone(target)
		return true
	_hit_flash[target] = HIT_FLASH_TIME
	_fx.impact(_board.grid_to_world(target.position))
	_board.add_decal(DEBRIS_TEX, target.position)
	_play("hit")
	return false


func _try_overcharge(cell: Vector2i) -> bool:
	if _time_control.is_paused():
		return false
	if not _vents.has(cell):
		return false
	var vent: Dictionary = _vents[cell]
	if vent["cooldown"] > 0.0:
		return false
	if not economy.spend(OVERCHARGE_COST):
		return false
	vent["cooldown"] = OVERCHARGE_COOLDOWN
	var center := Drone.center_of(cell)
	for d in _drones.duplicate():
		if center.distance_to(d.position) <= OVERCHARGE_RADIUS:
			_apply_damage(d, OVERCHARGE_DAMAGE)
	_fx.ember_burst(_board.grid_to_world(center))
	_fx.shake(SHAKE_OVERCHARGE)
	_play("overcharge")
	telemetry.event("overcharge", {"cell": [cell.x, cell.y], "money": economy.money})
	_update_hud()
	return true


func _remove_drone(d: Drone) -> void:
	_drones.erase(d)
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


func _apply_time_scale() -> void:
	## Engine-level scale (tweens/particles included); pause = 0.
	Engine.time_scale = _time_control.scale()


func _toggle_pause() -> void:
	_time_control.toggle_pause()
	_apply_time_scale()
	telemetry.event("time_control", {
		"action": "pause" if _time_control.is_paused() else "resume",
		"speed": _time_control.speed(),
		"paused": _time_control.is_paused(),
	})
	_update_hud()


func _cycle_speed() -> void:
	_time_control.cycle_speed()
	_apply_time_scale()
	telemetry.event("time_control", {
		"action": "speed",
		"speed": _time_control.speed(),
		"paused": _time_control.is_paused(),
	})
	_update_hud()


func _win_run() -> void:
	## Mission clear: the goal wave is down — win screen + mission_cleared.
	## Precondition: `register_wave_cleared` already returned true for this wave.
	_director.end_run()
	_hud.show_run_end("win", _director.wave, economy.money, _run_state.kills, _run_state.leaks, 0)
	_play("wave")
	_fx.shake(SHAKE_GAME_OVER)
	telemetry.event("mission_cleared", {
		"wave": _director.wave,
		"money": economy.money,
		"kills": _run_state.kills,
		"leaks": _run_state.leaks,
	})
	telemetry.flush(TELEMETRY_PATH % game_seed)
	_update_hud()


func _continue_endless() -> void:
	## Win screen → endless segment: same run, back into the break.
	if not _run_state.is_mission_won() or _run_state.endless:
		return
	_run_state.endless = true
	_hud.hide_run_end()
	_director.begin_break()
	_update_hud()


func _end_run() -> void:
	if _director.phase == WaveDirector.Phase.GAME_OVER:
		return
	_run_state.register_loss()
	_director.end_run()
	var result := "win" if _run_state.is_mission_won() else "loss"
	_hud.show_run_end(
		result,
		_director.wave,
		economy.money,
		_run_state.kills,
		_run_state.leaks,
		_run_state.cleared_wave
	)
	_play("gameover")
	_fx.shake(SHAKE_GAME_OVER)
	telemetry.event("run_end", {
		"wave": _director.wave,
		"money": economy.money,
		"result": result,
		"endless": _run_state.endless,
	})
	telemetry.flush(TELEMETRY_PATH % game_seed)
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
		var world := _board.grid_to_world(d.position)
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
		_update_hp_bar(d, _board.grid_to_world(d.position))
	for p in _projectiles:
		var sprite := _projectile_sprites.get(p) as Sprite2D
		if sprite == null:
			continue
		sprite.position = _board.grid_to_world(p.position)
		var direction := p.target.position - p.position
		if direction.length() > 0.001:
			sprite.rotation = direction.angle()
	for cell in _vents:
		var vent: Dictionary = _vents[cell]
		vent["cooldown"] = maxf(vent["cooldown"] - delta, 0.0)
		_board.set_vent_ready(cell, vent["cooldown"] == 0.0)
	_board.set_routes(_active_paths())


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


func _active_paths() -> Array:
	# Route preview input: the board draws these paths under the units.
	var routes: Array = []
	for d in _drones:
		routes.append(d.path)
	return routes


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


func _segment_cells(segments: Array, fallback_side: int, label: String) -> Array[Vector2i]:
	## Config guard: an empty cell list would crash the scatter draw
	## (`randi_range(0, -1)`); fall back to the full side and warn.
	var cells := SideSegments.cells(MAP_SIZE, segments)
	if cells.is_empty():
		push_warning("%s segments produced no cells - falling back to the full side" % label)
		cells = SideSegments.cells(MAP_SIZE, [{"side": fallback_side}])
	return cells


func _show_tower(cell: Vector2i) -> void:
	var root := Node2D.new()
	root.position = _board.cell_center(cell)
	var base := Sprite2D.new()
	base.name = "Base"
	base.texture = GUN_BASE_TEX
	base.scale = ART_SCALE
	var barrel := Sprite2D.new()
	barrel.name = "Barrel"
	barrel.texture = GUN_BARREL_TEX
	barrel.scale = ART_SCALE
	# Pivot at the barrel base: texture (16, 24) sits on the tower center.
	barrel.offset = Vector2(0, -8)
	root.add_child(base)
	root.add_child(barrel)
	add_child(root)
	_tower_nodes[cell] = root
	var gun: Gun = _guns.get(cell)
	_refresh_pips(root, gun.level if gun != null else 1)


func _hide_tower(cell: Vector2i) -> void:
	var node := _tower_nodes.get(cell) as Node
	if node != null:
		node.queue_free()
		_tower_nodes.erase(cell)


func _show_wall(cell: Vector2i) -> void:
	var node := Sprite2D.new()
	node.texture = WALL_TEX
	node.scale = ART_SCALE
	node.position = _board.cell_center(cell)
	add_child(node)
	_wall_nodes[cell] = node


func _hide_wall(cell: Vector2i) -> void:
	var node := _wall_nodes.get(cell) as Node
	if node != null:
		node.queue_free()
		_wall_nodes.erase(cell)


func _update_hud() -> void:
	var modifier_id := ""
	match _director.phase:
		WaveDirector.Phase.RUNNING:
			modifier_id = str(_director.composition.get("modifier", ""))
		WaveDirector.Phase.BREAK:
			modifier_id = _director.next_modifier
	var selected := {}
	if _selected != Vector2i(-1, -1) and _guns.has(_selected):
		var gun: Gun = _guns[_selected]
		var next_cost := GunUpgrades.upgrade_cost(gun.level)
		selected = {
			"level": gun.level,
			"name": GunUpgrades.display_name(gun.level),
			"max_level": GunUpgrades.MAX_LEVEL,
			"next_cost": next_cost,
			"affordable": economy.can_afford(next_cost),
		}
	else:
		_selected = Vector2i(-1, -1)  # stale selection (tower gone)
	_hud.update_state({
		"money": economy.money,
		"build_cost": Pieces.cost(_build_kind),
		"build_label": Pieces.label(_build_kind),
		"build_kind": Pieces.telemetry_name(_build_kind),
		"wave": _director.wave,
		"phase": _phase_name(),
		"alive": _drones.size(),
		"queued": _director.queue.size(),
		"total": _director.total,
		"break_left": _director.break_timer,
		"modifier_id": modifier_id,
		"modifier_label": _modifier_text(modifier_id),
		"paused": _time_control.is_paused(),
		"speed": _time_control.speed(),
		"selected": selected,
	})


func _phase_name() -> String:
	match _director.phase:
		WaveDirector.Phase.IDLE:
			return "idle"
		WaveDirector.Phase.RUNNING:
			return "running"
		WaveDirector.Phase.BREAK:
			return "break"
		_:
			return "game_over"


func _modifier_text(modifier: String) -> String:
	if modifier == "":
		return ""
	return str(MODIFIER_LABELS.get(modifier, modifier))
