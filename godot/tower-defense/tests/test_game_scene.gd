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


func _find_clearable(game) -> Vector2i:
	for cell in game.maze.blockers:
		if not game._vents.has(cell):
			return cell
	return Vector2i(-1, -1)


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
	assert_true(game._hud.is_game_over_visible(), "Game-over screen is shown")
	assert_true(game._hud.game_over_text().contains("GELD"), "Summary shows the run stats")
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
