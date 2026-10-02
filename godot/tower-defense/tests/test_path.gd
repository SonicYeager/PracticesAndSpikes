extends GutTest
## Pathfinder: open grids connect, walls block, sizes vary.


func test_open_grid_connected() -> void:
	var p := Pathfinder.new()
	p.setup(Vector2i(10, 8))
	assert_true(p.has_path(Vector2i(0, 0), Vector2i(9, 7)))


func test_full_wall_disconnects() -> void:
	var p := Pathfinder.new()
	p.setup(Vector2i(5, 5))
	for y in 5:
		p.set_solid(Vector2i(2, y), true)
	assert_false(p.has_path(Vector2i(0, 2), Vector2i(4, 2)))


func test_wall_with_gap_stays_connected() -> void:
	var p := Pathfinder.new()
	p.setup(Vector2i(5, 5))
	for y in 5:
		if y != 2:
			p.set_solid(Vector2i(2, y), true)
	var path := p.find_path(Vector2i(0, 2), Vector2i(4, 2))
	assert_false(path.is_empty())
	assert_has(path, Vector2i(2, 2), "Path must route through the gap")


func test_variable_sizes() -> void:
	for s in [Vector2i(8, 6), Vector2i(20, 12), Vector2i(30, 20)]:
		var p := Pathfinder.new()
		p.setup(s)
		assert_true(
			p.has_path(Vector2i.ZERO, s - Vector2i(1, 1)),
			"Open grid %s must connect corners" % str(s)
		)


func test_reachable_from_multiple_sources() -> void:
	var p := Pathfinder.new()
	p.setup(Vector2i(5, 5))
	for y in 5:
		p.set_solid(Vector2i(2, y), true)
	var right := p.reachable_from([Vector2i(4, 2)])
	assert_true(right.has(Vector2i(4, 0)))
	assert_false(right.has(Vector2i(0, 2)), "Wall blocks the left side")
	var both := p.reachable_from([Vector2i(0, 0), Vector2i(4, 4)])
	assert_true(both.has(Vector2i(1, 1)))
	assert_true(both.has(Vector2i(3, 3)))
