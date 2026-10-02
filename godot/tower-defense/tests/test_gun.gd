extends GutTest
## Gun targeting + cadence: fixed range, nearest-to-base priority.

const PATH: Array[Vector2i] = [
	Vector2i(0, 0), Vector2i(1, 0), Vector2i(2, 0), Vector2i(3, 0), Vector2i(4, 0),
	Vector2i(5, 0), Vector2i(6, 0), Vector2i(7, 0), Vector2i(8, 0), Vector2i(9, 0),
	Vector2i(10, 0),
]


func _drone_at(index: int) -> Drone:
	var d := Drone.new("normal", PATH, 20.0, 1.0)
	d.path_index = index
	d.position = Drone.center_of(PATH[index])
	return d


func test_out_of_range_never_fires() -> void:
	var gun := Gun.new(Vector2(0.5, 0.5))
	var far := _drone_at(10)
	assert_false(gun.in_range(far))
	assert_null(gun.try_fire(1.0, [far]))


func test_fires_and_respects_cooldown() -> void:
	var gun := Gun.new(Vector2(0.5, 0.5))
	var near := _drone_at(1)
	assert_true(gun.in_range(near))
	assert_eq(gun.try_fire(0.0, [near]), near, "Ready gun fires immediately")
	assert_null(gun.try_fire(0.3, [near]), "Cadence blocks a second shot")
	assert_eq(gun.try_fire(0.3, [near]), near, "Fires again after the interval")


func test_prefers_target_closest_to_base() -> void:
	var gun := Gun.new(Vector2(5.5, 0.5))
	var behind := _drone_at(3)
	var ahead := _drone_at(4)
	assert_eq(gun.try_fire(0.0, [behind, ahead]), ahead)
	assert_eq(gun.try_fire(1.0, [ahead, behind]), ahead, "Order does not matter")


func test_ignores_dead_and_finished_targets() -> void:
	var gun := Gun.new(Vector2(0.5, 0.5))
	var dead := _drone_at(1)
	dead.take_damage(100.0)
	assert_null(gun.try_fire(0.0, [dead]))
	var leaked := _drone_at(1)
	leaked.finished = true
	assert_null(gun.try_fire(0.0, [leaked]))
