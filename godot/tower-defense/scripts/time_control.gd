class_name TimeControl
extends RefCounted
## Pause/speed state (T19): pure state machine — the scene applies it
## (Engine.time_scale) and owns input, HUD and telemetry.

const SPEEDS: Array[float] = [1.0, 2.0, 3.0]

var paused := false
var speed_index := 0


func scale() -> float:
	## Effective engine time scale: 0 while paused, else the selected speed.
	return 0.0 if paused else SPEEDS[speed_index]


func is_paused() -> bool:
	return paused


func toggle_pause() -> void:
	paused = not paused


func cycle_speed() -> void:
	speed_index = (speed_index + 1) % SPEEDS.size()


func speed() -> float:
	return SPEEDS[speed_index]


func speed_text() -> String:
	## HUD label: "×2"/"×3"; empty at ×1.
	var value := speed()
	return "" if value == 1.0 else "×%d" % int(value)
