class_name GunUpgrades
extends RefCounted
## Five-level upgrade path for the gun (ADR 0012): cumulative prices (an upgrade
## pays the delta to the next level), stats and display names per level. Level 1
## is the base gun, level 5 the signature level. Pure data + helpers, no
## RNG, no scene access — the map to these numbers is docs/BALANCE.md.

const MAX_LEVEL := 5
const LEVELS: Array = [
	{"price": 25, "damage": 8.0, "range": 3.5, "interval": 0.6, "name": "KANONE", "description": "Basis-Plasma-Kanone."},
	{"price": 45, "damage": 11.0, "range": 3.5, "interval": 0.6, "name": "KANONE 2", "description": "Mehr Schaden."},
	{"price": 80, "damage": 15.0, "range": 3.5, "interval": 0.55, "name": "KANONE 3", "description": "Mehr Schaden, schneller."},
	{"price": 140, "damage": 21.0, "range": 3.8, "interval": 0.5, "name": "KANONE 4", "description": "Mehr Schaden und Reichweite."},
	{"price": 250, "damage": 30.0, "range": 4.5, "interval": 0.5, "name": "LANZE", "description": "Signatur: Reichweite und Schaden."},
]


static func stats(level: int) -> Dictionary:
	return LEVELS[clampi(level, 1, MAX_LEVEL) - 1]


static func price(level: int) -> int:
	return int(stats(level)["price"])


static func upgrade_cost(level: int) -> int:
	## Cost to go from `level` to `level + 1`; 0 at max level.
	if level >= MAX_LEVEL:
		return 0
	return price(level + 1) - price(level)


static func refund(level: int) -> int:
	## Selling returns half of the current cumulative investment.
	return price(level) / 2


static func display_name(level: int) -> String:
	return str(stats(level)["name"])


static func description(level: int) -> String:
	return str(stats(level)["description"])
