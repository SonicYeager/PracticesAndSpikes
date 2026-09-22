class_name Telemetry
extends RefCounted
## Minimal run logger: collects JSON events, flushes once as JSONL.
## No per-frame logging — wave/build/leak events only. Analysis lives
## in a Python script (T05); this class is the writer side.

var lines: Array[String] = []


func _init(game_seed: int = 0) -> void:
	event("run_start", {"seed": game_seed})


func event(type: String, data: Dictionary = {}) -> void:
	var entry := {"t": type}
	entry.merge(data)
	lines.append(JSON.stringify(entry))


func flush(path: String) -> int:
	var f := FileAccess.open(path, FileAccess.WRITE)
	if f == null:
		return FileAccess.get_open_error()
	for line in lines:
		f.store_line(line)
	f.close()
	return OK
