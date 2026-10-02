class_name Gun
extends RefCounted
## Tower with a five-level upgrade path (`GunUpgrades` table, ADR 0012): stats
## come from the level; the scene owns placement and upgrades. Grid-space
## position (cell units); targets are Drones. Priority is the target closest to
## its own exit, so imminent leaks get shot first.

var position: Vector2
var level := 1
var cooldown := 0.0
var range_bonus := 0.0  # per-wave modifier hook (e.g. blackout), scene-set


func _init(p_position: Vector2, p_level: int = 1) -> void:
	position = p_position
	level = clampi(p_level, 1, GunUpgrades.MAX_LEVEL)


func damage() -> float:
	return float(GunUpgrades.stats(level)["damage"])


func fire_interval() -> float:
	return float(GunUpgrades.stats(level)["interval"])


func base_range() -> float:
	return float(GunUpgrades.stats(level)["range"])


func in_range(target: Drone) -> bool:
	return position.distance_to(target.position) <= base_range() + range_bonus


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
	cooldown = fire_interval()
	return target
