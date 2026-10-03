extends GutTest
## Time control (T19): pause/speed state, scale mapping, HUD labels.


func test_defaults() -> void:
	var tc := TimeControl.new()
	assert_false(tc.is_paused())
	assert_almost_eq(tc.scale(), 1.0, 0.001)
	assert_eq(tc.speed_text(), "", "×1 shows no label")


func test_toggle_pause() -> void:
	var tc := TimeControl.new()
	tc.toggle_pause()
	assert_true(tc.is_paused())
	assert_almost_eq(tc.scale(), 0.0, 0.001)
	tc.toggle_pause()
	assert_false(tc.is_paused())
	assert_almost_eq(tc.scale(), 1.0, 0.001)


func test_cycle_speed_wraps() -> void:
	var tc := TimeControl.new()
	tc.cycle_speed()
	assert_almost_eq(tc.speed(), 2.0, 0.001)
	assert_eq(tc.speed_text(), "×2")
	tc.cycle_speed()
	assert_almost_eq(tc.speed(), 3.0, 0.001)
	assert_eq(tc.speed_text(), "×3")
	tc.cycle_speed()
	assert_almost_eq(tc.speed(), 1.0, 0.001, "Wraps back to ×1")


func test_pause_wins_over_speed() -> void:
	var tc := TimeControl.new()
	tc.cycle_speed()
	tc.toggle_pause()
	assert_almost_eq(tc.scale(), 0.0, 0.001, "Pause beats the selected speed")
	tc.cycle_speed()
	assert_almost_eq(tc.scale(), 0.0, 0.001, "Speed changes do not lift the pause")
	tc.toggle_pause()
	assert_almost_eq(tc.scale(), 3.0, 0.001, "Resume restores the selected speed")
