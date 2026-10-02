extends GutTest
## Upgrade table: cumulative prices, deltas, refunds, clamping (ADR 0012).


func test_table_shape_and_monotonicity() -> void:
	assert_eq(GunUpgrades.LEVELS.size(), GunUpgrades.MAX_LEVEL, "Five levels")
	var last_price := 0
	var last_damage := 0.0
	for level in range(1, GunUpgrades.MAX_LEVEL + 1):
		var price := GunUpgrades.price(level)
		var damage: float = GunUpgrades.stats(level)["damage"]
		assert_gt(price, last_price, "Price rises with the level")
		assert_gt(damage, last_damage, "Damage rises with the level")
		last_price = price
		last_damage = damage


func test_upgrade_cost_is_the_price_delta() -> void:
	assert_eq(GunUpgrades.upgrade_cost(1), 20)
	assert_eq(GunUpgrades.upgrade_cost(2), 35)
	assert_eq(GunUpgrades.upgrade_cost(3), 60)
	assert_eq(GunUpgrades.upgrade_cost(4), 110)
	assert_eq(GunUpgrades.upgrade_cost(5), 0, "Max level has no next upgrade")


func test_refund_is_half_of_the_cumulative_price() -> void:
	assert_eq(GunUpgrades.refund(1), 12)
	assert_eq(GunUpgrades.refund(2), 22)
	assert_eq(GunUpgrades.refund(5), 125)
	assert_lt(GunUpgrades.refund(2), GunUpgrades.price(2), "Refund never exceeds the invest")


func test_base_price_matches_the_build_cost() -> void:
	assert_eq(GunUpgrades.price(1), Economy.GUN_COST, "Level 1 price is the gun cost")


func test_levels_are_clamped() -> void:
	assert_eq(GunUpgrades.stats(0)["name"], GunUpgrades.stats(1)["name"], "Below level 1 clamps")
	assert_eq(
		GunUpgrades.stats(99)["name"],
		GunUpgrades.stats(GunUpgrades.MAX_LEVEL)["name"],
		"Above max clamps"
	)
	assert_eq(GunUpgrades.display_name(1), "KANONE")
	assert_eq(GunUpgrades.display_name(5), "LANZE")
	assert_ne(GunUpgrades.description(5), "", "The signature has a description")
