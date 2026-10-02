extends GutTest
## Drone path walking, damage, kinds and reroute (grid-space, no scene).

const PATH: Array[Vector2i] = [Vector2i(0, 0), Vector2i(1, 0), Vector2i(2, 0)]


func test_spawns_at_path_start() -> void:
	var d := Drone.new("normal", PATH, 20.0, 1.0)
	assert_eq(d.position, Vector2(0.5, 0.5))
	assert_eq(d.cell(), Vector2i(0, 0))
	assert_false(d.finished)
	assert_true(d.alive)


func test_walks_to_base_and_leaks() -> void:
	var d := Drone.new("normal", PATH, 20.0, 1.0)
	assert_false(d.advance(1.5), "Still walking after reaching waypoint 1")
	assert_eq(d.position, Vector2(1.5, 0.5))
	assert_true(d.advance(1.0), "Reaching the base is a leak")
	assert_true(d.finished)
	assert_eq(d.position, Vector2(2.5, 0.5))


func test_kind_modifiers() -> void:
	var fast := Drone.spawn("fast", PATH, 20.0, 1.0)
	assert_almost_eq(fast.hp, 12.0, 0.001)
	assert_almost_eq(fast.speed, 1.6, 0.001)
	var tank := Drone.spawn("tank", PATH, 20.0, 1.0)
	assert_almost_eq(tank.hp, 48.0, 0.001)
	assert_almost_eq(tank.speed, 0.55, 0.001)


func test_take_damage_and_death() -> void:
	var d := Drone.new("normal", PATH, 20.0, 1.0)
	assert_false(d.take_damage(8.0))
	assert_almost_eq(d.hp, 12.0, 0.001)
	assert_true(d.take_damage(12.0))
	assert_false(d.alive)
	assert_false(d.take_damage(1.0), "Dead drones take no further damage")
	assert_false(d.advance(1.0), "Dead drones do not move")


func test_facing_follows_path_direction() -> void:
	var d := Drone.new("normal", PATH, 20.0, 1.0)
	assert_eq(d.facing(), Vector2(1, 0))
	var detour: Array[Vector2i] = [Vector2i(0, 0), Vector2i(0, 1)]
	d.reroute(detour)
	assert_eq(d.facing(), Vector2(0, 1))


func test_reroute_onto_base_cell_is_finished() -> void:
	var d := Drone.new("normal", PATH, 20.0, 1.0)
	d.reroute([Vector2i(2, 0)])
	assert_true(d.finished, "A path that is only the base cell counts as leaked")
	assert_false(d.advance(1.0))


func test_reroute_keeps_position_and_follows_new_path() -> void:
	var d := Drone.new("normal", PATH, 20.0, 1.0)
	d.advance(1.5)
	var detour: Array[Vector2i] = [Vector2i(1, 0), Vector2i(1, 1), Vector2i(2, 1), Vector2i(2, 0)]
	d.reroute(detour)
	assert_eq(d.position, Vector2(1.5, 0.5), "Reroute does not teleport")
	assert_false(d.advance(0.5))
	assert_eq(d.position, Vector2(1.5, 1.0), "Follows the detour instead")
	assert_eq(d.cell(), Vector2i(1, 1))
