class_name SkillStub
extends RefCounted
## Meta-progression stub (Vision V1): persists exactly one dummy bonus.
## The real skill tree UI comes later; runs only need save/load + hash.

const SAVE_PATH := "user://skill_stub.cfg"

var start_money_bonus: int = 0


func state_hash() -> String:
	return str(start_money_bonus)


func save_to(path: String = SAVE_PATH) -> int:
	var cfg := ConfigFile.new()
	cfg.set_value("meta", "start_money_bonus", start_money_bonus)
	return cfg.save(path)


func load_from(path: String = SAVE_PATH) -> int:
	var cfg := ConfigFile.new()
	var err := cfg.load(path)
	if err != OK:
		return err
	start_money_bonus = int(cfg.get_value("meta", "start_money_bonus", 0))
	return OK
