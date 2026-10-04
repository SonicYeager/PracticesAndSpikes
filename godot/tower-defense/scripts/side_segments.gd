class_name SideSegments
extends RefCounted
## Entry/exit topology (T21): a side plus an optional inclusive cell range
## along it. `cells()` converts configs into deterministic cell lists:
## config order, ascending within a segment, first-seen dedupe. The game
## reads the configs once in `_ready`; defaults stay full sides. Sides must
## be ints; non-numeric bounds fall back to the side bounds.

enum Side { LEFT, RIGHT, TOP, BOTTOM }


static func cells(map_size: Vector2i, segments: Array) -> Array[Vector2i]:
	var result: Array[Vector2i] = []
	var seen := {}
	for segment in segments:
		if not (segment is Dictionary):
			continue
		var raw_side = segment.get("side", null)
		if typeof(raw_side) != TYPE_INT:
			continue
		var side := int(raw_side)
		var axis_len := _axis_length(side, map_size)
		if axis_len == 0:
			continue
		var first := clampi(_as_int(segment.get("from", 0), 0), 0, axis_len - 1)
		var last := clampi(_as_int(segment.get("to", axis_len - 1), axis_len - 1), 0, axis_len - 1)
		for index in range(mini(first, last), maxi(first, last) + 1):
			var cell := _cell(side, index, map_size)
			if not seen.has(cell):
				seen[cell] = true
				result.append(cell)
	return result


static func _as_int(value, fallback: int) -> int:
	## Numeric bounds only; anything else keeps the side's default bound.
	if typeof(value) == TYPE_INT or typeof(value) == TYPE_FLOAT:
		return int(value)
	return fallback


static func _axis_length(side: int, map_size: Vector2i) -> int:
	match side:
		Side.LEFT, Side.RIGHT:
			return map_size.y
		Side.TOP, Side.BOTTOM:
			return map_size.x
	return 0


static func _cell(side: int, index: int, map_size: Vector2i) -> Vector2i:
	match side:
		Side.LEFT:
			return Vector2i(0, index)
		Side.RIGHT:
			return Vector2i(map_size.x - 1, index)
		Side.TOP:
			return Vector2i(index, 0)
		Side.BOTTOM:
			return Vector2i(index, map_size.y - 1)
	return Vector2i(-1, -1)
