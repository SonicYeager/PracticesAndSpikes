extends GutTest
## WaveDirector (T12.5): phase machine, queue, timers and the modifier knobs,
## driven without the game scene.

const SEED := 121


func test_start_fills_queue_and_phase() -> void:
	var d := WaveDirector.new(SEED)
	d.start(1)
	assert_eq(d.phase, WaveDirector.Phase.RUNNING, "Wave runs")
	assert_eq(d.wave, 1)
	assert_eq(d.total, int(d.composition["count"]), "Total matches the composition")
	assert_eq(d.queue.size(), d.total, "Queue holds every drone")
	assert_eq(d.queue.size(), 4, "Wave 1 has four drones")


func test_spawn_tick_respects_interval_and_drains_queue() -> void:
	var d := WaveDirector.new(SEED)
	d.start(1)
	assert_eq(d.tick_spawn(0.1), "normal", "The first spawn is due immediately")
	assert_eq(d.tick_spawn(0.1), "", "No spawn before the interval")
	assert_eq(d.tick_spawn(1.0), "normal", "Spawn after the interval")
	var spawned := 2
	while d.tick_spawn(1.0) != "":
		spawned += 1
	assert_eq(spawned, d.total, "The queue drains into spawns")
	assert_eq(d.tick_spawn(1.0), "", "Empty queue spawns nothing")


func test_begin_break_and_tick() -> void:
	var d := WaveDirector.new(SEED)
	d.start(1)
	d.begin_break()
	assert_eq(d.phase, WaveDirector.Phase.BREAK, "Break phase")
	assert_almost_eq(d.break_timer, WaveDirector.BREAK_SECONDS, 0.001, "Full countdown")
	assert_false(d.tick_break(1.0), "Break still running")
	assert_almost_eq(d.break_timer, WaveDirector.BREAK_SECONDS - 1.0, 0.001, "Timer counts down")
	assert_true(d.tick_break(WaveDirector.BREAK_SECONDS), "Break elapses")
	assert_eq(d.break_timer, 0.0, "Timer clamps at zero")


func test_break_previews_the_next_modifier() -> void:
	var d := WaveDirector.new(SEED)
	d.start(6)
	d.begin_break()
	assert_eq(
		d.next_modifier,
		str(WaveGen.composition(7, SEED)["modifier"]),
		"Preview matches the next composition"
	)


func test_modifier_knobs() -> void:
	var d := WaveDirector.new(1)
	d.composition = WaveGen.composition(5, 1)
	d._apply_modifier("rush")
	assert_almost_eq(
		d.spawn_interval,
		WaveDirector.SPAWN_INTERVAL * 0.6,
		0.001,
		"Rush tightens the spawn interval"
	)
	d._apply_modifier("blackout")
	assert_almost_eq(d.range_bonus, -1.0, 0.001, "Blackout shrinks gun range")
	d._apply_modifier("bounty")
	assert_eq(d.kill_reward, Economy.KILL_REWARD + 2, "Bounty raises the kill reward")
	d.composition = {"count": 4, "hp": 20.0, "fast": 0, "tanks": 0, "modifier": "swarm"}
	d._apply_modifier("swarm")
	assert_eq(d.composition["count"], 6, "Swarm adds drones")
	assert_almost_eq(d.composition["hp"], 14.0, 0.001, "Swarm weakens them")
	d._apply_modifier("")
	assert_almost_eq(d.spawn_interval, WaveDirector.SPAWN_INTERVAL, 0.001, "Knobs reset per wave")
	assert_eq(d.kill_reward, Economy.KILL_REWARD, "Kill reward resets per wave")


func test_end_run_sets_game_over() -> void:
	var d := WaveDirector.new(SEED)
	d.start(1)
	d.end_run()
	assert_eq(d.phase, WaveDirector.Phase.GAME_OVER)
