extends GutTest
## HUD state API + user intents (T12). The HUD is a separate scene; these
## tests drive it directly without the game.


func _make_hud():
	var hud = load("res://scenes/Hud.tscn").instantiate()
	add_child_autofree(hud)
	return hud


func _running_state() -> Dictionary:
	return {
		"money": 160,
		"build_cost": 25,
		"build_label": "KANONE",
		"build_kind": "gun",
		"wave": 3,
		"phase": "running",
		"alive": 4,
		"queued": 6,
		"total": 10,
		"break_left": 0.0,
		"modifier_id": "swarm",
		"modifier_label": "Schwarm",
	}


func test_state_updates_status_and_wave_row() -> void:
	var hud = _make_hud()
	hud.update_state(_running_state())
	assert_eq(hud._money_label.text, "160")
	assert_eq(hud._cost_label.text, "25")
	assert_true(hud._wave_label.text.contains("WELLE 3"), "Status shows the wave")
	assert_true(hud._wave_text.text.contains("4 UNTERWEGS"), "Live count is shown")
	assert_true(hud._wave_text.text.contains("6 WARTESCHLANGE"), "Queue count is shown")
	assert_true(hud._chip.visible, "Modifier chip is visible")
	assert_true(hud._chip_label.text.contains("SCHWARM"), "Chip shows the modifier")
	assert_true(hud._wave_button.disabled, "Wave button is disabled while running")


func test_wave_button_emits_intent_when_startable() -> void:
	var hud = _make_hud()
	hud.update_state(_running_state())
	watch_signals(hud)
	hud._wave_button.pressed.emit()
	assert_signal_not_emitted(hud, "wave_pressed", "Disabled phase emits nothing")

	var idle := _running_state()
	idle["phase"] = "idle"
	idle["wave"] = 0
	idle["modifier_label"] = ""
	hud.update_state(idle)
	assert_false(hud._wave_button.disabled, "Idle enables the start button")
	assert_false(hud._chip.visible, "No chip without a modifier")
	hud._wave_button.pressed.emit()
	assert_signal_emitted(hud, "wave_pressed")

func test_sell_toggle_emits_active_state() -> void:
	var hud = _make_hud()
	watch_signals(hud)
	hud._sell_button.button_pressed = true
	assert_signal_emitted_with_parameters(hud, "sell_toggled", [true])
	hud._sell_button.button_pressed = false
	assert_signal_emitted_with_parameters(hud, "sell_toggled", [false])


func test_progress_bar_freezes_on_game_over() -> void:
	var hud = _make_hud()
	var running := _running_state()
	running["alive"] = 2
	running["queued"] = 3
	hud.update_state(running)
	assert_eq(hud._wave_bar._done, 5, "Half the wave is resolved")
	var over := running.duplicate()
	over["phase"] = "game_over"
	hud.update_state(over)
	assert_eq(hud._wave_bar._done, 5, "Bar keeps its last state at game over")


func test_run_end_overlay() -> void:
	var hud = _make_hud()
	assert_false(hud.is_run_end_visible(), "Overlay starts hidden")
	hud.show_run_end("loss", 2, -1, 3, 1)
	assert_true(hud.is_run_end_visible(), "Overlay is shown")
	assert_eq(hud._run_end_title.text, "GAME OVER")
	assert_true(hud.run_end_text().contains("GELD"), "Summary shows the run stats")
	assert_false(hud._continue_button.visible, "No continue on a loss")
	assert_false(hud._run_end_note.visible, "No mission note on a plain loss")
	watch_signals(hud)
	hud._restart_button.pressed.emit()
	assert_signal_emitted(hud, "restart_pressed")


func test_run_end_variants() -> void:
	var hud = _make_hud()
	hud.show_run_end("win", 20, 350, 214, 3)
	assert_eq(hud._run_end_title.text, "SIEG")
	assert_true(hud._continue_button.visible, "Win offers the endless continue")
	assert_false(hud._run_end_note.visible)
	assert_true(hud.run_end_text().contains("KILLS"), "Summary carries the kills")
	assert_true(hud.run_end_text().contains("LEAKS"), "Summary carries the leaks")
	watch_signals(hud)
	hud._continue_button.pressed.emit()
	assert_signal_emitted(hud, "continue_pressed")
	hud.show_run_end("win", 34, -2, 400, 9, 20)
	assert_eq(hud._run_end_title.text, "GAME OVER", "Endless loss reads as game over")
	assert_true(hud._run_end_note.visible, "Mission note is shown")
	assert_true(hud._run_end_note.text.contains("WELLE 20"))
	assert_false(hud._continue_button.visible, "No inert continue after endless")


func test_upgrade_row_follows_selection_and_affordability() -> void:
	var hud = _make_hud()
	var state := _running_state()
	assert_false(hud._upgrade_row.visible, "No selection, no upgrade row")
	state["selected"] = {
		"level": 2, "name": "KANONE 2", "max_level": 5, "next_cost": 35, "affordable": true,
	}
	hud.update_state(state)
	assert_true(hud._upgrade_row.visible, "Selection shows the row")
	assert_true(hud._upgrade_info.text.contains("KANONE 2"), "Row shows the name")
	assert_true(hud._upgrade_info.text.contains("+35"), "Row shows the delta")
	assert_false(hud._upgrade_button.disabled, "Affordable upgrade is enabled")


func test_upgrade_button_disabled_states_and_signal() -> void:
	var hud = _make_hud()
	var state := _running_state()
	watch_signals(hud)
	state["selected"] = {
		"level": 2, "name": "KANONE 2", "max_level": 5, "next_cost": 35, "affordable": false,
	}
	hud.update_state(state)
	assert_true(hud._upgrade_button.disabled, "Unaffordable upgrade is disabled")
	hud._upgrade_button.pressed.emit()
	assert_signal_not_emitted(hud, "upgrade_pressed", "Disabled button emits nothing")
	state["selected"] = {
		"level": 5, "name": "LANZE", "max_level": 5, "next_cost": 0, "affordable": true,
	}
	hud.update_state(state)
	assert_true(hud._upgrade_button.disabled, "Max level is disabled")
	assert_true(hud._upgrade_info.text.contains("MAX"), "Max level shows MAX")
	state["selected"] = {
		"level": 1, "name": "KANONE", "max_level": 5, "next_cost": 20, "affordable": true,
	}
	hud.update_state(state)
	hud._upgrade_button.pressed.emit()
	assert_signal_emitted(hud, "upgrade_pressed", "Enabled button emits the intent")
	state["phase"] = "game_over"
	hud.update_state(state)
	assert_false(hud._upgrade_row.visible, "Game over hides the row")


func test_build_slot_follows_the_build_kind() -> void:
	var hud = _make_hud()
	var state := _running_state()
	hud.update_state(state)
	assert_eq(hud._slot_caption.text, "KANONE")
	assert_eq(hud._cost_label.text, "25")
	assert_eq(hud._slot_icon.texture, GameHud.GUN_ICON, "Gun icon by default")
	state["build_kind"] = "wall"
	state["build_label"] = "MAUER"
	state["build_cost"] = 10
	hud.update_state(state)
	assert_eq(hud._slot_caption.text, "MAUER")
	assert_eq(hud._cost_label.text, "10")
	assert_eq(hud._slot_icon.texture, GameHud.WALL_ICON, "Wall icon in wall mode")


func test_update_state_defaults_without_build_keys() -> void:
	var hud = _make_hud()
	hud.update_state({"money": 50, "wave": 0, "phase": "idle"})
	assert_eq(hud._cost_label.text, "0")
	assert_eq(hud._slot_caption.text, "KANONE")
	assert_eq(hud._slot_icon.texture, GameHud.GUN_ICON, "Defaults to the gun icon")
