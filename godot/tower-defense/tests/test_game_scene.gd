extends GutTest
## Scene smoke: Main wiring end-to-end (spawn → walk → shoot → kill),
## stepped manually so the assertions stay frame-rate independent.

const STEP := 1.0 / 60.0


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


func test_game_over_shows_label_and_flushes_telemetry() -> void:
	var game = load("res://scenes/Main.tscn").instantiate()
	add_child_autofree(game)
	game.economy.money = -1
	game._end_run()
	assert_true(game._game_over)
	assert_true(game._game_over_label.visible)
	var path := "user://run_%d.jsonl" % game.GAME_SEED
	assert_true(FileAccess.file_exists(path), "Run log is flushed")
	var content := FileAccess.get_file_as_string(path)
	assert_true(content.contains("\"run_end\""), "Log ends with the run_end event")
	DirAccess.remove_absolute(ProjectSettings.globalize_path(path))
