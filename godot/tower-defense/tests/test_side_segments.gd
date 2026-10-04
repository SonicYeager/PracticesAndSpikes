extends GutTest
## Entry/exit segments (T21): config -> deterministic cell lists.

const SIZE := Vector2i(20, 12)


func _cells(segments: Array) -> Array[Vector2i]:
	return SideSegments.cells(SIZE, segments)


func test_full_sides_keep_the_old_order() -> void:
	var left := _cells([{"side": SideSegments.Side.LEFT}])
	assert_eq(left.size(), 12)
	assert_eq(left[0], Vector2i(0, 0))
	assert_eq(left[11], Vector2i(0, 11))
	var right := _cells([{"side": SideSegments.Side.RIGHT}])
	assert_eq(right.size(), 12)
	assert_eq(right[0], Vector2i(19, 0))
	assert_eq(right[11], Vector2i(19, 11))
	var top := _cells([{"side": SideSegments.Side.TOP}])
	assert_eq(top.size(), 20)
	assert_eq(top[0], Vector2i(0, 0))
	assert_eq(top[19], Vector2i(19, 0))
	var bottom := _cells([{"side": SideSegments.Side.BOTTOM}])
	assert_eq(bottom.size(), 20)
	assert_eq(bottom[0], Vector2i(0, 11))
	assert_eq(bottom[19], Vector2i(19, 11))


func test_ranges_use_the_side_axis() -> void:
	var top_expected: Array[Vector2i] = [Vector2i(3, 0), Vector2i(4, 0), Vector2i(5, 0)]
	assert_eq(_cells([{"side": SideSegments.Side.TOP, "from": 3, "to": 5}]), top_expected)
	var right_expected: Array[Vector2i] = [Vector2i(19, 2), Vector2i(19, 3)]
	assert_eq(_cells([{"side": SideSegments.Side.RIGHT, "from": 2, "to": 3}]), right_expected)
	var bottom_expected: Array[Vector2i] = [Vector2i(1, 11), Vector2i(2, 11)]
	assert_eq(_cells([{"side": SideSegments.Side.BOTTOM, "from": 1, "to": 2}]), bottom_expected)


func test_open_ended_single_and_float_ranges() -> void:
	var from_only: Array[Vector2i] = [Vector2i(18, 11), Vector2i(19, 11)]
	assert_eq(_cells([{"side": SideSegments.Side.BOTTOM, "from": 18}]), from_only)
	var to_only: Array[Vector2i] = [Vector2i(0, 11), Vector2i(1, 11)]
	assert_eq(_cells([{"side": SideSegments.Side.BOTTOM, "to": 1}]), to_only)
	var single: Array[Vector2i] = [Vector2i(0, 4)]
	assert_eq(_cells([{"side": SideSegments.Side.LEFT, "from": 4, "to": 4}]), single)
	var floats: Array[Vector2i] = [Vector2i(0, 2), Vector2i(0, 3)]
	assert_eq(_cells([{"side": SideSegments.Side.LEFT, "from": 2.9, "to": 3.1}]), floats)


func test_clamps_and_normalizes() -> void:
	var clamped: Array[Vector2i] = [Vector2i(0, 0), Vector2i(0, 1), Vector2i(0, 2)]
	assert_eq(_cells([{"side": SideSegments.Side.LEFT, "from": -5, "to": 2}]), clamped)
	var reversed: Array[Vector2i] = [
		Vector2i(3, 0), Vector2i(4, 0), Vector2i(5, 0), Vector2i(6, 0), Vector2i(7, 0),
	]
	assert_eq(_cells([{"side": SideSegments.Side.TOP, "from": 7, "to": 3}]), reversed)
	var oversize: Array[Vector2i] = [Vector2i(19, 0)]
	assert_eq(_cells([{"side": SideSegments.Side.TOP, "from": 50, "to": 99}]), oversize)


func test_multi_segment_order_and_dedupe() -> void:
	var expected: Array[Vector2i] = [Vector2i(0, 0), Vector2i(0, 1), Vector2i(1, 0), Vector2i(0, 2)]
	assert_eq(
		_cells([
			{"side": SideSegments.Side.LEFT, "from": 0, "to": 1},
			{"side": SideSegments.Side.TOP, "from": 0, "to": 1},
			{"side": SideSegments.Side.LEFT, "from": 1, "to": 2},
		]),
		expected
	)


func test_invalid_configs_yield_no_cells() -> void:
	assert_true(_cells([]).is_empty())
	assert_true(_cells([{"side": 99}]).is_empty())
	assert_true(_cells([{"from": 0, "to": 3}]).is_empty())
	assert_true(_cells([42]).is_empty())
	assert_true(_cells([{"side": "TOP"}]).is_empty(), "String sides are rejected, not coerced")
	assert_true(_cells([{"side": null}]).is_empty())
	assert_true(_cells([{"side": 1.9}]).is_empty(), "Float sides are rejected")


func test_bounds_are_numeric_only() -> void:
	var fallback_from: Array[Vector2i] = [Vector2i(0, 0), Vector2i(0, 1), Vector2i(0, 2)]
	assert_eq(
		_cells([{"side": SideSegments.Side.LEFT, "from": "oops", "to": 2}]),
		fallback_from,
		"A non-numeric from falls back to the side start"
	)
	assert_eq(_cells([{"side": SideSegments.Side.LEFT, "to": "oops"}]).size(), 12)
	assert_eq(_cells([{"side": SideSegments.Side.TOP, "from": [], "to": {}}]).size(), 20)


func test_one_wide_map_clamps() -> void:
	var expected: Array[Vector2i] = [Vector2i(0, 0)]
	assert_eq(SideSegments.cells(Vector2i(1, 4), [{"side": SideSegments.Side.TOP}]), expected)
