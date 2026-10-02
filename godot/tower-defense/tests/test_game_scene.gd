extends GutTest
## Scene smoke: Main wiring end-to-end (spawn → walk → shoot → kill),
## stepped manually so the assertions stay frame-rate independent.

const STEP := 1.0 / 60.0


func _spawn_all_and_clear(game) -> void:
	# Drain the spawn queue, then leak every drone: the wave ends without
	# combat so the chaining flow is tested in isolation.
	while not game._spawn_queue.is_empty():
		game._process(STEP)
	for d in game._drones.duplicate():
		game._on_leak(d)


func test_wave_spawns_walks_and_gun_kills() -> void:
	var game = load("res://scenes/Main.tscn").instantiate()
	add_child_autofree(game)
	game._start_wave(1)
	for i in 240:
		game._process(STEP)
	assert_eq(game._drones.size(), 4, "Wave 1 spawns its four drones")
	assert_gt(game._drones[0].position.x, 0.5, "Drones walk the path")

	var cell := Vector2i(9, 5)
	game.economy.earn(Economy.GUN_COST)
	assert_true(game.economy.spend(Economy.GUN_COST))
	assert_true(game.maze.build(cell))
	game._guns[cell] = Gun.new(Drone.center_of(cell))
	game._show_tower(cell)

	var kills := 0
	for i in 600:
		game._process(STEP)
		kills = 4 - game._drones.size()
		if kills >= 2:
			break
	assert_gt(kills, 1, "Gun kills at least two drones in transit")
	assert_eq(game.economy.money, 100 + kills * Economy.KILL_REWARD, "Kills pay out")


func test_wave_end_starts_break_then_auto_chains() -> void:
	var game = load("res://scenes/Main.tscn").instantiate()
	add_child_autofree(game)
	game._start_wave(1)
	_spawn_all_and_clear(game)
	game._process(STEP)
	assert_false(game._wave_running, "Wave ends when queue and field are empty")
	assert_gt(game._break_timer, 0.0, "Break starts after the wave")

	for i in ceili(game.BREAK_SECONDS / STEP) + 2:
		game._process(STEP)
	assert_eq(game._wave, 2, "Next wave auto-starts after the break")
	assert_true(game._wave_running, "Wave 2 is running")


func test_space_skips_break() -> void:
	var game = load("res://scenes/Main.tscn").instantiate()
	add_child_autofree(game)
	game._start_wave(1)
	_spawn_all_and_clear(game)
	game._process(STEP)
	assert_gt(game._break_timer, 0.0, "Break is running")

	var ev := InputEventKey.new()
	ev.keycode = KEY_SPACE
	ev.pressed = true
	game._unhandled_input(ev)
	assert_eq(game._wave, 2, "Space starts the next wave during the break")
	assert_true(game._wave_running, "Wave 2 is running")
	assert_eq(game._break_timer, 0.0, "Break is over")


func test_double_leak_same_frame_records_single_run_end() -> void:
	var game = load("res://scenes/Main.tscn").instantiate()
	add_child_autofree(game)
	var base_only: Array[Vector2i] = [game.maze.base_cell]
	# Two drones with a base-only path leak in the same frame; the first one
	# ends the run, the second must not re-trigger _end_run.
	game._drones.append(Drone.spawn("normal", base_only, 20.0, 1.0))
	game._drones.append(Drone.spawn("normal", base_only, 20.0, 1.0))
	game.economy.money = -1
	game._process(STEP)
	assert_true(game._game_over, "First leak ends the run")
	assert_eq(game._drones.size(), 1, "Leak processing stops at game over")
	var path := "user://run_%d.jsonl" % game.GAME_SEED
	var content := FileAccess.get_file_as_string(path)
	assert_eq(content.count("\"run_end\""), 1, "Exactly one run_end event is recorded")
	DirAccess.remove_absolute(ProjectSettings.globalize_path(path))


func test_game_over_shows_screen_and_flushes_telemetry() -> void:
	var game = load("res://scenes/Main.tscn").instantiate()
	add_child_autofree(game)
	game.economy.money = -1
	game._end_run()
	assert_true(game._game_over)
	assert_true(game._game_over_screen.visible, "Game-over screen is shown")
	assert_true(game._game_over_stats.text.contains("Geld"), "Summary shows the run stats")
	assert_true(
		game._restart_button.pressed.is_connected(game._restart),
		"Restart button is wired to _restart"
	)
	var path := "user://run_%d.jsonl" % game.GAME_SEED
	assert_true(FileAccess.file_exists(path), "Run log is flushed")
	var content := FileAccess.get_file_as_string(path)
	assert_true(content.contains("\"run_end\""), "Log ends with the run_end event")
	DirAccess.remove_absolute(ProjectSettings.globalize_path(path))
