extends SceneTree
## Reusable visual-QA shot tool: renders the real Main.tscn and saves PNGs.
## Run WINDOWED (headless renders no pixels):
##   Godot --path . -s tools/shot.gd -- \
##     [--states idle,running,gameover] [--seed 7] [--wave 3] \
##     [--towers "8,3;9,6"] [--spawn fast,tank] [--wait 3.0] \
##     [--out-prefix reports/shot]
## Writes {out-prefix}_{state}.png and logs "SHOT <path> <WxH>" per capture.
## Extension point: state presets live in `_setup_state`; the game hooks it
## uses (seed_override, _start_wave, _spawn_drone, _show_tower, _end_run) are
## the documented QA surface — a rename there fails loudly via `_fail`.

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


func _initialize() -> void:
	if not _parse_args():
		return
	var states := _parse_states()
	if states.is_empty():
		return
	_validate_options()
	if _failed:
		return
	var main = load("res://scenes/Main.tscn").instantiate()
	main.seed_override = int(_opts["seed"])
	root.add_child(main)
	await process_frame
	await process_frame
	for state in states:
		await _setup_state(main, state)
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


func _validate_options() -> void:
	## Validates every option up front — even ones a later state would consume —
	## so a typo fails fast instead of producing a misleading screenshot.
	if not str(_opts["seed"]).is_valid_int():
		_fail("--seed must be an integer, got '%s'" % _opts["seed"])
		return
	if not str(_opts["wave"]).is_valid_int():
		_fail("--wave must be an integer, got '%s'" % _opts["wave"])
		return
	if not str(_opts["wait"]).is_valid_float():
		_fail("--wait must be a number, got '%s'" % _opts["wait"])
		return
	for raw in str(_opts["spawn"]).split(",", false):
		var kind := raw.strip_edges()
		if not DRONE_KINDS.has(kind):
			_fail("unknown drone kind '%s' (known: %s)" % [kind, ", ".join(DRONE_KINDS)])
			return
	_parse_cells(str(_opts["towers"]))  # validates the spec; _fail sets the flag


func _setup_state(main, state: String) -> void:
	match state:
		"idle":
			await process_frame
		"running":
			_build_towers(main)
			if _failed:
				return
			main._start_wave(int(_opts["wave"]))
			for raw in str(_opts["spawn"]).split(",", false):
				main._spawn_drone(raw.strip_edges())
			await create_timer(float(_opts["wait"])).timeout
		"gameover":
			main.economy.money = -1
			main._end_run()
			await process_frame


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
		cells.append(Vector2i(int(xy[0]), int(xy[1])))
	return cells


func _capture(state: String) -> void:
	await process_frame
	var image := root.get_texture().get_image()
	if image.get_width() == 0 or image.get_height() == 0:
		_fail("empty frame for state '%s'" % state)
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
