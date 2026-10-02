extends GutTest
## Maze rules: last-path builds rejected, entries/exits reserved, sell
## restores, per-entry reachability (multi-entry/exit).


func test_cannot_block_last_path() -> void:
	# 3x1 corridor: (1,0) is the only route from the entry to the exit.
	var m := Maze.new(Vector2i(3, 1), [Vector2i(0, 0)], [Vector2i(2, 0)])
	assert_false(m.can_build(Vector2i(1, 0)))
	assert_false(m.build(Vector2i(1, 0)))
	assert_eq(m.tower_at(Vector2i(1, 0)), "")


func test_cannot_build_on_entries_or_exits() -> void:
	var m := Maze.new(Vector2i(5, 5), [Vector2i(0, 0)], [Vector2i(4, 4)])
	assert_false(m.build(Vector2i(0, 0)), "Entry is reserved")
	assert_false(m.build(Vector2i(4, 4)), "Exit is reserved")


func test_build_and_sell_restores_path() -> void:
	var m := Maze.new(Vector2i(5, 5), [Vector2i(0, 2)], [Vector2i(4, 2)])
	assert_true(m.build(Vector2i(2, 1), "gun"))
	assert_true(m.build(Vector2i(2, 3), "gun"))
	assert_eq(m.tower_at(Vector2i(2, 1)), "gun")
	assert_true(m.sell(Vector2i(2, 1)))
	assert_eq(m.tower_at(Vector2i(2, 1)), "")
	assert_true(m.pathfinder.has_path(Vector2i(0, 2), Vector2i(4, 2)), "Maze stays solvable")


func test_double_build_and_ghost_sell_rejected() -> void:
	var m := Maze.new(Vector2i(5, 5), [Vector2i(0, 2)], [Vector2i(4, 2)])
	assert_true(m.build(Vector2i(2, 2), "gun"))
	assert_false(m.build(Vector2i(2, 2), "gun"), "Occupied tile rejected")
	assert_false(m.sell(Vector2i(3, 3)), "Selling empty tile rejected")
	assert_false(m.build(Vector2i(99, 99), "gun"), "Out-of-bounds rejected")


func test_terrain_blockers_are_solid_and_reserved() -> void:
	var m := Maze.new(Vector2i(5, 5), [Vector2i(0, 2)], [Vector2i(4, 2)], [Vector2i(2, 2)])
	assert_false(m.can_build(Vector2i(2, 2)), "Blocker is not buildable")
	assert_false(m.build(Vector2i(2, 2)))
	assert_false(m.sell(Vector2i(2, 2)), "Blocker cannot be sold")
	assert_true(m.pathfinder.is_solid(Vector2i(2, 2)), "Blocker stays solid")


func test_clear_blocker_opens_a_terrain_cell() -> void:
	var m := Maze.new(Vector2i(5, 5), [Vector2i(0, 2)], [Vector2i(4, 2)], [Vector2i(2, 2)])
	assert_true(m.clear_blocker(Vector2i(2, 2)))
	assert_false(m.blockers.has(Vector2i(2, 2)))
	assert_false(m.pathfinder.is_solid(Vector2i(2, 2)))
	assert_false(m.is_reserved(Vector2i(2, 2)))
	assert_true(m.can_build(Vector2i(2, 2)), "Cleared cell is buildable")
	assert_false(m.clear_blocker(Vector2i(2, 2)), "Double clear rejected")
	assert_false(m.clear_blocker(Vector2i(0, 2)), "Entry is not a blocker")


func test_entry_needs_one_open_exit() -> void:
	# Two exits: sealing one is allowed (it becomes inert), sealing the last
	# one is rejected.
	var m := Maze.new(Vector2i(5, 3), [Vector2i(0, 1)], [Vector2i(4, 0), Vector2i(4, 2)])
	assert_true(m.build(Vector2i(4, 1)), "Blocking the exit column is allowed")
	assert_true(m.build(Vector2i(3, 0)), "First exit may be sealed")
	assert_false(m.build(Vector2i(3, 2)), "Entry must keep one exit reachable")


func test_creative_split_with_two_entries_allowed() -> void:
	# A full wall across the map is fine: each entry keeps its own exit.
	var m := Maze.new(
		Vector2i(5, 5),
		[Vector2i(0, 1), Vector2i(0, 3)],
		[Vector2i(4, 1), Vector2i(4, 3)],
	)
	for x in 5:
		assert_true(m.build(Vector2i(x, 2)), "Wall cell (%d,2) keeps both entries connected" % x)
