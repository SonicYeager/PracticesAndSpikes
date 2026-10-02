extends GutTest
## Homing projectile: hits the target, fizzles when it dies mid-flight.

const PATH: Array[Vector2i] = [Vector2i(0, 0), Vector2i(5, 0)]


func _still_drone() -> Drone:
	var d := Drone.new("normal", PATH, 20.0, 0.0)
	d.path_index = 1
	d.position = Vector2(3.5, 0.5)
	return d


func test_homing_hit() -> void:
	var d := _still_drone()
	var p := Projectile.new(Vector2(0.5, 0.5), d, 8.0)
	assert_false(p.advance(0.1), "Still flying")
	assert_gt(p.position.x, 0.5, "Moved toward the target")
	assert_true(p.advance(1.0), "Reaches the target")
	assert_eq(p.position, d.position)
	assert_eq(p.damage, 8.0, "Damage is carried by the tracer")
	assert_false(p.alive)


func test_fizzles_when_target_dies() -> void:
	var d := _still_drone()
	var p := Projectile.new(Vector2(0.5, 0.5), d, 8.0)
	d.take_damage(100.0)
	assert_false(p.advance(0.1))
	assert_false(p.alive, "Dead target ends the shot")
