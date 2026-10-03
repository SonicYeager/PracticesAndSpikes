class_name HarnessRun
extends RefCounted
## Headless fast-forward driver (T20): steps a live Main scene manually.
## The caller owns the scene lifecycle and disables the engine's own
## `_process` (`main.set_process(false)`) — direct steps are the only sim
## driver. No awaits, no scene lifecycle; `verify_log` is a read-only check.
## `first_leak_wave`/`first_bankruptcy_wave`: 0 = none/none yet.

const STEP := 1.0 / 60.0
const GUARD_SECONDS_PER_WAVE := 120.0

var game: Node
var cap: int
var max_steps: int

var steps := 0
var waves_started := 0
var waves_cleared := 0
var terminal := ""  # "" (running) | "loss" | "cap" | "guard"
var mission_cleared := false
var first_leak_wave := 0
var first_bankruptcy_wave := 0
var _last_leaks := 0


func _init(p_game: Node, p_cap := 30) -> void:
	game = p_game
	cap = maxi(p_cap, 1)
	max_steps = int(ceil(float(cap) * GUARD_SECONDS_PER_WAVE / STEP))


func step() -> bool:
	## One simulation frame; true when the run is finished.
	if done():
		return true
	game._process(STEP)
	steps += 1
	_track()
	return done()


func done() -> bool:
	return terminal != ""


func _track() -> void:
	var director = game._director
	var state = game._run_state
	if state.leaks > _last_leaks:
		if first_leak_wave == 0:
			first_leak_wave = director.wave
		_last_leaks = state.leaks
	waves_started = maxi(waves_started, director.wave)
	if director.phase == WaveDirector.Phase.BREAK:
		waves_cleared = director.wave
	if director.phase == WaveDirector.Phase.GAME_OVER:
		if state.is_mission_won() and not state.endless:
			# Win screen → player-like endless continue.
			mission_cleared = true
			waves_cleared = maxi(waves_cleared, director.wave)
			game._continue_endless()
			return
		terminal = "loss"
		first_bankruptcy_wave = director.wave
		return
	if director.phase == WaveDirector.Phase.BREAK and director.wave >= cap:
		waves_cleared = director.wave
		terminal = "cap"
		return
	if steps >= max_steps:
		terminal = "guard"


func metrics() -> Dictionary:
	return {
		"terminal": terminal,
		"mission_cleared": mission_cleared,
		"waves_started": waves_started,
		"waves_cleared": waves_cleared,
		"kills": game._run_state.kills,
		"leaks": game._run_state.leaks,
		"money": game.economy.money,
		"first_leak_wave": first_leak_wave,
		"first_bankruptcy_wave": first_bankruptcy_wave,
		"steps": steps,
		"nodes": game.get_child_count(),
		"entities": game._drones.size(),
		"fx": game._fx.get_child_count(),
		"decals": game._board._decals.size(),
	}


func verify_log(path: String) -> String:
	## "" when the flushed log matches memory; else a mismatch description.
	var f := FileAccess.open(path, FileAccess.READ)
	if f == null:
		return "log not readable: %s" % path
	var kills := 0
	var leaks := 0
	var mission := 0
	var end_money = null
	var harness_money = null
	while not f.eof_reached():
		var line := f.get_line()
		if line.strip_edges().is_empty():
			continue
		var entry = JSON.parse_string(line)
		if entry is Dictionary:
			match str(entry.get("t", "")):
				"kill":
					kills += 1
				"leak":
					leaks += 1
				"mission_cleared":
					mission += 1
				"run_end":
					end_money = entry.get("money")
				"harness_end":
					harness_money = entry.get("money")
	if kills != game._run_state.kills:
		return "kills: log %d != memory %d" % [kills, game._run_state.kills]
	if leaks != game._run_state.leaks:
		return "leaks: log %d != memory %d" % [leaks, game._run_state.leaks]
	if mission != (1 if mission_cleared else 0):
		return "mission_cleared: log %d != expected %d" % [mission, 1 if mission_cleared else 0]
	var final_money = end_money if end_money != null else harness_money
	if final_money != null and int(final_money) != game.economy.money:
		return "money: log %s != memory %d" % [final_money, game.economy.money]
	return ""


static func apply_builds(main: Node, towers: String, walls: String) -> String:
	## "" on success; pre-wave-1 builds through the real _try_build path.
	var err := _build_spec(main, towers, Pieces.Kind.GUN)
	if err != "":
		return err
	return _build_spec(main, walls, Pieces.Kind.WALL)


static func _build_spec(main: Node, spec: String, kind: int) -> String:
	for part in spec.split(";", false):
		var xy := part.strip_edges().split(",", false)
		if xy.size() != 2 or not xy[0].is_valid_int() or not xy[1].is_valid_int():
			return "bad cell spec '%s'" % part
		var cell := Vector2i(int(xy[0]), int(xy[1]))
		if not main.maze.can_build(cell):
			return "cell %s is not buildable" % str(cell)
		if not main._try_build(cell, kind):
			return "build at %s failed (money/validation)" % str(cell)
	return ""
