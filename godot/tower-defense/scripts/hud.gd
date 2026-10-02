class_name GameHud
extends Control
## HUD layer (T12, XT-style): status (gold / wave / modifier chip), build
## panel with sell toggle, segmented wave bar with start button, and the
## game-over overlay. `game.gd` pushes state via `update_state`; user intents
## come back as signals (`wave_pressed`, `sell_toggled`, `restart_pressed`).

signal wave_pressed
signal sell_toggled(active: bool)
signal restart_pressed

const COLOR_CYAN := Color(0.624, 0.847, 1.0)
const MODIFIER_COLORS := {
	"rush": Color(1.0, 0.541, 0.231),
	"swarm": Color(0.557, 0.878, 0.29),
	"blackout": Color(0.624, 0.847, 1.0),
	"bounty": Color(1.0, 0.843, 0.369),
}

var _chip_modifier := ""

@onready var _money_label: Label = $Status/Row/Money
@onready var _wave_label: Label = $Status/Row/Wave
@onready var _chip: PanelContainer = $Status/Row/Chip
@onready var _chip_label: Label = $Status/Row/Chip/Label
@onready var _cost_label: Label = $Build/Box/Actions/Slot/SlotRow/Cost
@onready var _sell_button: Button = $Build/Box/Actions/Sell
@onready var _wave_text: Label = $WavePanel/Row/WaveText
@onready var _wave_bar: GameHudBar = $WavePanel/Row/Bar
@onready var _wave_button: Button = $WavePanel/Row/WaveButton
@onready var _game_over: Control = $GameOver
@onready var _game_over_stats: Label = $GameOver/Center/Stats
@onready var _restart_button: Button = $GameOver/Center/Restart


func _ready() -> void:
	_wave_button.pressed.connect(_on_wave_pressed)
	_sell_button.toggled.connect(func(active: bool) -> void: sell_toggled.emit(active))
	_restart_button.pressed.connect(func() -> void: restart_pressed.emit())
	_game_over.hide()


func _on_wave_pressed() -> void:
	if not _wave_button.disabled:
		wave_pressed.emit()


func update_state(state: Dictionary) -> void:
	var wave := int(state.get("wave", 0))
	var phase := str(state.get("phase", "idle"))
	_set_label(_money_label, str(int(state.get("money", 0))))
	_set_label(_cost_label, str(int(state.get("gun_cost", 0))))
	_update_status(wave, phase, str(state.get("modifier_id", "")), str(state.get("modifier_label", "")))
	_update_wave_row(wave, phase, state)
	if phase != "game_over":
		_wave_bar.set_progress(_resolved(state), int(state.get("total", 0)))


func show_game_over(wave: int, money: int) -> void:
	_game_over_stats.text = "WELLE %d — GELD %d" % [wave, money]
	_game_over.show()


func is_game_over_visible() -> bool:
	return _game_over.visible


func game_over_text() -> String:
	return _game_over_stats.text


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


func _update_wave_row(wave: int, phase: String, state: Dictionary) -> void:
	var alive := int(state.get("alive", 0))
	var queued := int(state.get("queued", 0))
	match phase:
		"running":
			_set_label(_wave_text, "WELLE %d · %d UNTERWEGS · %d WARTESCHLANGE" % [wave, alive, queued])
			_set_wave_button("LÄUFT", false)
		"break":
			_set_label(_wave_text, "NÄCHSTE WELLE IN %.1f S" % float(state.get("break_left", 0.0)))
			_set_wave_button("JETZT STARTEN", true)
		"idle":
			_set_label(_wave_text, "BEREIT ZUM START")
			_set_wave_button("WELLE %d STARTEN" % (wave + 1), true)
		_:
			_set_label(_wave_text, "LAUF BEENDET")
			_set_wave_button("—", false)


func _set_wave_button(text: String, enabled: bool) -> void:
	if _wave_button.text != text:
		_wave_button.text = text
	_wave_button.disabled = not enabled


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
