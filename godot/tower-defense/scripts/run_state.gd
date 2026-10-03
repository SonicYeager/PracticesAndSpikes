class_name RunState
extends RefCounted
## Finite-run frame (M1): wave goal, mission result, endless flag and the
## run-summary counters. Pure transitions — the scene owns presentation.

enum Result { NONE, WIN, LOSS }

const DEFAULT_GOAL := 20

var goal: int
var result: Result = Result.NONE
var endless := false
var kills := 0
var leaks := 0
var cleared_wave := 0


func _init(p_goal: int = DEFAULT_GOAL) -> void:
	goal = maxi(p_goal, 1)


func register_wave_cleared(wave: int) -> bool:
	## True exactly once, when the mission goal is reached (never in endless).
	if result != Result.NONE or endless or wave < goal:
		return false
	result = Result.WIN
	cleared_wave = wave
	return true


func register_loss() -> void:
	if result == Result.NONE:
		result = Result.LOSS


func register_kill() -> void:
	kills += 1


func register_leak() -> void:
	leaks += 1


func is_mission_won() -> bool:
	return result == Result.WIN
