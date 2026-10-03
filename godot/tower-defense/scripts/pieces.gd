class_name Pieces
extends RefCounted
## Buildable piece kinds (wall slice, T17): kind → cost/label/telemetry name.
## Static data only — no instances; costs live in Economy, the gun refund
## stays level-based in GunUpgrades.

enum Kind { GUN, WALL }

const LABELS := { Kind.GUN: "KANONE", Kind.WALL: "MAUER" }
const TELEMETRY_NAMES := { Kind.GUN: "gun", Kind.WALL: "wall" }


static func cost(kind: int) -> int:
	return Economy.WALL_COST if kind == Kind.WALL else Economy.GUN_COST


static func label(kind: int) -> String:
	return str(LABELS.get(kind, LABELS[Kind.GUN]))


static func telemetry_name(kind: int) -> String:
	return str(TELEMETRY_NAMES.get(kind, TELEMETRY_NAMES[Kind.GUN]))
