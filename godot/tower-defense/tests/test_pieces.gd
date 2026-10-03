extends GutTest
## Piece table (T17): kind → cost/label/telemetry name; unknown kinds fall
## back to the gun (old-log/robustness default).


func test_costs_come_from_economy() -> void:
	assert_eq(Pieces.cost(Pieces.Kind.GUN), Economy.GUN_COST)
	assert_eq(Pieces.cost(Pieces.Kind.WALL), Economy.WALL_COST)
	assert_eq(Pieces.cost(Pieces.Kind.WALL), 10, "First balance pass")


func test_wall_refund_is_half() -> void:
	assert_eq(Pieces.cost(Pieces.Kind.WALL) / 2, 5, "Selling a wall refunds half")


func test_labels_and_telemetry_names() -> void:
	assert_eq(Pieces.label(Pieces.Kind.GUN), "KANONE")
	assert_eq(Pieces.label(Pieces.Kind.WALL), "MAUER")
	assert_eq(Pieces.telemetry_name(Pieces.Kind.GUN), "gun")
	assert_eq(Pieces.telemetry_name(Pieces.Kind.WALL), "wall")


func test_unknown_kinds_fall_back_to_gun() -> void:
	assert_eq(Pieces.cost(99), Economy.GUN_COST)
	assert_eq(Pieces.label(99), "KANONE")
	assert_eq(Pieces.telemetry_name(99), "gun")
