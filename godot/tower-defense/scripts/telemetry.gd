class_name Telemetry
extends RefCounted
## Minimal run logger: collects JSON events, flushes once as JSONL.
## No per-frame logging — wave/build/leak/kill/send events only. Analysis lives in
## tools/analyze_run.py; this class is the writer side.
## `source` tags the run's provenance (`"local"` default; the balance harness
## passes `"harness"` — logged as `harness: true`).

var lines: Array[String] = []


func _init(game_seed: int = 0, source := "local") -> void:
	event("run_start", {"seed": game_seed, "source": source, "harness": source != "local"})


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
