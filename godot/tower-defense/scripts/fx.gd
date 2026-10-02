class_name Fx
extends Node2D
## Combat/feel effects (T14): screen shake (camera offset), muzzle flashes,
## impact sparks, explosions, ember bursts and barrel recoil. `game.gd` calls
## the spawn methods with world positions and drives `update(delta)` from its
## frame order (keeps the manual test stepping working).

const ART_SCALE := BoardView.ART_SCALE
const SHAKE_DECAY := 1.6         # trauma lost per second
const SHAKE_MAX_OFFSET := 9.0    # px at full trauma
const RECOIL_PX := 3.0

const MUZZLE_TEX := preload("res://art/muzzle.png")
const IMPACT_TEX := preload("res://art/impact.png")
const EXPLOSION_TEX: Array = [
	preload("res://art/explosion_0.png"),
	preload("res://art/explosion_1.png"),
]
const EMBER_TEX := preload("res://art/ember.png")

var _camera: Camera2D
var _trauma := 0.0
var _anim_time := 0.0
var _fx_counter := 0
var _jitter_phase := 0


func setup(camera: Camera2D) -> void:
	_camera = camera


func update(delta: float) -> void:
	_anim_time += delta
	_trauma = maxf(_trauma - SHAKE_DECAY * delta, 0.0)
	if _camera == null:
		return
	# Deterministic pseudo-noise: no RNG in the scene layer.
	var amount := _trauma * _trauma
	_camera.offset = Vector2(
		sin(_anim_time * 37.0) + 0.5 * sin(_anim_time * 61.0),
		cos(_anim_time * 43.0) + 0.5 * cos(_anim_time * 71.0)
	) * (SHAKE_MAX_OFFSET * amount)


func shake(amount: float) -> void:
	_trauma = minf(_trauma + amount, 1.0)


func muzzle(world_pos: Vector2, direction: Vector2) -> void:
	var s := Sprite2D.new()
	s.texture = MUZZLE_TEX
	s.scale = ART_SCALE
	s.position = world_pos
	s.rotation = direction.angle() + _jitter(0.3)
	add_child(s)
	var t := create_tween()
	t.tween_property(s, "scale", ART_SCALE * 1.35, 0.05)
	t.parallel().tween_property(s, "modulate:a", 0.0, 0.05)
	t.tween_callback(s.queue_free)


func impact(world_pos: Vector2) -> void:
	var s := Sprite2D.new()
	s.texture = IMPACT_TEX
	s.scale = ART_SCALE * 0.7
	s.position = world_pos
	add_child(s)
	var t := create_tween()
	t.tween_property(s, "scale", ART_SCALE * 1.5, 0.12)
	t.parallel().tween_property(s, "modulate:a", 0.0, 0.12)
	t.tween_callback(s.queue_free)


func explosion(world_pos: Vector2) -> void:
	var s := Sprite2D.new()
	s.texture = EXPLOSION_TEX[_fx_counter % EXPLOSION_TEX.size()]
	_fx_counter += 1
	s.position = world_pos
	s.scale = ART_SCALE
	add_child(s)
	var t := create_tween()
	t.tween_property(s, "scale", ART_SCALE * 1.8, 0.18)
	t.parallel().tween_property(s, "modulate:a", 0.0, 0.18)
	t.tween_callback(s.queue_free)


func ember_burst(world_pos: Vector2) -> void:
	var p := CPUParticles2D.new()
	p.position = world_pos
	p.texture = EMBER_TEX
	p.amount = 28
	p.lifetime = 1.1
	p.one_shot = true
	p.explosiveness = 1.0
	p.direction = Vector2(0, -1)
	p.spread = 180.0
	p.initial_velocity_min = 30.0
	p.initial_velocity_max = 90.0
	p.gravity = Vector2(0, 60)
	p.scale_amount_min = 0.3
	p.scale_amount_max = 0.7
	p.color = Color(1.0, 0.75, 0.35, 0.95)
	add_child(p)
	p.emitting = true
	var t := create_tween()
	t.tween_interval(1.5)
	t.tween_callback(p.queue_free)
	for i in 3:
		explosion(world_pos + Vector2(_jitter(1.0), _jitter(1.0)))


func recoil(tower_root: Node2D, direction: Vector2) -> void:
	if tower_root == null:
		return
	var barrel := tower_root.get_node_or_null("Barrel") as Node2D
	if barrel == null:
		return
	# Gun cadence (0.6 s) is far above the tween time (0.08 s): shots never
	# overlap, so resetting the position per shot is safe.
	barrel.position = -direction * RECOIL_PX
	var t := create_tween()
	t.tween_property(barrel, "position", Vector2.ZERO, 0.08)


func _jitter(radians: float) -> float:
	# Deterministic per-effect variation; separate counter so the explosion
	# frame alternation stays strict.
	_jitter_phase += 1
	return (float(_jitter_phase % 3) - 1.0) * radians
