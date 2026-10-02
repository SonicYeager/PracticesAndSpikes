extends GutTest
## Fx (T14): shake clamp/decay, explosion frame alternation, ember-burst
## spread, recoil kick and the spawn methods — driven without the game scene.


func _make_fx() -> Dictionary:
	var camera := Camera2D.new()
	add_child_autofree(camera)
	var fx := Fx.new()
	add_child_autofree(fx)
	fx.setup(camera)
	return {"fx": fx, "camera": camera}


func test_shake_clamps_and_decays() -> void:
	var rig := _make_fx()
	var fx: Fx = rig["fx"]
	var camera: Camera2D = rig["camera"]
	fx.shake(0.8)
	fx.shake(0.8)
	assert_almost_eq(fx._trauma, 1.0, 0.001, "Trauma clamps at 1")
	fx.update(0.1)
	assert_gt(camera.offset.length(), 0.0, "Full trauma offsets the camera")
	for i in 60:
		fx.update(0.1)
	assert_almost_eq(fx._trauma, 0.0, 0.001, "Trauma decays to zero")
	assert_almost_eq(camera.offset.length(), 0.0, 0.001, "Camera returns to rest")


func test_explosion_alternates_frames() -> void:
	var rig := _make_fx()
	var fx: Fx = rig["fx"]
	fx.explosion(Vector2.ZERO)
	fx.explosion(Vector2.ZERO)
	var sprites: Array = fx.get_children().filter(func(c): return c is Sprite2D)
	assert_eq(sprites.size(), 2, "Two explosions spawned")
	assert_ne(sprites[0].texture, sprites[1].texture, "Frames alternate")


func test_ember_burst_spawns_particles_and_spread_explosions() -> void:
	var rig := _make_fx()
	var fx: Fx = rig["fx"]
	var center := Vector2(100, 100)
	fx.ember_burst(center)
	var sprites: Array = fx.get_children().filter(func(c): return c is Sprite2D)
	var particles: Array = fx.get_children().filter(func(c): return c is CPUParticles2D)
	assert_eq(particles.size(), 1, "One ember emitter")
	assert_eq(sprites.size(), 3, "Three explosions")
	var spread := 0.0
	for s in sprites:
		spread = maxf(spread, s.position.distance_to(center))
	assert_gt(spread, 1.0, "Explosions spread in pixel space")


func test_recoil_kicks_the_barrel() -> void:
	var rig := _make_fx()
	var fx: Fx = rig["fx"]
	var root := Node2D.new()
	var barrel := Node2D.new()
	barrel.name = "Barrel"
	root.add_child(barrel)
	add_child_autofree(root)
	fx.recoil(root, Vector2.RIGHT)
	assert_eq(barrel.position, Vector2(-Fx.RECOIL_PX, 0), "Kick opposite the shot")
	fx.recoil(null, Vector2.RIGHT)  # null-safe, must not crash


func test_muzzle_and_impact_spawn_sprites() -> void:
	var rig := _make_fx()
	var fx: Fx = rig["fx"]
	fx.muzzle(Vector2(10, 10), Vector2.RIGHT)
	fx.impact(Vector2(20, 20))
	var sprites: Array = fx.get_children().filter(func(c): return c is Sprite2D)
	assert_eq(sprites.size(), 2, "Muzzle flash and impact spark spawned")
