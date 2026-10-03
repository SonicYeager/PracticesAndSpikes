class_name GameHud
extends Control
## HUD layer (T12, XT-style): status (gold / wave / modifier chip), build
## panel with sell toggle, segmented wave bar with start button, and the
## game-over overlay. `game.gd` pushes state via `update_state`; user intents
## come back as signals (`wave_pressed`, `sell_toggled`, `restart_pressed`).

signal wave_pressed
signal sell_toggled(active: bool)
signal restart_pressed
signal upgrade_pressed
signal continue_pressed

const COLOR_CYAN := Color(0.624, 0.847, 1.0)
const COLOR_GOLD := Color(1.0, 0.843137, 0.368627)
const LEAK_FLASH_ALPHA := 0.45
const LEAK_FLASH_FADE := 0.45
const GUN_ICON := preload("res://art/gun_base.png")
const WALL_ICON := preload("res://art/wall.png")
const MODIFIER_COLORS := {
	"rush": Color(1.0, 0.541, 0.231),
	"swarm": Color(0.557, 0.878, 0.29),
	"blackout": Color(0.624, 0.847, 1.0),
	"bounty": Color(1.0, 0.843, 0.369),
}

var _chip_modifier := ""
var _flash_tween: Tween

@onready var _leak_flash: TextureRect = $LeakFlash
@onready var _money_label: Label = $Status/Row/Money
@onready var _wave_label: Label = $Status/Row/Wave
@onready var _chip: PanelContainer = $Status/Row/Chip
@onready var _chip_label: Label = $Status/Row/Chip/Label
@onready var _time_label: Label = $Status/Row/Time
@onready var _cost_label: Label = $Build/Box/Actions/Slot/SlotRow/Cost
@onready var _slot_icon: TextureRect = $Build/Box/Actions/Slot/SlotRow/Gun
@onready var _slot_caption: Label = $Build/Box/Actions/Slot/SlotRow/Caption
@onready var _sell_button: Button = $Build/Box/Actions/Sell
@onready var _upgrade_row: HBoxContainer = $Build/Box/UpgradeRow
@onready var _upgrade_info: Label = $Build/Box/UpgradeRow/UpgradeInfo
@onready var _upgrade_button: Button = $Build/Box/UpgradeRow/Upgrade
@onready var _wave_text: Label = $WavePanel/Row/WaveText
@onready var _wave_bar: GameHudBar = $WavePanel/Row/Bar
@onready var _wave_button: Button = $WavePanel/Row/WaveButton
@onready var _game_over: Control = $GameOver
@onready var _run_end_title: Label = $GameOver/Center/Title
@onready var _run_end_stats: Label = $GameOver/Center/Stats
@onready var _run_end_note: Label = $GameOver/Center/Note
@onready var _continue_button: Button = $GameOver/Center/Continue
@onready var _restart_button: Button = $GameOver/Center/Restart


func _ready() -> void:
	_wave_button.pressed.connect(_on_wave_pressed)
	_sell_button.toggled.connect(func(active: bool) -> void: sell_toggled.emit(active))
	_restart_button.pressed.connect(func() -> void: restart_pressed.emit())
	_continue_button.pressed.connect(func() -> void: continue_pressed.emit())
	_upgrade_button.pressed.connect(_on_upgrade_pressed)
	_game_over.hide()


func _on_upgrade_pressed() -> void:
	if not _upgrade_button.disabled:
		upgrade_pressed.emit()


func _on_wave_pressed() -> void:
	if not _wave_button.disabled:
		wave_pressed.emit()


func update_state(state: Dictionary) -> void:
	var wave := int(state.get("wave", 0))
	var phase := str(state.get("phase", "idle"))
	_set_label(_money_label, str(int(state.get("money", 0))))
	_set_label(_cost_label, str(int(state.get("build_cost", 0))))
	_set_label(_slot_caption, str(state.get("build_label", "KANONE")))
	_slot_icon.texture = (
		WALL_ICON if str(state.get("build_kind", "gun")) == "wall" else GUN_ICON
	)
	_update_status(wave, phase, str(state.get("modifier_id", "")), str(state.get("modifier_label", "")))
	_update_time_row(state)
	_update_wave_row(wave, phase, state)
	_update_upgrade_row(phase, state)
	if phase != "game_over":
		_wave_bar.set_progress(_resolved(state), int(state.get("total", 0)))


func show_run_end(
	result: String,
	wave: int,
	money: int,
	kills: int,
	leaks: int,
	mission_wave := 0
) -> void:
	## End screen for both outcomes. `mission_wave > 0` marks an endless
	## segment that ended after the mission was already cleared.
	var endless_end := mission_wave > 0
	_run_end_title.text = "SIEG" if result == "win" and not endless_end else "GAME OVER"
	_run_end_stats.text = "WELLE %d — GELD %d — %d KILLS · %d LEAKS" % [wave, money, kills, leaks]
	_run_end_note.text = "MISSION GEWONNEN (WELLE %d)" % mission_wave
	_run_end_note.visible = endless_end
	_continue_button.visible = result == "win" and not endless_end
	_game_over.show()


func hide_run_end() -> void:
	_game_over.hide()


func is_run_end_visible() -> bool:
	return _game_over.visible


func flash_leak() -> void:
	## Red edge pulse on a leak; re-triggers restart the fade (no stacking).
	_leak_flash.modulate.a = LEAK_FLASH_ALPHA
	if _flash_tween != null and _flash_tween.is_valid():
		_flash_tween.kill()
	_flash_tween = create_tween()
	_flash_tween.tween_property(_leak_flash, "modulate:a", 0.0, LEAK_FLASH_FADE)


func run_end_text() -> String:
	return _run_end_stats.text


func _update_status(wave: int, phase: String, modifier_id: String, modifier_label: String) -> void:
	_set_label(_wave_label, "WELLE %d" % (wave + 1 if phase == "idle" else wave))
	var show_chip := modifier_label != "" and phase != "game_over"
	_chip.visible = show_chip
	if not show_chip:
		_chip_modifier = ""
		return
	if _chip_modifier == modifier_id:
		return
	_chip_modifier = modifier_id
	var color: Color = MODIFIER_COLORS.get(modifier_id, COLOR_CYAN)
	_chip_label.text = modifier_label.to_upper()
	_chip_label.add_theme_color_override("font_color", color)
	var style: StyleBoxFlat = _chip.get_theme_stylebox("panel").duplicate()
	style.border_color = Color(color.r, color.g, color.b, 0.6)
	_chip.add_theme_stylebox_override("panel", style)


func _update_time_row(state: Dictionary) -> void:
	## Pause/speed indicator; pause wins over the speed label.
	var paused := bool(state.get("paused", false))
	var speed := float(state.get("speed", 1.0))
	if paused:
		_set_label(_time_label, "PAUSE")
		_time_label.add_theme_color_override("font_color", COLOR_GOLD)
		_time_label.visible = true
	elif speed > 1.0:
		_set_label(_time_label, "×%d" % int(speed))
		_time_label.add_theme_color_override("font_color", COLOR_CYAN)
		_time_label.visible = true
	else:
		_time_label.visible = false


func _update_wave_row(wave: int, phase: String, state: Dictionary) -> void:
	var alive := int(state.get("alive", 0))
	var queued := int(state.get("queued", 0))
	var paused := bool(state.get("paused", false))
	match phase:
		"running":
			_set_label(_wave_text, "WELLE %d · %d UNTERWEGS · %d WARTESCHLANGE" % [wave, alive, queued])
			_set_wave_button("LÄUFT", false)
		"break":
			_set_label(_wave_text, "NÄCHSTE WELLE IN %.1f S" % float(state.get("break_left", 0.0)))
			_set_wave_button("JETZT STARTEN", not paused)
		"idle":
			_set_label(_wave_text, "BEREIT ZUM START")
			_set_wave_button("WELLE %d STARTEN" % (wave + 1), not paused)
		_:
			_set_label(_wave_text, "LAUF BEENDET")
			_set_wave_button("—", false)


func _set_wave_button(text: String, enabled: bool) -> void:
	if _wave_button.text != text:
		_wave_button.text = text
	_wave_button.disabled = not enabled


func _update_upgrade_row(phase: String, state: Dictionary) -> void:
	var selected: Dictionary = state.get("selected", {})
	var has_selection := not selected.is_empty() and phase != "game_over"
	_upgrade_row.visible = has_selection
	if not has_selection:
		return
	var level := int(selected.get("level", 1))
	var max_level := int(selected.get("max_level", 1))
	if level >= max_level:
		_set_label(_upgrade_info, "%s · MAX" % str(selected.get("name", "")))
		_upgrade_button.disabled = true
		return
	_set_label(
		_upgrade_info,
		"%s · +%d" % [str(selected.get("name", "")), int(selected.get("next_cost", 0))]
	)
	_upgrade_button.disabled = not bool(selected.get("affordable", false))


func _resolved(state: Dictionary) -> int:
	var total := int(state.get("total", 0))
	match str(state.get("phase", "idle")):
		"running":
			return maxi(total - int(state.get("queued", 0)) - int(state.get("alive", 0)), 0)
		"break":
			return total
		_:
			return 0


func _set_label(label: Label, text: String) -> void:
	if label.text != text:
		label.text = text
