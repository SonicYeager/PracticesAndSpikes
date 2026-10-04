class_name Drone
extends RefCounted
## Grid-space walker: follows a cell path at cells/second and leaks when it
## reaches its exit (the last path cell). Position is in cell units
## (e.g. (2.5, 3.5) = center of cell (2, 3)), so movement stays independent
## of scene origin and tile size.

const KIND_MODS := {
	"normal": {"hp": 1.0, "speed": 1.0},
	"fast": {"hp": 0.6, "speed": 1.6},
	"tank": {"hp": 2.4, "speed": 0.55},
	"splitter": {"hp": 1.0, "speed": 0.85},
	"child": {"hp": 0.4, "speed": 1.25},
}

var kind: String
var path: Array[Vector2i] = []
var path_index := 0
var position: Vector2
var hp: float
var max_hp: float
var speed: float
var alive := true
var finished := false


static func spawn(p_kind: String, p_path: Array[Vector2i], base_hp: float, base_speed: float) -> Drone:
	var mods: Dictionary = KIND_MODS.get(p_kind, KIND_MODS["normal"])
	return Drone.new(p_kind, p_path, base_hp * float(mods["hp"]), base_speed * float(mods["speed"]))


static func center_of(cell: Vector2i) -> Vector2:
	return Vector2(cell) + Vector2(0.5, 0.5)


func _init(p_kind: String, p_path: Array[Vector2i], p_hp: float, p_speed: float) -> void:
	kind = p_kind
	path = p_path
	hp = p_hp
	max_hp = p_hp
	speed = p_speed
	path_index = 1
	position = center_of(path[0])


func cell() -> Vector2i:
	return Vector2i(position.floor())


func advance(delta: float) -> bool:
	if finished or not alive:
		return false
	if path_index >= path.size():
		finished = true
		return true
	var target := center_of(path[path_index])
	var to_target := target - position
	var step := speed * delta
	if to_target.length() <= step:
		position = target
		path_index += 1
		if path_index >= path.size():
			finished = true
			return true
	else:
		position += to_target.normalized() * step
	return false


func take_damage(amount: float) -> bool:
	if not alive:
		return false
	hp -= amount
	if hp <= 0.0:
		hp = 0.0
		alive = false
		return true
	return false


func reroute(new_path: Array[Vector2i]) -> void:
	if new_path.is_empty():
		return
	path = new_path
	path_index = 1
	if path_index >= path.size():
		finished = true


func exit_cell() -> Vector2i:
	return path[path.size() - 1]


func facing() -> Vector2:
	if path_index < path.size():
		var direction := center_of(path[path_index]) - position
		if direction.length() > 0.001:
			return direction.normalized()
	return Vector2.RIGHT


func distance_to_exit() -> float:
	return position.distance_to(center_of(path[path.size() - 1]))
