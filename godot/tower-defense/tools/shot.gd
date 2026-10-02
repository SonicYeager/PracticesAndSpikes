extends SceneTree
## Reusable visual-QA shot tool: renders the real Main.tscn and saves PNGs.
## Run WINDOWED — a --headless run is refused (no pixels are rendered):
##   Godot --path . -s tools/shot.gd -- \
##     [--states idle,running,gameover] [--seed 7] [--wave 3] \
##     [--towers "8,3;9,6"] [--spawn fast,tank] [--wait 3.0] \
##     [--out-prefix reports/shot]
## States must be unique and in order idle → running → gameover (they share
## one scene; --towers/--spawn only apply to `running`).
## Writes {out-prefix}_{state}.png and logs "SHOT <path> <WxH>" per capture.
## Extension point: state presets live in `_setup_state`; the game hooks it
## uses (seed_override, _start_wave, _spawn_drone, _show_tower, _end_run) are
## the documented QA surface — if one is renamed, the setup aborts and
## `_capture` fails loudly instead of writing a misleading screenshot.

const STATES := ["idle", "running", "gameover"]
const DRONE_KINDS := ["normal", "fast", "tank"]

var _opts := {
	"states": "idle,running",
	"seed": "7",
	"wave": "3",
	"towers": "",
	"spawn": "",
	"wait": "3.0",
	"out-prefix": "reports/shot",
}
var _failed := false
var _state_ready := false


func _initialize() -> void:
	if DisplayServer.get_name() == "headless":
		_fail("needs a window — a headless run renders no pixels")
		return
	if not _parse_args():
		return
	var states := _parse_states()
	if states.is_empty():
		return
	_validate_options(states)
	if _failed:
		return
	var scene := load("res://scenes/Main.tscn")
	if scene == null:
		_fail("could not load res://scenes/Main.tscn")
		return
	var main = scene.instantiate()
	main.seed_override = int(_opts["seed"])
	root.add_child(main)
	await process_frame
	await process_frame
	DirAccess.make_dir_recursive_absolute(_resolve_path(str(_opts["out-prefix"])).get_base_dir())
	for state in states:
		_state_ready = false
		await _setup_state(main, state)
		if not _failed and not _state_ready:
			_fail("state '%s' aborted during setup (game hook renamed?)" % state)
		if _failed:
			return
		await _capture(state)
		if _failed:
			return
	quit(0)


func _parse_args() -> bool:
	var args := OS.get_cmdline_user_args()
	var i := 0
	while i < args.size():
		var flag: String = args[i]
		if not flag.begins_with("--"):
			return _fail("unexpected argument '%s' (expected --key value)" % flag)
		var key := flag.substr(2)
		if not _opts.has(key):
			return _fail("unknown option '%s' (known: %s)" % [flag, ", ".join(_opts.keys())])
		if i + 1 >= args.size():
			return _fail("option '%s' needs a value" % flag)
		_opts[key] = args[i + 1]
		i += 2
	return true


func _parse_states() -> Array:
	var out: Array = []
	for raw in str(_opts["states"]).split(",", false):
		var state := raw.strip_edges()
		if not STATES.has(state):
			_fail("unknown state '%s' (known: %s)" % [state, ", ".join(STATES)])
			return []
		out.append(state)
	if out.is_empty():
		_fail("--states must name at least one state (known: %s)" % ", ".join(STATES))
	return out


func _validate_options(states: Array) -> void:
	## Validates every option up front — even ones a later state would consume —
	## so a typo fails fast instead of producing a misleading screenshot.
	var seed_text := str(_opts["seed"])
	if not seed_text.is_valid_int():
		_fail("--seed must be an integer, got '%s'" % seed_text)
		return
	if int(seed_text) < 0:
		_fail("--seed must be >= 0 (it pins the run seed), got %s" % seed_text)
		return
	var wave_text := str(_opts["wave"])
	if not wave_text.is_valid_int():
		_fail("--wave must be an integer, got '%s'" % wave_text)
		return
	if int(wave_text) < 1 or int(wave_text) > 1000:
		_fail("--wave must be 1..1000, got %s" % wave_text)
		return
	if not str(_opts["wait"]).is_valid_float():
		_fail("--wait must be a number, got '%s'" % _opts["wait"])
		return
	if str(_opts["out-prefix"]).strip_edges() == "":
		_fail("--out-prefix must not be empty")
		return
	for raw in str(_opts["spawn"]).split(",", false):
		var kind := raw.strip_edges()
		if not DRONE_KINDS.has(kind):
			_fail("unknown drone kind '%s' (known: %s)" % [kind, ", ".join(DRONE_KINDS)])
			return
	_parse_cells(str(_opts["towers"]))  # validates the spec; _fail sets the flag
	if _failed:
		return
	if not _validate_states(states):
		return
	if (str(_opts["towers"]) != "" or str(_opts["spawn"]) != "") and not states.has("running"):
		_fail("--towers/--spawn only apply to the 'running' state (add it to --states)")


func _validate_states(states: Array) -> bool:
	## States are presets over ONE shared scene: order matters and duplicates
	## would mislabel/overwrite captures.
	var rank := {"idle": 0, "running": 1, "gameover": 2}
	var last := -1
	for state in states:
		if rank[state] <= last:
			_fail("--states must be unique and in order idle,running,gameover (got '%s')" % ",".join(states))
			return false
		last = rank[state]
	return true


func _setup_state(main, state: String) -> void:
	match state:
		"idle":
			await process_frame
			_state_ready = true
		"running":
			_build_towers(main)
			if _failed:
				return
			main._start_wave(int(_opts["wave"]))
			for raw in str(_opts["spawn"]).split(",", false):
				main._spawn_drone(raw.strip_edges())
			await create_timer(float(_opts["wait"])).timeout
			_state_ready = true
		"gameover":
			main.economy.money = -1
			main._end_run()
			await process_frame
			_state_ready = true


func _build_towers(main) -> void:
	for cell in _parse_cells(str(_opts["towers"])):
		if not main.maze.can_build(cell):
			_fail("tower cell %s is not buildable (terrain/entry?)" % str(cell))
			return
		main.economy.earn(Economy.GUN_COST)
		main.economy.spend(Economy.GUN_COST)
		main.maze.build(cell)
		main._guns[cell] = Gun.new(Drone.center_of(cell))
		main._show_tower(cell)


func _parse_cells(spec: String) -> Array[Vector2i]:
	var cells: Array[Vector2i] = []
	for part in spec.split(";", false):
		var xy := part.strip_edges().split(",")
		if xy.size() != 2:
			_fail("bad tower cell '%s' (expected x,y)" % part)
			return []
		if not xy[0].strip_edges().is_valid_int() or not xy[1].strip_edges().is_valid_int():
			_fail("bad tower cell '%s' (expected integer x,y)" % part)
			return []
		cells.append(Vector2i(xy[0].strip_edges().to_int(), xy[1].strip_edges().to_int()))
	return cells


func _capture(state: String) -> void:
	await process_frame
	var image := root.get_texture().get_image()
	if image == null or image.get_width() == 0 or image.get_height() == 0:
		_fail("empty frame for state '%s' (headless?)" % state)
		return
	var path := _resolve_path("%s_%s.png" % [_opts["out-prefix"], state])
	var err := image.save_png(path)
	if err != OK:
		_fail("saving %s failed (error %d)" % [path, err])
		return
	print("SHOT %s %dx%d" % [path, image.get_width(), image.get_height()])


func _resolve_path(path: String) -> String:
	# Accept `reports/x` like the docs show; keep res:// and absolute paths as-is.
	if path.begins_with("res://") or path.is_absolute_path():
		return path
	return "res://" + path


func _fail(message: String) -> bool:
	push_error("shot.gd: " + message)
	_failed = true
	quit(1)
	return false
