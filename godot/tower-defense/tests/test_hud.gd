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
		"gun_cost": 25,
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


func test_game_over_overlay() -> void:
	var hud = _make_hud()
	assert_false(hud.is_game_over_visible(), "Overlay starts hidden")
	hud.show_game_over(2, -1)
	assert_true(hud.is_game_over_visible(), "Overlay is shown")
	assert_true(hud.game_over_text().contains("GELD"), "Summary shows the run stats")
	watch_signals(hud)
	hud._restart_button.pressed.emit()
	assert_signal_emitted(hud, "restart_pressed")


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
