class_name Projectile
extends RefCounted
## Homing shot: travels grid-space toward a Drone and reports the hit.
## Fizzles when the target dies or leaks mid-flight.

const SPEED := 14.0

var position: Vector2
var target: Drone
var damage: float
var alive := true


func _init(p_position: Vector2, p_target: Drone, p_damage: float) -> void:
	position = p_position
	target = p_target
	damage = p_damage


func advance(delta: float) -> bool:
	if not alive:
		return false
	if target == null or not target.alive or target.finished:
		alive = false
		return false
	var to_target := target.position - position
	var step := SPEED * delta
	if to_target.length() <= step:
		position = target.position
		alive = false
		return true
	position += to_target.normalized() * step
	return false
