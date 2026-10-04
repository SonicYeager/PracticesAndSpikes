class_name WaveDirector
extends RefCounted
## Wave flow state machine (T12.5): phase, composition, spawn queue, break
## countdown and the per-wave modifier knobs. Pure logic — the scene asks for
## the next spawn (`tick_spawn`) and owns everything visual (sfx, telemetry,
## sprite creation). `begin_break()` is called by the scene once the queue and
## the field are both empty.

enum Phase { IDLE, RUNNING, BREAK, GAME_OVER }

const SPAWN_INTERVAL := 0.7
const BREAK_SECONDS := 5.0
const RUSH_INTERVAL_MUL := 0.6
const SWARM_COUNT_MUL := 1.5
const SWARM_HP_MUL := 0.7
const BLACKOUT_RANGE := -1.0
const BOUNTY_KILL_BONUS := 2

var wave := 0
var phase: Phase = Phase.IDLE
var composition: Dictionary = {}
var queue: Array[String] = []
var total := 0
var break_timer := 0.0
var next_modifier := ""
var spawn_interval := SPAWN_INTERVAL
var range_bonus := 0.0
var kill_reward := Economy.KILL_REWARD

var _seed := 0
var _spawn_timer := 0.0


func _init(run_seed: int) -> void:
	_seed = run_seed


func start(n: int) -> void:
	## Starts wave `n`: composes it from the run seed, applies the modifier
	## and (re)fills the queue. The scene calls this on manual starts and on
	## break timeouts.
	wave = n
	composition = WaveGen.composition(n, _seed)
	_apply_modifier(str(composition.get("modifier", "")))
	queue.clear()
	var normals: int = (
		int(composition["count"])
		- int(composition["fast"])
		- int(composition["tanks"])
		- int(composition.get("splitters", 0))
	)
	for i in normals:
		queue.append("normal")
	for i in int(composition["fast"]):
		queue.append("fast")
	for i in int(composition["tanks"]):
		queue.append("tank")
	for i in int(composition.get("splitters", 0)):
		queue.append("splitter")
	total = queue.size()
	phase = Phase.RUNNING
	break_timer = 0.0
	_spawn_timer = 0.0


func begin_break() -> void:
	## Queue empty + field clear: intermission until the next wave. Caches the
	## next wave's modifier for the HUD break preview.
	phase = Phase.BREAK
	break_timer = BREAK_SECONDS
	next_modifier = str(WaveGen.composition(wave + 1, _seed)["modifier"])


func end_run() -> void:
	phase = Phase.GAME_OVER


func tick_break(dt: float) -> bool:
	## Counts the break down; returns true once it elapsed (the scene starts
	## the next wave, so the start stays a scene-visible event).
	if phase != Phase.BREAK:
		return false
	break_timer = maxf(break_timer - dt, 0.0)
	return break_timer == 0.0


func tick_spawn(dt: float) -> String:
	## Returns the next kind once a spawn is due, otherwise "".
	if phase != Phase.RUNNING or queue.is_empty():
		return ""
	_spawn_timer -= dt
	if _spawn_timer > 0.0:
		return ""
	_spawn_timer = spawn_interval
	return queue.pop_front()


func _apply_modifier(modifier: String) -> void:
	## Resets the per-wave knobs, then applies the event (T10). Swarm mutates
	## the composition itself (more, weaker drones); the rest are game knobs.
	spawn_interval = SPAWN_INTERVAL
	range_bonus = 0.0
	kill_reward = Economy.KILL_REWARD
	match modifier:
		"rush":
			spawn_interval *= RUSH_INTERVAL_MUL
		"swarm":
			composition["count"] = roundi(float(composition["count"]) * SWARM_COUNT_MUL)
			composition["hp"] = float(composition["hp"]) * SWARM_HP_MUL
		"blackout":
			range_bonus = BLACKOUT_RANGE
		"bounty":
			kill_reward += BOUNTY_KILL_BONUS
