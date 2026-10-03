extends GutTest
## Balance harness (T20): HarnessRun core against the real Main scene.
## The tool wrapper owns args/IO/exit; the core is stepped directly here.

const COMPARE_KEYS := [
	"terminal", "mission_cleared", "waves_started", "waves_cleared", "kills",
	"leaks", "money", "first_leak_wave", "first_bankruptcy_wave", "steps",
]


func _make_game(seed_value: int = 1):
	var game = load("res://scenes/Main.tscn").instantiate()
	game.seed_override = seed_value
	add_child_autofree(game)
	game.set_process(false)  # the harness drives _process; frames must not
	return game


func _telemetry_path(game) -> String:
	return "user://run_%d.jsonl" % game.game_seed


func _cleanup(game) -> void:
	DirAccess.remove_absolute(ProjectSettings.globalize_path(_telemetry_path(game)))


func _run_to_end(game, cap: int) -> HarnessRun:
	var run := HarnessRun.new(game, cap)
	while not run.step():
		pass
	return run


func test_run_reaches_cap_with_towers() -> void:
	var game = _make_game()
	game.economy.money = 500
	assert_eq(HarnessRun.apply_builds(game, "9,3;10,3;11,3", ""), "")
	game._on_wave_pressed()
	var run := _run_to_end(game, 2)
	assert_eq(run.terminal, "cap")
	assert_eq(run.waves_cleared, 2)
	assert_lt(run.steps, run.max_steps, "Cap reached inside the guard budget")


func test_loss_terminal_and_money() -> void:
	var game = _make_game()
	game.economy.money = 5
	game._on_wave_pressed()
	var run := _run_to_end(game, 5)
	assert_eq(run.terminal, "loss")
	assert_lt(game.economy.money, 0)
	assert_eq(run.first_leak_wave, 1)
	assert_true(run.first_bankruptcy_wave >= 1)
	assert_eq(run.verify_log(_telemetry_path(game)), "", "Loss log matches memory")
	_cleanup(game)


func test_determinism_two_runs_identical() -> void:
	var game_a = _make_game()
	game_a.economy.money = 500
	assert_eq(HarnessRun.apply_builds(game_a, "9,3;10,3;11,3", ""), "")
	game_a._on_wave_pressed()
	var run_a := _run_to_end(game_a, 2)
	var game_b = _make_game()
	game_b.economy.money = 500
	assert_eq(HarnessRun.apply_builds(game_b, "9,3;10,3;11,3", ""), "")
	game_b._on_wave_pressed()
	var run_b := _run_to_end(game_b, 2)
	var a := run_a.metrics()
	var b := run_b.metrics()
	for key in COMPARE_KEYS:
		assert_eq(a[key], b[key], "Same seed + builds: '%s' matches" % key)


func test_auto_endless_after_mission() -> void:
	var game = _make_game()
	game._run_state.goal = 1
	game.economy.money = 300
	assert_eq(HarnessRun.apply_builds(game, "9,3", ""), "")
	game._on_wave_pressed()
	var run := _run_to_end(game, 2)
	assert_true(run.mission_cleared, "Win screen auto-continues into endless")
	assert_eq(run.terminal, "cap")
	assert_eq(run.waves_cleared, 2)
	_cleanup(game)


func test_verify_log_matches_memory() -> void:
	var game = _make_game()
	game._on_wave_pressed()
	var run := _run_to_end(game, 1)
	assert_eq(run.terminal, "cap")
	game.telemetry.event("harness_end", {
		"wave": game._director.wave,
		"reason": run.terminal,
		"money": game.economy.money,
		"kills": game._run_state.kills,
		"leaks": game._run_state.leaks,
		"steps": run.steps,
	})
	game.telemetry.flush(_telemetry_path(game))
	assert_eq(run.verify_log(_telemetry_path(game)), "", "Log matches memory")
	_cleanup(game)


func test_frame_invariance_requires_set_process_off() -> void:
	var game = _make_game()
	game._on_wave_pressed()
	var run := HarnessRun.new(game, 5)
	for i in 120:
		run.step()
	assert_gt(game._drones.size(), 0, "Drones are on the field")
	var pos: Vector2 = game._drones[0].position
	var money: int = game.economy.money
	var kills: int = game._run_state.kills
	var wave: int = game._director.wave
	await get_tree().process_frame
	assert_eq(game._drones[0].position, pos, "Cleanup frames must not engine-step the sim")
	assert_eq(game.economy.money, money)
	assert_eq(game._run_state.kills, kills)
	assert_eq(game._director.wave, wave)


func test_apply_builds_rejects_bad_spec_and_cells() -> void:
	var game = _make_game()
	assert_ne(HarnessRun.apply_builds(game, "junk", ""), "", "Malformed spec fails loudly")
	assert_ne(HarnessRun.apply_builds(game, "9,3,4", ""), "", "Three numbers fail loudly")
	var entry: Vector2i = game.maze.entries[0]
	assert_ne(
		HarnessRun.apply_builds(game, "%d,%d" % [entry.x, entry.y], ""),
		"",
		"Entry cells are not buildable"
	)
