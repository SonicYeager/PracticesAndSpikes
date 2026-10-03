class_name Economy
extends RefCounted
## Single-currency economy: money IS health.
## Kills earn, leaks cost. Game over only when money drops below zero.

const KILL_REWARD := 6
const LEAK_COST := 10
const GUN_COST := 25
const WALL_COST := 10

var money: int


func _init(start_money: int = 100) -> void:
	money = start_money


func on_kill(amount: int = KILL_REWARD) -> void:
	money += amount


func on_leak() -> void:
	money -= LEAK_COST


func earn(amount: int) -> void:
	money += amount


func can_afford(cost: int) -> bool:
	return money >= cost


func spend(cost: int) -> bool:
	if not can_afford(cost):
		return false
	money -= cost
	return true


func is_game_over() -> bool:
	return money < 0
