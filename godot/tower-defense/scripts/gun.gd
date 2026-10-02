class_name Gun
extends RefCounted
## Placement-only tower: fixed range, damage and cadence.
## Grid-space position (cell units); targets are Drones. Priority is the
## target closest to its own exit, so imminent leaks get shot first.

const RANGE := 3.5
const DAMAGE := 8.0
const INTERVAL := 0.6

var position: Vector2
var cooldown := 0.0


func _init(p_position: Vector2) -> void:
	position = p_position


func in_range(target: Drone) -> bool:
	return position.distance_to(target.position) <= RANGE


func acquire(targets: Array) -> Drone:
	var best: Drone = null
	var best_distance := INF
	for t in targets:
		var d := t as Drone
		if d == null or not d.alive or d.finished:
			continue
		if not in_range(d):
			continue
		var distance := d.distance_to_exit()
		if distance < best_distance:
			best_distance = distance
			best = d
	return best


func try_fire(delta: float, targets: Array) -> Drone:
	cooldown = maxf(cooldown - delta, 0.0)
	if cooldown > 0.0:
		return null
	var target := acquire(targets)
	if target == null:
		return null
	cooldown = INTERVAL
	return target
