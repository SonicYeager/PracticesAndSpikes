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
	var moving := 0
	for d in game._drones:
		if d.position != Drone.center_of(d.path[0]):
			moving += 1
	assert_gt(moving, 0, "Drones walk the path")

	# Three guns cover every row of the mid-column; scatter spawns must cross it.
	for cell in [Vector2i(9, 2), Vector2i(9, 5), Vector2i(9, 8)]:
		game.economy.earn(Economy.GUN_COST)
		assert_true(game.economy.spend(Economy.GUN_COST))
		assert_true(game.maze.build(cell))
		game._guns[cell] = Gun.new(Drone.center_of(cell))
		game._show_tower(cell)

	var tracked: Array = game._drones.duplicate()
	var kills := 0
	var leaks := 0
	for i in 2400:
		game._process(STEP)
		kills = 0
		leaks = 0
		for d in tracked:
			if d.finished:
				leaks += 1
			elif not d.alive:
				kills += 1
		if kills >= 2:
			break
	assert_gt(kills, 1, "Guns kill at least two drones in transit")
	assert_eq(
		kills + leaks + game._drones.size(),
		tracked.size(),
		"Drones are killed, leaked or still alive"
	)
	assert_eq(
		game.economy.money,
		100 + kills * Economy.KILL_REWARD - leaks * Economy.LEAK_COST,
		"Kills pay out, leaks cost"
	)


func test_wave_end_starts_break_then_auto_chains() -> void:
	var game = load("res://scenes/Main.tscn").instantiate()
	add_child_autofree(game)
	game._start_wave(1)
	_spawn_all_and_clear(game)
	game._process(STEP)
	assert_eq(game._phase, game.Phase.BREAK, "Wave end starts the break")
	assert_gt(game._break_timer, 0.0, "Break timer counts down")

	for i in ceili(game.BREAK_SECONDS / STEP) + 2:
		game._process(STEP)
	assert_eq(game._wave, 2, "Next wave auto-starts after the break")
	assert_eq(game._phase, game.Phase.RUNNING, "Wave 2 is running")


func test_space_skips_break() -> void:
	var game = load("res://scenes/Main.tscn").instantiate()
	add_child_autofree(game)
	game._start_wave(1)
	_spawn_all_and_clear(game)
	game._process(STEP)
	assert_eq(game._phase, game.Phase.BREAK, "Break is running")

	var ev := InputEventKey.new()
	ev.keycode = KEY_SPACE
	ev.pressed = true
	game._unhandled_input(ev)
	assert_eq(game._wave, 2, "Space starts the next wave during the break")
	assert_eq(game._phase, game.Phase.RUNNING, "Wave 2 is running")
	assert_eq(game._break_timer, 0.0, "Break is over")


func test_double_leak_same_frame_records_single_run_end() -> void:
	var game = load("res://scenes/Main.tscn").instantiate()
	add_child_autofree(game)
	var exit_only: Array[Vector2i] = [game.maze.exits[0]]
	# Two drones with an exit-only path leak in the same frame; the first one
	# ends the run, the second must not re-trigger _end_run.
	game._drones.append(Drone.spawn("normal", exit_only, 20.0, 1.0))
	game._drones.append(Drone.spawn("normal", exit_only, 20.0, 1.0))
	game.economy.money = -1
	game._process(STEP)
	assert_eq(game._phase, game.Phase.GAME_OVER, "First leak ends the run")
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
	assert_eq(game._phase, game.Phase.GAME_OVER)
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


func test_shake_offsets_and_decays() -> void:
	var game = load("res://scenes/Main.tscn").instantiate()
	add_child_autofree(game)
	assert_not_null(game.get_node_or_null("Camera"), "Camera exists")
	var vignette := game.get_node_or_null("Hud/Vignette") as TextureRect
	assert_not_null(vignette, "Vignette overlay exists")
	assert_not_null(vignette.texture, "Vignette texture is wired")
	game._add_shake(1.0)
	game._process(STEP)
	assert_gt(game._camera.offset.length(), 0.5, "Shake offsets the camera")
	for i in 120:
		game._process(STEP)
	assert_lt(game._camera.offset.length(), 0.001, "Shake decays back to rest")

	# The shake also decays while the world is frozen at game over.
	game.economy.money = -1
	game._end_run()
	game._process(STEP)
	assert_gt(game._camera.offset.length(), 0.5, "Game-over shake offsets the camera")
	for i in 120:
		game._process(STEP)
	assert_lt(game._camera.offset.length(), 0.001, "Shake decays during game over")
	DirAccess.remove_absolute(
		ProjectSettings.globalize_path("user://run_%d.jsonl" % game.GAME_SEED)
	)


func test_scatter_spawn_is_deterministic() -> void:
	var first = load("res://scenes/Main.tscn").instantiate()
	add_child_autofree(first)
	var second = load("res://scenes/Main.tscn").instantiate()
	add_child_autofree(second)
	for game in [first, second]:
		game._start_wave(1)
		for i in 240:
			game._process(STEP)
	assert_eq(first._drones.size(), 4)
	var rows := {}
	for i in 4:
		assert_eq(first._drones[i].path, second._drones[i].path, "Same seed → same scatter")
		rows[first._drones[i].path[0].y] = true
	assert_gt(rows.size(), 1, "Spawns scatter across the entry side")


func test_fallback_to_nearest_exit_when_target_cut() -> void:
	var game = load("res://scenes/Main.tscn").instantiate()
	add_child_autofree(game)
	# A drone whose (arbitrary) assigned target gets walled off must re-route
	# to a reachable exit instead.
	var entry := Vector2i(0, 5)
	var target := Vector2i(10, 6)
	var cells: Array[Vector2i] = []
	for p in game.maze.pathfinder.find_path(entry, target):
		cells.append(Vector2i(p))
	game._drones.append(Drone.spawn("normal", cells, 20.0, 1.0))
	assert_eq(game._drones[0].exit_cell(), target)
	for cell in [Vector2i(9, 6), Vector2i(11, 6), Vector2i(10, 5), Vector2i(10, 7)]:
		assert_true(game.maze.build(cell), "Sealing cell %s is allowed" % str(cell))
	game._reroute_drones()
	var d: Drone = game._drones[0]
	assert_ne(d.exit_cell(), target, "Cut-off target is replaced")
	assert_true(game.maze.exits.has(d.exit_cell()), "New target is a real exit")
	assert_false(
		game.maze.pathfinder.find_path(d.cell(), d.exit_cell()).is_empty(),
		"Fallback path exists"
	)
