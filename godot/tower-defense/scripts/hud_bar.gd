class_name GameHudBar
extends Control
## Segmented wave-progress bar (T12): XT-style blocks that fill as the wave
## resolves. Pure drawing, no textures; state is pushed via `set_progress`.

const SEGMENTS := 24
const GAP := 2.0
const COLOR_FILL := Color(0.557, 0.878, 0.29)   # acid green (XT palette)
const COLOR_EMPTY := Color(0.106, 0.145, 0.188) # dark slot
const COLOR_EDGE := Color(0.184, 0.231, 0.29)

var _done := 0
var _total := 0


func set_progress(done: int, total: int) -> void:
	if done == _done and total == _total:
		return
	_done = done
	_total = total
	queue_redraw()


func _draw() -> void:
	var seg_w := (size.x - GAP * (SEGMENTS - 1)) / SEGMENTS
	var filled := 0.0
	if _total > 0:
		filled = clampf(float(_done) / float(_total), 0.0, 1.0) * SEGMENTS
	for i in SEGMENTS:
		var x := i * (seg_w + GAP)
		var rect := Rect2(x, 0, seg_w, size.y)
		draw_rect(rect, COLOR_EMPTY)
		var amount := clampf(filled - float(i), 0.0, 1.0)
		if amount > 0.0:
			draw_rect(Rect2(x, 0, seg_w * amount, size.y), COLOR_FILL)
		draw_rect(rect, COLOR_EDGE, false, 1.0)
