extends GutTest
## Scene smoke: Main wiring end-to-end (spawn → walk → shoot → kill),
## stepped manually so the assertions stay frame-rate independent.
## All scene tests pin `seed_override` so terrain and scatter are stable.

const STEP := 1.0 / 60.0


func _make_game(seed_value: int = 1):
	var game = load("res://scenes/Main.tscn").instantiate()
	game.seed_override = seed_value
	add_child_autofree(game)
	return game


func _spawn_all_and_clear(game) -> void:
	# Drain the spawn queue, then leak every drone: the wave ends without
	# combat so the chaining flow is tested in isolation.
	while not game._director.queue.is_empty():
		game._process(STEP)
	for d in game._drones.duplicate():
		game._on_leak(d)


func _telemetry_path(game) -> String:
	return "user://run_%d.jsonl" % game.game_seed


func _find_buildable(game, row: int = 2) -> Vector2i:
	for offset in [0, -1, 1, -2, 2, -3, 3]:
		var cell := Vector2i(9 + offset, row)
		if game.maze.can_build(cell):
			return cell
	return Vector2i(-1, -1)


func _make_game_with_segments(entry_segments: Array, exit_segments: Array):
	var game = load("res://scenes/Main.tscn").instantiate()
	game.seed_override = 1
	game.entry_segments = entry_segments
	game.exit_segments = exit_segments
	add_child_autofree(game)
	return game


func _find_clearable(game) -> Vector2i:
	for cell in game.maze.blockers:
		if not game._vents.has(cell):
			return cell
	return Vector2i(-1, -1)


func _visible_pips(pips: Node2D) -> int:
	var count := 0
	for child in pips.get_children():
		if (child as Sprite2D).visible:
			count += 1
	return count


func _press_key(game, code) -> void:
	var ev := InputEventKey.new()
	ev.keycode = code
	ev.pressed = true
	game._unhandled_input(ev)


func after_each() -> void:
	# Pause tests drive Engine.time_scale; a leftover 0 would hang GUT's
	# per-run SceneTimer await (gut.gd:890) and test-local scaled waits.
	Engine.time_scale = 1.0


func test_wave_spawns_walks_and_gun_kills() -> void:
	var game = _make_game()
	game._start_wave(1)
	for i in 240:
		game._process(STEP)
	assert_eq(game._drones.size(), 4, "Wave 1 spawns its four drones")
	var moving := 0
	for d in game._drones:
		if d.position != Drone.center_of(d.path[0]):
			moving += 1
	assert_gt(moving, 0, "Drones walk the path")

	# Guns near the mid-column; scatter spawns must cross it. Terrain can
	# block the exact cells, so each row picks the nearest buildable one.
	var gun_cells: Array[Vector2i] = []
	for row in [2, 5, 8]:
		for offset in [0, -1, 1, -2, 2, -3, 3]:
			var cell := Vector2i(9 + offset, row)
			if game.maze.can_build(cell):
				gun_cells.append(cell)
				break
	assert_gt(gun_cells.size(), 2, "Three gun spots found around the mid-column")
	for cell in gun_cells:
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
	var game = _make_game()
	game._start_wave(1)
	_spawn_all_and_clear(game)
	game._process(STEP)
	assert_eq(game._director.phase, WaveDirector.Phase.BREAK, "Wave end starts the break")
	assert_gt(game._director.break_timer, 0.0, "Break timer counts down")

	for i in ceili(WaveDirector.BREAK_SECONDS / STEP) + 2:
		game._process(STEP)
	assert_eq(game._director.wave, 2, "Next wave auto-starts after the break")
	assert_eq(game._director.phase, WaveDirector.Phase.RUNNING, "Wave 2 is running")


func test_space_skips_break() -> void:
	var game = _make_game()
	game._start_wave(1)
	_spawn_all_and_clear(game)
	game._process(STEP)
	assert_eq(game._director.phase, WaveDirector.Phase.BREAK, "Break is running")

	var ev := InputEventKey.new()
	ev.keycode = KEY_SPACE
	ev.pressed = true
	game._unhandled_input(ev)
	assert_eq(game._director.wave, 2, "Space starts the next wave during the break")
	assert_eq(game._director.phase, WaveDirector.Phase.RUNNING, "Wave 2 is running")
	assert_eq(game._director.break_timer, 0.0, "Break is over")
	var sends := 0
	for line in game.telemetry.lines:
		if line.contains("\"t\":\"send\""):
			sends += 1
	assert_eq(sends, 1, "The Space skip sends the next wave")


func test_double_leak_same_frame_records_single_run_end() -> void:
	var game = _make_game()
	var exit_only: Array[Vector2i] = [game.maze.exits[0]]
	# Two drones with an exit-only path leak in the same frame; the first one
	# ends the run, the second must not re-trigger _end_run.
	game._drones.append(Drone.spawn("normal", exit_only, 20.0, 1.0))
	game._drones.append(Drone.spawn("normal", exit_only, 20.0, 1.0))
	game.economy.money = -1
	game._process(STEP)
	assert_eq(game._director.phase, WaveDirector.Phase.GAME_OVER, "First leak ends the run")
	assert_eq(game._drones.size(), 1, "Leak processing stops at game over")
	var path := _telemetry_path(game)
	var content := FileAccess.get_file_as_string(path)
	assert_eq(content.count("\"run_end\""), 1, "Exactly one run_end event is recorded")
	assert_false(content.contains("\"wave_end\""), "No wave summary for a run that died mid-wave")
	DirAccess.remove_absolute(ProjectSettings.globalize_path(path))


func test_game_over_shows_screen_and_flushes_telemetry() -> void:
	var game = _make_game()
	game.economy.money = -1
	game._end_run()
	assert_eq(game._director.phase, WaveDirector.Phase.GAME_OVER)
	assert_true(game._hud.is_run_end_visible(), "End screen is shown")
	assert_true(game._hud.run_end_text().contains("GELD"), "Summary shows the run stats")
	assert_true(
		game._hud.restart_pressed.is_connected(game._restart),
		"Restart intent is wired to _restart"
	)
	var path := _telemetry_path(game)
	assert_true(FileAccess.file_exists(path), "Run log is flushed")
	var content := FileAccess.get_file_as_string(path)
	assert_true(content.contains("\"run_end\""), "Log ends with the run_end event")
	DirAccess.remove_absolute(ProjectSettings.globalize_path(path))


func test_shake_offsets_and_decays() -> void:
	var game = _make_game()
	assert_not_null(game.get_node_or_null("Camera"), "Camera exists")
	var vignette := game.get_node_or_null("Hud/Vignette") as TextureRect
	assert_not_null(vignette, "Vignette overlay exists")
	assert_not_null(vignette.texture, "Vignette texture is wired")
	game._fx.shake(1.0)
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
	DirAccess.remove_absolute(ProjectSettings.globalize_path(_telemetry_path(game)))


func test_scatter_spawn_is_deterministic() -> void:
	var first = _make_game(1)
	var second = _make_game(1)
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
	var game = _make_game()
	# Blockers are irrelevant to the fallback logic: use a clean maze so the
	# sealing cells are guaranteed free.
	game.maze = Maze.new(game.MAP_SIZE, game.maze.entries, game.maze.exits)
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


func test_terrain_blocks_are_solid_and_never_seal() -> void:
	var game = _make_game()
	assert_gt(game.maze.blockers.size(), 0, "Seed 1 places terrain")
	var reachable: Dictionary = game.maze.pathfinder.reachable_from(game.maze.exits)
	for cell in game.maze.blockers:
		assert_false(game.maze.can_build(cell), "Blocker %s is not buildable" % str(cell))
		assert_false(reachable.has(cell), "Blocker %s is solid" % str(cell))
	for entry in game.maze.entries:
		assert_true(reachable.has(entry), "Entry %s still reaches an exit" % str(entry))


func test_different_seeds_dress_different_maps() -> void:
	var first = _make_game(1)
	var second = _make_game(2)
	assert_ne(first.game_seed, second.game_seed)
	assert_ne(first.maze.blockers, second.maze.blockers, "Seeds dress different terrain")


func test_leak_leaves_a_decal_and_vent_only_ambient() -> void:
	var game = _make_game()
	var emitters := 0
	for child in game._board.get_children():
		if child is CPUParticles2D:
			emitters += 1
	assert_gt(game._vents.size(), 0, "Every map has at least one vent")
	assert_eq(emitters, game._vents.size(), "Only vents emit; decor stays static")
	var exit_only: Array[Vector2i] = [game.maze.exits[0]]
	game._drones.append(Drone.spawn("normal", exit_only, 20.0, 1.0))
	game._process(STEP)
	assert_eq(game._board._decals.size(), 1, "A leak leaves a skid decal")


func test_seed_override_pins_the_run_seed() -> void:
	var game = _make_game(7)
	assert_eq(game.game_seed, 7, "seed_override pins game_seed")
	assert_true(game.telemetry.lines[0].contains("\"seed\":7"), "Run start logs the seed")
	assert_true(game.telemetry.lines[0].contains("\"source\":\"local\""), "Run start tags the source")
	assert_true(game.telemetry.lines[0].contains("\"harness\":false"), "Local runs are not harness runs")


func test_decal_cap_evicts_oldest() -> void:
	var game = _make_game()
	# Spread across cells (per-cell cap 2) so the global cap is what evicts.
	for i in BoardView.DECAL_CAP + 10:
		var cell := Vector2i(i % 20, (i / 20) % 12)
		game._board.add_decal(game.DEBRIS_TEX, Vector2(cell))
	assert_eq(game._board._decals.size(), BoardView.DECAL_CAP, "Decals are capped globally")


func test_decal_per_cell_cap() -> void:
	var game = _make_game()
	game._board.add_decal(game.DEBRIS_TEX, Vector2(5, 5))
	var oldest: Sprite2D = game._board._decals[0]
	for i in 4:
		game._board.add_decal(game.DEBRIS_TEX, Vector2(5, 5))
	assert_eq(
		game._board._decals_by_cell[Vector2i(5, 5)].size(),
		BoardView.DECAL_CELL_CAP,
		"A cell keeps only its cap"
	)
	assert_true(oldest.is_queued_for_deletion(), "The oldest decal is freed")
	assert_false(
		game._board._decals_by_cell[Vector2i(5, 5)].has(oldest),
		"And removed from the cell list"
	)
	assert_eq(game._board._decals.size(), BoardView.DECAL_CELL_CAP, "Overflow is freed")


func test_overcharge_spends_and_damages_nearby_drones() -> void:
	var game = _make_game()
	assert_gt(game._vents.size(), 0, "Every map has at least one vent")
	var vent: Vector2i = game._vents.keys()[0]
	var drone := Drone.new("normal", [vent], 20.0, 1.0)
	game._drones.append(drone)
	var money_before: int = game.economy.money
	assert_true(game._try_overcharge(vent), "Overcharge fires")
	assert_eq(
		game.economy.money,
		money_before - game.OVERCHARGE_COST,
		"Overcharge costs money"
	)
	assert_almost_eq(
		drone.hp,
		20.0 - game.OVERCHARGE_DAMAGE,
		0.001,
		"Nearby drones take damage"
	)
	assert_false(game._try_overcharge(vent), "Cooldown blocks a second burst")
	assert_false(game._try_overcharge(Vector2i(9, 9)), "Non-vent cells are ignored")


func test_overcharge_kill_emits_kill_event() -> void:
	var game = _make_game()
	var vent: Vector2i = game._vents.keys()[0]
	# A fast drone has 12 hp at wave-1 base hp — dies to the 15 overcharge damage.
	var path: Array[Vector2i] = [vent]
	var drone := Drone.spawn("fast", path, 20.0, 1.0)
	game._drones.append(drone)
	assert_true(game._try_overcharge(vent), "Overcharge fires")
	var kill_line := ""
	for line in game.telemetry.lines:
		if line.contains("\"t\":\"kill\""):
			kill_line = line
	assert_true(kill_line.contains("\"kind\":\"fast\""), "Overcharge kill emits a kill event")


func test_wave_event_logs_the_modifier() -> void:
	var game = _make_game()
	game._start_wave(1)
	var found := false
	for line in game.telemetry.lines:
		if line.contains("\"t\":\"wave\""):
			found = true
			assert_true(line.contains("\"modifier\""), "Wave event carries the modifier")
	assert_true(found, "Wave event was logged")


func test_kill_wave_summary_and_send_events() -> void:
	var game = _make_game()
	game._on_wave_pressed()
	# Drain the spawn queue, then kill one drone and leak the rest.
	while not game._director.queue.is_empty():
		game._process(STEP)
	var victim: Drone = game._drones[0]
	game._apply_damage(victim, 999.0)
	for d in game._drones.duplicate():
		game._on_leak(d)
	game._process(STEP)
	var kill_line := ""
	var wave_end_line := ""
	for line in game.telemetry.lines:
		if line.contains("\"t\":\"kill\""):
			kill_line = line
		if line.contains("\"t\":\"wave_end\""):
			wave_end_line = line
	assert_true(kill_line.contains("\"kind\""), "Kill event carries the drone kind")
	assert_true(kill_line.contains("\"cell\""), "Kill event carries the cell")
	assert_true(wave_end_line.contains("\"kills\":1"), "One kill counted in the wave summary")
	assert_true(wave_end_line.contains("\"leaks\":3"), "Three leaks counted in the wave summary")
	assert_true(wave_end_line.contains("\"money_start\":100"), "Money at wave start is frozen")
	assert_true(wave_end_line.contains("\"money_end\":76"), "Kill +6, three leaks -30")
	# The break auto-starts wave 2 without another send event.
	for i in ceili(WaveDirector.BREAK_SECONDS / STEP) + 2:
		game._process(STEP)
	assert_eq(game._director.wave, 2, "Wave 2 auto-started")
	var sends := 0
	for line in game.telemetry.lines:
		if line.contains("\"t\":\"send\""):
			sends += 1
	assert_eq(sends, 1, "Only the player start sends; the auto-chain does not")
	# Wave 2: per-wave counters must start from zero again (2 kills, not 3).
	while not game._director.queue.is_empty():
		game._process(STEP)
	for i in 2:
		game._apply_damage(game._drones[0], 999.0)
	for d in game._drones.duplicate():
		game._on_leak(d)
	game._process(STEP)
	var wave2_line := ""
	for line in game.telemetry.lines:
		if line.contains("\"t\":\"wave_end\"") and line.contains("\"wave\":2"):
			wave2_line = line
	assert_true(wave2_line.contains("\"kills\":2"), "Wave 2 counters reset (2 kills, not 3)")
	assert_true(wave2_line.contains("\"leaks\":4"), "Wave 2 leak counter is fresh")


func test_harness_run_provenance() -> void:
	var game = load("res://scenes/Main.tscn").instantiate()
	game.seed_override = 1
	game.run_source = "harness"
	add_child_autofree(game)
	assert_true(game.telemetry.lines[0].contains("\"source\":\"harness\""), "Harness source logged")
	assert_true(game.telemetry.lines[0].contains("\"harness\":true"), "Harness flag logged")


func test_start_wave_applies_the_seeded_modifier() -> void:
	var rush = _make_game(121)
	assert_eq(WaveGen.composition(7, 121)["modifier"], "rush")
	rush._start_wave(7)
	assert_almost_eq(
		rush._director.spawn_interval,
		WaveDirector.SPAWN_INTERVAL * 0.6,
		0.001,
		"Rush wave tightens pacing"
	)
	var blackout = _make_game(222)
	assert_eq(WaveGen.composition(5, 222)["modifier"], "blackout")
	blackout._start_wave(5)
	assert_almost_eq(blackout._director.range_bonus, -1.0, 0.001, "Blackout wave shrinks range")
	var bounty = _make_game(121)
	assert_eq(WaveGen.composition(3, 121)["modifier"], "bounty")
	bounty._start_wave(3)
	assert_eq(bounty._director.kill_reward, Economy.KILL_REWARD + 2, "Bounty wave pays more")


func test_blackout_updates_existing_guns() -> void:
	var game = _make_game()
	game._guns[Vector2i(1, 1)] = Gun.new(Vector2(1.5, 1.5))
	game._director.range_bonus = -1.0
	game._apply_wave_knobs()
	assert_almost_eq(
		game._guns[Vector2i(1, 1)].range_bonus, -1.0, 0.001, "Existing guns get the modifier"
	)
	game._director.range_bonus = 0.0
	game._apply_wave_knobs()
	assert_almost_eq(
		game._guns[Vector2i(1, 1)].range_bonus, 0.0, 0.001, "And it resets next wave"
	)


func test_try_build_places_gun_pays_and_puffs() -> void:
	var game = _make_game()
	var cell := _find_buildable(game)
	assert_ne(cell, Vector2i(-1, -1), "Seed 1 has a buildable cell near the mid-column")
	var money_before: int = game.economy.money
	var fx_before: int = game._fx.get_child_count()
	assert_true(game._try_build(cell), "Build succeeds")
	assert_eq(game.economy.money, money_before - Economy.GUN_COST, "Build pays the gun cost")
	assert_true(game.maze.built.has(cell), "Maze tracks the tower")
	assert_true(game._guns.has(cell), "Gun exists")
	assert_true(game._tower_nodes.has(cell), "Tower sprite exists")
	assert_gt(game._fx.get_child_count(), fx_before, "Build puffs dust")
	var build_lines := 0
	for line in game.telemetry.lines:
		if line.contains("\"t\":\"build\""):
			build_lines += 1
	assert_eq(build_lines, 1, "Build is logged once")
	var money_after: int = game.economy.money
	assert_false(game._try_build(cell), "Occupied cell is rejected")
	assert_eq(game.economy.money, money_after, "Rejected build refunds")


func test_try_build_rejects_drone_cell_and_broke_player() -> void:
	var game = _make_game()
	var cell := _find_buildable(game)
	assert_ne(cell, Vector2i(-1, -1))
	game._drones.append(Drone.spawn("normal", [cell], 20.0, 1.0))
	var money_before: int = game.economy.money
	assert_false(game._try_build(cell), "A drone on the cell blocks the build")
	assert_eq(game.economy.money, money_before, "Blocked build refunds")
	assert_false(game.maze.built.has(cell), "Nothing was placed")
	game._drones.clear()
	game.economy.money = Economy.GUN_COST - 1
	assert_false(game._try_build(cell), "Unaffordable build is denied")
	assert_eq(game.economy.money, Economy.GUN_COST - 1, "Denied build moves no money")


func test_primary_dispatch_covers_sell_vent_and_build() -> void:
	var game = _make_game()
	var cell := _find_buildable(game)
	assert_ne(cell, Vector2i(-1, -1))
	game._dispatch_primary(cell)
	assert_true(game.maze.built.has(cell), "Dispatch builds on a free cell")
	game._sell_mode = true
	var fx_before: int = game._fx.get_child_count()
	game._dispatch_primary(cell)
	assert_false(game.maze.built.has(cell), "Sell mode sells via dispatch")
	assert_gt(game._fx.get_child_count(), fx_before, "Sell puffs dust")
	game._sell_mode = false
	var vent: Vector2i = game._vents.keys()[0]
	var money_before: int = game.economy.money
	game._dispatch_primary(vent)
	assert_eq(game.economy.money, money_before, "Idle vent click does not overcharge")
	assert_eq(game._vents[vent]["cooldown"], 0.0, "No overcharge cooldown in idle")
	# Running phase: the same click must overcharge.
	game._start_wave(1)
	money_before = game.economy.money
	game._dispatch_primary(vent)
	assert_eq(
		game.economy.money,
		money_before - game.OVERCHARGE_COST,
		"Running vent click spends the overcharge"
	)
	assert_eq(
		game._vents[vent]["cooldown"],
		game.OVERCHARGE_COOLDOWN,
		"Overcharge starts its cooldown"
	)
	var overcharge_lines := 0
	for line in game.telemetry.lines:
		if line.contains("\"t\":\"overcharge\""):
			overcharge_lines += 1
	assert_eq(overcharge_lines, 1, "Overcharge is logged")


func test_kill_spawns_shockwave() -> void:
	var game = _make_game()
	game._start_wave(1)
	while not game._director.queue.is_empty():
		game._process(STEP)
	var victim: Drone = game._drones[0]
	var fx_before: int = game._fx.get_child_count()
	game._apply_damage(victim, 999.0)
	assert_eq(
		game._fx.get_child_count(), fx_before + 2,
		"Kill spawns the explosion and the shockwave ring"
	)


func test_try_clear_rock_costs_opens_and_logs() -> void:
	var game = _make_game()
	var cell := _find_clearable(game)
	assert_ne(cell, Vector2i(-1, -1), "Seed 1 has a clearable blocker")
	var money_before: int = game.economy.money
	var fx_before: int = game._fx.get_child_count()
	assert_true(game._try_clear(cell), "Clear succeeds")
	assert_eq(game.economy.money, money_before - game.ROCK_CLEAR_COST, "Clear costs money")
	assert_false(game.maze.blockers.has(cell), "Blocker is gone")
	assert_false(game.maze.pathfinder.is_solid(cell), "Cell is walkable now")
	assert_true(game.maze.can_build(cell), "Cleared cell is buildable")
	assert_false(game._board._blocker_sprites.has(cell), "Blocker sprite is removed")
	assert_gt(game._fx.get_child_count(), fx_before, "Clear puffs dust")
	var clear_line := ""
	for line in game.telemetry.lines:
		if line.contains("\"t\":\"clear\""):
			clear_line = line
	assert_true(clear_line.contains("\"cell\":[%d,%d]" % [cell.x, cell.y]), "Clear logs the cell")
	assert_true(clear_line.contains("\"money\":%d" % game.economy.money), "Clear logs the money")


func test_try_clear_rejects_vents_empty_and_broke() -> void:
	var game = _make_game()
	var vent: Vector2i = game._vents.keys()[0]
	assert_false(game._try_clear(vent), "Vents are the money sink, not clearable")
	assert_true(game.maze.blockers.has(vent), "Vent stays a blocker")
	var empty := Vector2i(-1, -1)
	for y in 3:
		for x in 3:
			var c := Vector2i(x, y)
			if game.maze.is_inside(c) and not game.maze.blockers.has(c):
				empty = c
	assert_ne(empty, Vector2i(-1, -1), "Found a non-blocker cell")
	var money_before: int = game.economy.money
	assert_false(game._try_clear(empty), "Empty cells are ignored")
	assert_eq(game.economy.money, money_before, "Ignored clear moves no money")
	var rock := _find_clearable(game)
	assert_ne(rock, Vector2i(-1, -1))
	game.economy.money = game.ROCK_CLEAR_COST - 1
	assert_false(game._try_clear(rock), "Cannot afford the clear")
	assert_true(game.maze.blockers.has(rock), "Rock stays")
	assert_eq(game.economy.money, game.ROCK_CLEAR_COST - 1, "No money moved")


func test_leak_pulses_the_hud_flash() -> void:
	var game = _make_game()
	var flash := game.get_node_or_null("Hud/HudRoot/LeakFlash") as TextureRect
	assert_not_null(flash, "Leak flash node exists")
	assert_almost_eq(flash.modulate.a, 0.0, 0.001, "Flash starts dark")
	var exit_only: Array[Vector2i] = [game.maze.exits[0]]
	game._drones.append(Drone.spawn("normal", exit_only, 20.0, 1.0))
	game._process(STEP)
	assert_gt(flash.modulate.a, 0.0, "Leak flashes the screen edge")


func test_try_upgrade_pays_and_logs() -> void:
	var game = _make_game()
	var cell := _find_buildable(game)
	assert_ne(cell, Vector2i(-1, -1))
	assert_true(game._try_build(cell))
	var money_before: int = game.economy.money
	var fx_before: int = game._fx.get_child_count()
	assert_true(game._try_upgrade(cell), "Upgrade succeeds")
	assert_eq(game.economy.money, money_before - 20, "Upgrade pays the level delta")
	assert_eq(game._guns[cell].level, 2, "The tower levels up")
	assert_gt(game._fx.get_child_count(), fx_before, "Upgrade puffs dust")
	var line := ""
	for entry in game.telemetry.lines:
		if entry.contains("\"t\":\"upgrade\""):
			line = entry
	assert_true(line.contains("\"from\":1"), "Upgrade logs the source level")
	assert_true(line.contains("\"to\":2"), "Upgrade logs the target level")
	assert_true(line.contains("\"cost\":20"), "Upgrade logs the cost")


func test_try_upgrade_rejects_broke_max_and_game_over() -> void:
	var game = _make_game()
	var cell := _find_buildable(game)
	assert_ne(cell, Vector2i(-1, -1))
	assert_false(game._try_upgrade(cell), "A cell without a tower is ignored")
	assert_true(game._try_build(cell))
	game.economy.money = 19
	assert_false(game._try_upgrade(cell), "Cannot afford the delta")
	assert_eq(game._guns[cell].level, 1, "Level unchanged when broke")
	assert_eq(game.economy.money, 19, "No money moved")
	game.economy.earn(300)
	for i in 4:
		assert_true(game._try_upgrade(cell), "Upgrade %d succeeds" % (i + 2))
	assert_eq(game._guns[cell].level, GunUpgrades.MAX_LEVEL, "Reached the max level")
	var money_at_max: int = game.economy.money
	assert_false(game._try_upgrade(cell), "Max level rejects further upgrades")
	assert_eq(game.economy.money, money_at_max, "Max rejection moves no money")
	game._director.end_run()
	assert_false(game._try_upgrade(cell), "Game over blocks upgrades")
	assert_eq(game.economy.money, money_at_max, "Game-over rejection moves no money")


func test_sell_refund_uses_the_invested_total() -> void:
	var game = _make_game()
	var cell := _find_buildable(game)
	assert_ne(cell, Vector2i(-1, -1))
	assert_true(game._try_build(cell))
	var money_after_build: int = game.economy.money
	assert_true(game._try_upgrade(cell), "Level 2 reached")
	assert_eq(game.economy.money, money_after_build - 20)
	assert_true(game._try_sell(cell))
	assert_eq(
		game.economy.money,
		money_after_build - 20 + 22,
		"Refund is half of the cumulative invest"
	)


func test_upgrade_keeps_blackout_range_bonus() -> void:
	var game = _make_game()
	var cell := _find_buildable(game)
	assert_ne(cell, Vector2i(-1, -1))
	assert_true(game._try_build(cell))
	game._guns[cell].range_bonus = -1.0
	assert_true(game._try_upgrade(cell))
	assert_almost_eq(
		game._guns[cell].range_bonus,
		-1.0,
		0.001,
		"Mutating in place keeps the blackout malus"
	)


func test_fire_carries_the_upgraded_damage() -> void:
	var game = _make_game()
	var cell := _find_buildable(game)
	assert_ne(cell, Vector2i(-1, -1))
	assert_true(game._try_build(cell))
	game.economy.earn(200)
	assert_true(game._try_upgrade(cell))
	assert_true(game._try_upgrade(cell), "Level 3 reached")
	var gun: Gun = game._guns[cell]
	assert_almost_eq(gun.damage(), 15.0, 0.001, "Level 3 damage")
	var target := Drone.spawn("normal", [cell], 20.0, 1.0)
	game._fire(cell, gun, target)
	assert_eq(game._projectiles.size(), 1, "One tracer spawned")
	assert_almost_eq(
		game._projectiles[0].damage,
		15.0,
		0.001,
		"The tracer carries the level damage"
	)


func test_primary_dispatch_selects_a_tower_and_escape_deselects() -> void:
	var game = _make_game()
	var cell := _find_buildable(game)
	assert_ne(cell, Vector2i(-1, -1))
	assert_true(game._try_build(cell))
	game._dispatch_primary(cell)
	assert_eq(game._selected, cell, "LMB on a tower selects it")
	var money_before: int = game.economy.money
	assert_eq(game.economy.money, money_before, "Selecting spends nothing")
	var ev := InputEventKey.new()
	ev.keycode = KEY_ESCAPE
	ev.pressed = true
	game._unhandled_input(ev)
	assert_eq(game._selected, Vector2i(-1, -1), "ESC deselects")


func test_upgrade_button_flow_upgrades_the_selection() -> void:
	var game = _make_game()
	var cell := _find_buildable(game)
	assert_ne(cell, Vector2i(-1, -1))
	assert_true(game._try_build(cell))
	game._select_tower(cell)
	assert_true(game._hud._upgrade_row.visible, "Selection shows the HUD row")
	game._hud._upgrade_button.pressed.emit()
	assert_eq(game._guns[cell].level, 2, "The HUD button upgrades the selected tower")


func test_level_pips_and_signature_tint() -> void:
	var game = _make_game()
	var cell := _find_buildable(game)
	assert_ne(cell, Vector2i(-1, -1))
	game.economy.earn(500)
	assert_true(game._try_build(cell))
	var pips := game._tower_nodes[cell].get_node_or_null("LevelPips") as Node2D
	assert_not_null(pips, "Pip container exists")
	assert_eq(_visible_pips(pips), 1, "Level 1 shows one pip")
	assert_true(game._try_upgrade(cell))
	assert_eq(_visible_pips(pips), 2, "Level 2 shows two pips")
	for i in 3:
		assert_true(game._try_upgrade(cell))
	assert_eq(_visible_pips(pips), 5, "Level 5 shows five pips")
	var base := game._tower_nodes[cell].get_node("Base") as Sprite2D
	assert_ne(base.modulate, Color.WHITE, "The signature tints the base")


func test_upgrade_works_during_running_and_sell_deselects() -> void:
	var game = _make_game()
	var cell := _find_buildable(game)
	assert_ne(cell, Vector2i(-1, -1))
	assert_true(game._try_build(cell))
	game._select_tower(cell)
	game._start_wave(1)
	assert_eq(game._director.phase, WaveDirector.Phase.RUNNING)
	assert_true(game._try_upgrade(cell), "Upgrades are allowed during a wave")
	assert_true(game._try_sell(cell))
	assert_eq(game._selected, Vector2i(-1, -1), "Selling the selected tower deselects it")


func test_update_hud_clears_stale_selection() -> void:
	var game = _make_game()
	var cell := _find_buildable(game)
	assert_ne(cell, Vector2i(-1, -1))
	assert_true(game._try_build(cell))
	game._select_tower(cell)
	game._guns.erase(cell)
	game._update_hud()
	assert_eq(game._selected, Vector2i(-1, -1), "Stale selection is cleared")


func test_show_tower_with_preleveled_gun() -> void:
	var game = _make_game()
	var cell := _find_buildable(game)
	assert_ne(cell, Vector2i(-1, -1))
	game._guns[cell] = Gun.new(Drone.center_of(cell), 3)
	game._show_tower(cell)
	var pips := game._tower_nodes[cell].get_node_or_null("LevelPips") as Node2D
	assert_eq(_visible_pips(pips), 3, "Pre-leveled towers render their pips")


func test_build_kind_toggles_with_b() -> void:
	var game = _make_game()
	assert_eq(game._build_kind, Pieces.Kind.GUN, "Starts as gun")
	var money_before: int = game.economy.money
	var ev := InputEventKey.new()
	ev.keycode = KEY_B
	ev.pressed = true
	game._unhandled_input(ev)
	assert_eq(game._build_kind, Pieces.Kind.WALL, "B flips to wall")
	game._unhandled_input(ev)
	assert_eq(game._build_kind, Pieces.Kind.GUN, "B flips back")
	assert_eq(game.economy.money, money_before, "Toggling spends nothing")


func test_build_wall_costs_and_blocks() -> void:
	var game = _make_game()
	var cell := _find_buildable(game)
	assert_ne(cell, Vector2i(-1, -1))
	var money_before: int = game.economy.money
	assert_true(game._try_build(cell, Pieces.Kind.WALL))
	assert_eq(game.economy.money, money_before - 10, "Wall costs 10")
	assert_true(game._walls.has(cell), "Wall is tracked")
	assert_eq(game.maze.tower_at(cell), "wall", "Built map knows the type")
	assert_true(game._wall_nodes.has(cell), "Wall sprite exists")
	assert_eq(
		(game._wall_nodes[cell] as Sprite2D).texture,
		game.WALL_TEX,
		"Wall uses the wall sprite"
	)
	var line := ""
	for entry in game.telemetry.lines:
		if entry.contains("\"t\":\"build\""):
			line = entry
	assert_true(line.contains("\"kind\":\"wall\""), "Build logs the kind")
	assert_false(game._try_upgrade(cell), "Walls have no upgrade path")
	assert_eq(game._guns.size(), 0, "No gun was created")


func test_wall_reroutes_drones() -> void:
	var game = _make_game()
	# Clean maze so the path is predictable (mirror of the fallback test).
	game.maze = Maze.new(game.MAP_SIZE, game.maze.entries, game.maze.exits)
	var entry := Vector2i(0, 5)
	var target := Vector2i(10, 6)
	var cells: Array[Vector2i] = []
	for p in game.maze.pathfinder.find_path(entry, target):
		cells.append(Vector2i(p))
	game._drones.append(Drone.spawn("normal", cells, 20.0, 1.0))
	var d: Drone = game._drones[0]
	var old_path: Array[Vector2i] = d.path.duplicate()
	var wall_cell := cells[2]
	assert_true(game._try_build(wall_cell, Pieces.Kind.WALL))
	assert_ne(d.path, old_path, "The wall re-routes the drone")
	assert_false(d.path.has(wall_cell), "The wall cell is off the new path")


func test_sell_wall_refunds_half_and_sell_mode_works() -> void:
	var game = _make_game()
	var cell := _find_buildable(game)
	assert_ne(cell, Vector2i(-1, -1))
	var money_before: int = game.economy.money
	assert_true(game._try_build(cell, Pieces.Kind.WALL))
	game._sell_mode = true
	game._dispatch_primary(cell)
	assert_eq(game.economy.money, money_before - 10 + 5, "Sell mode sells the wall for half")
	assert_false(game._walls.has(cell), "Wall bookkeeping is cleared")
	assert_false(game._wall_nodes.has(cell), "Wall sprite is gone")
	var line := ""
	for entry in game.telemetry.lines:
		if entry.contains("\"t\":\"sell\""):
			line = entry
	assert_true(line.contains("\"kind\":\"wall\""), "Sell logs the kind")


func test_dispatch_on_wall_denies_without_deselect() -> void:
	var game = _make_game()
	var gun_cell := _find_buildable(game)
	assert_ne(gun_cell, Vector2i(-1, -1))
	assert_true(game._try_build(gun_cell))
	game._select_tower(gun_cell)
	var wall_cell := _find_buildable(game, 4)
	assert_ne(wall_cell, Vector2i(-1, -1))
	assert_true(game._try_build(wall_cell, Pieces.Kind.WALL))
	var money_before: int = game.economy.money
	game._dispatch_primary(wall_cell)
	assert_eq(game._selected, gun_cell, "Clicking a wall keeps the selection")
	assert_eq(game.economy.money, money_before, "No spend on a wall click")


func test_mission_cleared_at_goal() -> void:
	var game = _make_game()
	game._run_state.goal = 1
	game.economy.money = 500
	game._start_wave(1)
	_spawn_all_and_clear(game)
	game._process(STEP)
	assert_eq(game._director.phase, WaveDirector.Phase.GAME_OVER, "The win freezes the run")
	assert_eq(game._run_state.result, RunState.Result.WIN)
	assert_true(game._hud.is_run_end_visible(), "Win screen is shown")
	assert_eq(game._hud._run_end_title.text, "SIEG")
	assert_true(game._hud._continue_button.visible, "Endless continue is offered")
	var has_cleared := false
	var cleared_count := 0
	var run_ends := 0
	for line in game.telemetry.lines:
		if line.contains("\"t\":\"mission_cleared\""):
			has_cleared = true
			cleared_count += 1
		if line.contains("\"t\":\"run_end\""):
			run_ends += 1
	assert_true(has_cleared, "Mission clear is logged")
	assert_eq(cleared_count, 1, "Mission clear is logged exactly once")
	assert_eq(run_ends, 0, "A win-stop logs no run_end")
	DirAccess.remove_absolute(ProjectSettings.globalize_path(_telemetry_path(game)))


func test_endless_continue_then_loss_logs_win_result() -> void:
	var game = _make_game()
	game._run_state.goal = 1
	game.economy.money = 500
	game._start_wave(1)
	_spawn_all_and_clear(game)
	game._process(STEP)
	assert_eq(game._run_state.result, RunState.Result.WIN)
	game._continue_endless()
	assert_eq(game._director.phase, WaveDirector.Phase.BREAK, "Continue returns to the break")
	assert_true(game._run_state.endless)
	assert_false(game._hud.is_run_end_visible(), "Win screen is hidden")
	game._start_wave(2)
	_spawn_all_and_clear(game)
	game.economy.money = -1
	var exit_only: Array[Vector2i] = [game.maze.exits[0]]
	game._drones.append(Drone.spawn("normal", exit_only, 20.0, 1.0))
	game._process(STEP)
	assert_eq(game._director.phase, WaveDirector.Phase.GAME_OVER)
	assert_eq(game._hud._run_end_title.text, "GAME OVER")
	assert_true(game._hud._run_end_note.visible, "Endless loss shows the mission note")
	assert_false(game._hud._continue_button.visible)
	var run_end := ""
	var run_ends := 0
	for line in game.telemetry.lines:
		if line.contains("\"t\":\"run_end\""):
			run_ends += 1
			run_end = line
	assert_eq(run_ends, 1, "Exactly one run_end for the endless segment")
	assert_true(run_end.contains("\"result\":\"win\""), "Mission result stays win")
	assert_true(run_end.contains("\"endless\":true"))
	DirAccess.remove_absolute(ProjectSettings.globalize_path(_telemetry_path(game)))


func test_loss_marks_result_and_run_end() -> void:
	var game = _make_game()
	game.economy.money = -1
	game._end_run()
	assert_eq(game._run_state.result, RunState.Result.LOSS)
	assert_eq(game._hud._run_end_title.text, "GAME OVER")
	assert_false(game._hud._run_end_note.visible)
	assert_false(game._hud._continue_button.visible)
	var run_end := ""
	for line in game.telemetry.lines:
		if line.contains("\"t\":\"run_end\""):
			run_end = line
	assert_true(run_end.contains("\"result\":\"loss\""))
	assert_true(run_end.contains("\"endless\":false"))
	DirAccess.remove_absolute(ProjectSettings.globalize_path(_telemetry_path(game)))


func test_bankruptcy_beats_goal_clear() -> void:
	var game = _make_game()
	game._run_state.goal = 1
	game._start_wave(1)
	_spawn_all_and_clear(game)
	game.economy.money = -1
	var exit_only: Array[Vector2i] = [game.maze.exits[0]]
	game._drones.append(Drone.spawn("normal", exit_only, 20.0, 1.0))
	game._process(STEP)
	assert_eq(game._director.phase, WaveDirector.Phase.GAME_OVER)
	assert_eq(game._run_state.result, RunState.Result.LOSS, "Bankruptcy beats the goal clear")
	assert_ne(game._hud._run_end_title.text, "SIEG", "No win screen")
	var has_cleared := false
	for line in game.telemetry.lines:
		if line.contains("\"t\":\"mission_cleared\""):
			has_cleared = true
	assert_false(has_cleared, "No mission_cleared after bankruptcy")
	DirAccess.remove_absolute(ProjectSettings.globalize_path(_telemetry_path(game)))


func test_pause_key_freezes_and_denies_wave_start() -> void:
	var game = _make_game()
	_press_key(game, KEY_P)
	assert_true(game._time_control.is_paused())
	assert_almost_eq(Engine.time_scale, 0.0, 0.001, "Engine scale is applied")
	assert_eq(game._hud._time_label.text, "PAUSE", "HUD follows the pause")
	assert_true(game._hud._time_label.visible)
	game._on_wave_pressed()
	assert_eq(game._director.phase, WaveDirector.Phase.IDLE, "Wave start is denied while paused")
	assert_eq(game._director.wave, 0)
	_press_key(game, KEY_P)
	assert_false(game._time_control.is_paused())
	assert_almost_eq(Engine.time_scale, 1.0, 0.001)
	assert_false(game._hud._time_label.visible)
	var time_lines := 0
	var payload_ok := false
	for line in game.telemetry.lines:
		if line.contains("\"t\":\"time_control\""):
			time_lines += 1
			if line.contains("\"action\":\"pause\"") and line.contains("\"paused\":true"):
				payload_ok = true
	assert_eq(time_lines, 2, "Pause and resume are logged")
	assert_true(payload_ok, "Payload carries the paused state")


func test_pause_freezes_the_break_timer_and_allows_building() -> void:
	var game = _make_game()
	game._start_wave(1)
	_spawn_all_and_clear(game)
	game._process(STEP)
	assert_eq(game._director.phase, WaveDirector.Phase.BREAK)
	assert_gt(game._director.break_timer, 0.0, "Break is running before the pause")
	_press_key(game, KEY_P)
	var left: float = game._director.break_timer
	game._process(STEP)
	assert_eq(game._director.break_timer, left, "Paused break does not tick")
	var cell := _find_buildable(game)
	assert_ne(cell, Vector2i(-1, -1))
	var money_before: int = game.economy.money
	assert_true(game._try_build(cell), "Building stays available while paused")
	assert_eq(game.economy.money, money_before - 25)


func test_pause_denies_overcharge_and_speed_cycles() -> void:
	var game = _make_game()
	var vent: Vector2i = game._vents.keys()[0]
	_press_key(game, KEY_P)
	game.economy.money = 500
	assert_false(game._try_overcharge(vent), "Overcharge is denied while paused")
	assert_eq(game.economy.money, 500, "No spend on a denied overcharge")
	_press_key(game, KEY_P)
	_press_key(game, KEY_T)
	assert_almost_eq(Engine.time_scale, 2.0, 0.001)
	_press_key(game, KEY_T)
	assert_almost_eq(Engine.time_scale, 3.0, 0.001)
	_press_key(game, KEY_T)
	assert_almost_eq(Engine.time_scale, 1.0, 0.001, "Speed wraps back to ×1")
	var speed_lines := 0
	for line in game.telemetry.lines:
		if line.contains("\"action\":\"speed\""):
			speed_lines += 1
	assert_eq(speed_lines, 3)


func test_ready_resets_a_leftover_time_scale() -> void:
	Engine.time_scale = 0.0
	var game = _make_game()
	assert_almost_eq(Engine.time_scale, 1.0, 0.001, "A fresh scene applies ×1")


func test_pause_during_running_freezes_drones() -> void:
	var game = _make_game()
	game._start_wave(1)
	for i in 60:
		game._process(STEP)
	assert_gt(game._drones.size(), 0, "Wave 1 is on the field")
	var pos: Vector2 = game._drones[0].position
	_press_key(game, KEY_P)
	game._process(STEP)
	assert_eq(game._drones[0].position, pos, "Paused drones do not move")
	_press_key(game, KEY_P)
	game._process(STEP)
	assert_ne(game._drones[0].position, pos, "Resume lets the drone move again")


func test_time_keys_are_ignored_at_game_over() -> void:
	var game = _make_game()
	game.economy.money = -1
	game._end_run()
	assert_eq(game._director.phase, WaveDirector.Phase.GAME_OVER)
	_press_key(game, KEY_P)
	_press_key(game, KEY_T)
	assert_false(game._time_control.is_paused(), "P is ignored at game over")
	assert_almost_eq(Engine.time_scale, 1.0, 0.001, "T is ignored at game over")
	DirAccess.remove_absolute(ProjectSettings.globalize_path(_telemetry_path(game)))


func test_default_topology_is_full_sides() -> void:
	var game = _make_game()
	var entries: Array[Vector2i] = []
	var exits: Array[Vector2i] = []
	for y in 12:
		entries.append(Vector2i(0, y))
		exits.append(Vector2i(19, y))
	assert_eq(game.maze.entries, entries, "Default entries stay the full left side")
	assert_eq(game.maze.exits, exits, "Default exits stay the full right side")


func test_segment_topology_scatter_and_validation() -> void:
	var game = _make_game_with_segments(
		[{"side": SideSegments.Side.TOP, "from": 4, "to": 8}],
		[{"side": SideSegments.Side.RIGHT, "from": 3, "to": 5}],
	)
	assert_eq(game.maze.entries.size(), 5)
	assert_eq(game.maze.entries[0], Vector2i(4, 0))
	assert_eq(game.maze.entries[4], Vector2i(8, 0))
	assert_eq(game.maze.exits.size(), 3)
	assert_false(game.maze.can_build(game.maze.entries[2]), "Segment entries stay unbuildable")
	game._start_wave(1)
	for i in 240:
		game._process(STEP)
	assert_gt(game._drones.size(), 0, "Segment entries spawn drones")
	for d in game._drones:
		assert_eq(d.path[0].y, 0, "Spawns use the top segment row")
		assert_true(d.path[0].x >= 4 and d.path[0].x <= 8, "Spawns stay inside the segment")
		assert_true(game.maze.exits.has(d.exit_cell()), "Assigned exits come from the segment")
	var other = _make_game_with_segments(
		[{"side": SideSegments.Side.TOP, "from": 4, "to": 8}],
		[{"side": SideSegments.Side.RIGHT, "from": 3, "to": 5}],
	)
	other._start_wave(1)
	for i in 240:
		other._process(STEP)
	assert_eq(game._drones.size(), other._drones.size())
	for i in game._drones.size():
		assert_eq(game._drones[i].path, other._drones[i].path, "Segment scatter is deterministic")


func test_fallback_lands_in_a_partial_exit_segment() -> void:
	var game = _make_game_with_segments(
		[{"side": SideSegments.Side.LEFT, "from": 4, "to": 6}],
		[{"side": SideSegments.Side.RIGHT, "from": 2, "to": 4}],
	)
	# Blockers are irrelevant to the fallback logic: use a clean maze so the
	# sealing cells are guaranteed free (same pattern as the full-side test).
	game.maze = Maze.new(game.MAP_SIZE, game.maze.entries, game.maze.exits)
	var entry := Vector2i(0, 5)
	var target := Vector2i(19, 2)
	var cells: Array[Vector2i] = []
	for p in game.maze.pathfinder.find_path(entry, target):
		cells.append(Vector2i(p))
	game._drones.append(Drone.spawn("normal", cells, 20.0, 1.0))
	assert_eq(game._drones[0].exit_cell(), target)
	# Seal the assigned exit: its floor neighbours are buildable, the second
	# segment exit (19,3) is reserved, so it is sealed low-level instead.
	assert_true(game.maze.build(Vector2i(18, 2)), "Sealing cell (18,2) is allowed")
	assert_true(game.maze.build(Vector2i(19, 1)), "Sealing cell (19,1) is allowed")
	game.maze.pathfinder.set_solid(Vector2i(19, 3), true)
	game._reroute_drones()
	var d: Drone = game._drones[0]
	assert_ne(d.exit_cell(), target, "Cut-off target is replaced")
	assert_true(game.maze.exits.has(d.exit_cell()), "Fallback stays inside the exit segment")
	assert_eq(d.exit_cell(), Vector2i(19, 4), "Fallback picks the remaining segment exit")
	assert_false(
		game.maze.pathfinder.find_path(d.cell(), d.exit_cell()).is_empty(),
		"Fallback path exists"
	)


func test_empty_segments_fall_back_to_full_sides() -> void:
	var game = _make_game_with_segments([], [])
	assert_eq(game.maze.entries.size(), 12, "Empty entry config falls back to the full side")
	assert_eq(game.maze.entries[0], Vector2i(0, 0), "Fallback is the left side")
	assert_eq(game.maze.exits.size(), 12, "Empty exit config falls back to the full side")
	assert_eq(game.maze.exits[0], Vector2i(19, 0), "Fallback is the right side")
	var mixed = _make_game_with_segments(
		[], [{"side": SideSegments.Side.RIGHT, "from": 3, "to": 5}]
	)
	assert_eq(mixed.maze.entries.size(), 12, "Entry guard is independent of the exit config")
	assert_eq(mixed.maze.exits.size(), 3, "A valid exit config is not overridden")


func test_segment_configs_are_editor_exported() -> void:
	var game = _make_game()
	var found := 0
	for prop in game.get_property_list():
		if prop["name"] == "entry_segments" or prop["name"] == "exit_segments":
			assert_true((prop["usage"] & PROPERTY_USAGE_EDITOR) != 0, "%s is editor-exported" % prop["name"])
			found += 1
	assert_eq(found, 2, "Both topology configs are editor-exported")
