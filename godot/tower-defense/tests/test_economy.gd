extends GutTest
## Money-is-HP: kills earn, leaks cost, game over strictly below zero.


func test_game_over_only_below_zero() -> void:
	var e := Economy.new(10)
	e.on_leak() # 10 - 10 = 0
	assert_eq(e.money, 0)
	assert_false(e.is_game_over(), "Zero money is not game over yet")
	e.on_leak() # -10
	assert_true(e.is_game_over(), "Negative money ends the run")


func test_kill_earns_leak_costs() -> void:
	var e := Economy.new(100)
	e.on_kill()
	assert_eq(e.money, 100 + Economy.KILL_REWARD)
	e.on_leak()
	assert_eq(e.money, 100 + Economy.KILL_REWARD - Economy.LEAK_COST)


func test_spend_requires_funds() -> void:
	var e := Economy.new(5)
	assert_false(e.spend(Economy.GUN_COST), "Cannot spend what you lack")
	assert_eq(e.money, 5, "Failed spend leaves money untouched")
	assert_true(e.spend(5))
	assert_eq(e.money, 0)


func test_on_kill_accepts_a_reward_override() -> void:
	var e := Economy.new(10)
	e.on_kill(8)
	assert_eq(e.money, 18, "Bounty modifier pays a custom reward")
	e.on_kill()
	assert_eq(e.money, 24, "Default reward unchanged")


func test_wall_costs_less_than_the_gun() -> void:
	assert_eq(Economy.WALL_COST, 10, "First balance pass")
	assert_lt(Economy.WALL_COST, Economy.GUN_COST, "The wall is the cheap role")
