extends GutTest
## Finite-run frame (T18): goal guard, win/loss transitions, counters.


func test_default_goal_and_clamp() -> void:
	assert_eq(RunState.new().goal, RunState.DEFAULT_GOAL)
	assert_eq(RunState.DEFAULT_GOAL, 20, "M1 first balance pass")
	assert_eq(RunState.new(0).goal, 1, "Goal clamps to at least 1")


func test_win_registers_exactly_once_at_the_goal() -> void:
	var state := RunState.new(3)
	assert_false(state.register_wave_cleared(2), "Below the goal is not a win")
	assert_true(state.register_wave_cleared(3), "Goal wave clears the mission")
	assert_eq(state.result, RunState.Result.WIN)
	assert_eq(state.cleared_wave, 3)
	assert_false(state.register_wave_cleared(4), "A second clear is not a win")
	assert_true(state.is_mission_won())


func test_goal_below_current_wave_still_wins_once() -> void:
	var state := RunState.new(2)
	assert_true(state.register_wave_cleared(2))
	assert_false(state.register_wave_cleared(3), "Only the first clear wins")
	assert_true(RunState.new(2).register_wave_cleared(5), "Overshooting the goal wins too")


func test_endless_blocks_further_wins() -> void:
	var state := RunState.new(2)
	state.endless = true
	assert_false(state.register_wave_cleared(5), "No win re-trigger in endless")


func test_loss_only_from_none_and_counters() -> void:
	var state := RunState.new()
	state.register_kill()
	state.register_kill()
	state.register_leak()
	assert_eq(state.kills, 2)
	assert_eq(state.leaks, 1)
	state.register_loss()
	assert_eq(state.result, RunState.Result.LOSS)
	assert_false(state.is_mission_won())
	state.result = RunState.Result.WIN
	state.register_loss()
	assert_eq(state.result, RunState.Result.WIN, "A won mission stays won")
