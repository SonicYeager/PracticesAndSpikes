extends SceneTree
## Headless fast-forward balance harness (T20). Drives Main.tscn manually:
## direct `_process(STEP)` calls are the only sim driver (`set_process(false)`),
## interleaved `process_frame` awaits only flush tweens/queue_free.
##
## Usage:
##   godot --headless --path . -s tools/harness.gd -- --seed 7 --waves 30 \
##       --towers "9,3;10,3" [--walls "8,4"] [--money 100] [--repeat 2] \
##       [--out reports/harness.jsonl]
##
## Limits: builds happen before wave 1 only (no per-wave scheduling); the
## fixed STEP (1/60) keeps runs frame-rate independent, so manual-run
## comparisons are outcome-level, not bit-identical. The guard budget is
## cap x 120 s of sim time (about 2.4x headroom on realistic runs). With
## `--repeat`, only the last run's log remains; a natural loss with `--out`
## also writes `user://run_<seed>.jsonl` via the game's own flush.
##
## Exit codes: 0 = invariants ok; 1 = failed (args, build, guard, mismatch).

const COMPARE_KEYS := [
	"terminal", "mission_cleared", "waves_started", "waves_cleared", "kills",
	"leaks", "money", "first_leak_wave", "first_bankruptcy_wave", "steps",
]

var _opts := {
	"seed": 7,
	"waves": 30,
	"towers": "",
	"walls": "",
	"money": 100,
	"repeat": 1,
	"out": "",
}
var _failed := false


func _initialize() -> void:
	_parse_args()
	if _failed:
		return
	_validate_options()
	if _failed:
		return
	var baseline: Dictionary = {}
	for r in int(_opts["repeat"]):
		var metrics: Dictionary = await _run_once()
		if _failed:
			break
		if r == 0:
			baseline = metrics
		else:
			_compare_runs(baseline, metrics, r + 1)
		_print_metrics(r + 1, metrics)
		if not _failed:
			_check_invariants(metrics)
	print("INVARIANTS " + ("ok" if not _failed else "FAILED"))
	quit(1 if _failed else 0)


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


func _validate_options() -> void:
	## Validates every option up front so a typo fails fast.
	var seed_text := str(_opts["seed"])
	if not seed_text.is_valid_int() or int(seed_text) < 0:
		_fail("--seed must be an integer >= 0, got '%s'" % seed_text)
		return
	var waves_text := str(_opts["waves"])
	if not waves_text.is_valid_int() or int(waves_text) < 1:
		_fail("--waves must be an integer >= 1, got '%s'" % waves_text)
		return
	var money_text := str(_opts["money"])
	if not money_text.is_valid_int() or int(money_text) < 0:
		_fail("--money must be an integer >= 0, got '%s'" % money_text)
		return
	var repeat_text := str(_opts["repeat"])
	if not repeat_text.is_valid_int() or int(repeat_text) < 1:
		_fail("--repeat must be an integer >= 1, got '%s'" % repeat_text)
		return
	# Build specs are validated loudly at run time by HarnessRun.apply_builds.


func _run_once() -> Dictionary:
	var scene := load("res://scenes/Main.tscn")
	if scene == null:
		return _fail_run("could not load res://scenes/Main.tscn")
	var main = scene.instantiate()
	main.seed_override = int(_opts["seed"])
	main.run_source = "harness"
	main.process_mode = Node.PROCESS_MODE_DISABLED  # no engine step before setup
	root.add_child(main)
	await process_frame  # _ready/@onready run; nothing processes this frame
	main.process_mode = Node.PROCESS_MODE_INHERIT
	main.set_process(false)  # direct steps are the only sim driver (sticks after ready)
	if main.is_processing():
		return _fail_run("could not disable Main._process")
	main.economy.money = int(_opts["money"])
	main.telemetry.event("harness_start", {
		"waves": int(_opts["waves"]),
		"money": int(_opts["money"]),
		"dt": HarnessRun.STEP,
		"towers": str(_opts["towers"]),
		"walls": str(_opts["walls"]),
	})
	var err := HarnessRun.apply_builds(main, str(_opts["towers"]), str(_opts["walls"]))
	if err != "":
		main.queue_free()
		await process_frame
		return _fail_run(err)
	main._on_wave_pressed()  # wave 1 (logs `send`, like a manual run)
	var run := HarnessRun.new(main, int(_opts["waves"]))
	var frames := 0
	while not run.step():
		frames += 1
		if frames % 60 == 0:
			await process_frame  # tween/queue_free cleanup only
	if run.terminal == "cap" or run.terminal == "guard":
		main.telemetry.event("harness_end", {
			"wave": main._director.wave,
			"reason": run.terminal,
			"money": main.economy.money,
			"kills": main._run_state.kills,
			"leaks": main._run_state.leaks,
			"steps": run.steps,
		})
	var log_path := "user://run_%d.jsonl" % main.game_seed
	if str(_opts["out"]) != "":
		log_path = _resolve_path(str(_opts["out"]))
		DirAccess.make_dir_recursive_absolute(log_path.get_base_dir())
	var flush_err: int = main.telemetry.flush(log_path)
	if flush_err != OK:
		main.queue_free()
		await process_frame
		return _fail_run("flush failed (%d) -> %s" % [flush_err, log_path])
	await process_frame  # let queue_frees settle before sampling load metrics
	var metrics := run.metrics()
	metrics["log"] = log_path
	metrics["mismatch"] = run.verify_log(log_path)
	main.queue_free()
	await process_frame
	return metrics


func _compare_runs(a: Dictionary, b: Dictionary, run_index: int) -> void:
	var diff := ""
	for key in COMPARE_KEYS:
		if a[key] != b[key]:
			diff += "%s (%s != %s) " % [key, str(a[key]), str(b[key])]
	if diff == "":
		return
	print("REPEAT MISMATCH: ", diff)
	print("  run 1: ", _tuple(a))
	print("  run %d: %s" % [run_index, _tuple(b)])
	_fail_invariant("repeat mismatch: " + diff)


func _check_invariants(m: Dictionary) -> void:
	if str(m.get("terminal", "")) == "guard":
		_fail_invariant("guard budget exhausted (no loss/cap in %d steps)" % int(m.get("steps", 0)))
	if str(m.get("mismatch", "")) != "":
		_fail_invariant("log mismatch: %s" % str(m.get("mismatch", "")))
	if str(m.get("terminal", "")) == "loss" and int(m.get("money", 0)) >= 0:
		_fail_invariant("loss terminal with non-negative money (%d)" % int(m.get("money", 0)))


func _print_metrics(index: int, m: Dictionary) -> void:
	if m.has("error"):
		print("HARNESS run %d FAILED: %s" % [index, m["error"]])
		return
	var text := (
		"HARNESS seed=%d waves=%d run=%d/%d terminal=%s mission_cleared=%s"
		+ " kills=%d leaks=%d money=%d first_leak=%d first_bankruptcy=%d"
		+ " steps=%d sim=%.1fs nodes=%d entities=%d fx=%d decals=%d"
	) % [
		int(_opts["seed"]), int(_opts["waves"]), index, int(_opts["repeat"]),
		str(m["terminal"]), str(m["mission_cleared"]), int(m["kills"]),
		int(m["leaks"]), int(m["money"]), int(m["first_leak_wave"]),
		int(m["first_bankruptcy_wave"]), int(m["steps"]),
		float(m["steps"]) * HarnessRun.STEP, int(m["nodes"]),
		int(m["entities"]), int(m["fx"]), int(m["decals"]),
	]
	print(text)
	print("  log: " + str(m.get("log", "")))


func _tuple(m: Dictionary) -> String:
	var parts: Array[String] = []
	for key in COMPARE_KEYS:
		parts.append("%s=%s" % [key, str(m[key])])
	return " ".join(parts)


func _resolve_path(path: String) -> String:
	# Accept `reports/x.jsonl` like the docs show; keep res:// and absolute paths as-is.
	if path.begins_with("res://") or path.is_absolute_path():
		return path
	return "res://" + path


func _fail(message: String) -> bool:
	push_error("harness.gd: " + message)
	_failed = true
	quit(1)
	return false


func _fail_run(message: String) -> Dictionary:
	push_error("harness.gd: " + message)
	_failed = true
	return {"error": message}


func _fail_invariant(message: String) -> void:
	push_error("harness.gd: " + message)
	_failed = true
