#!/usr/bin/env python3
"""Analyze Tower Defense telemetry runs (stdlib only).

Reads the JSONL files written by scripts/telemetry.gd (`user://run_<seed>.jsonl`)
and prints a per-run report plus an aggregate across runs.

Events: run_start {seed,source,harness} · wave {wave,count,hp,splitters}
        · build {cell,kind} · sell {cell,kind} · clear {cell,money}
        · upgrade {cell,from,to,cost,money} · kill {wave,cell,kind} · wave_end
        {wave,kills,leaks,money_start,money_end} · leak {wave,money,kind} · send
        {wave} · time_control {action,speed,paused} · overcharge {cell,money}
        · mission_cleared {wave,money,kills,leaks}
        · run_end {wave,money,result,endless}
        · harness_start {waves,money,dt,towers,walls}
        · harness_end {wave,reason,money,kills,leaks,steps}

Note: leak% uses the wave count (children excluded) - it can exceed 100% on
splitter waves. Legacy events without a kind parse unchanged.

Derived numbers:
  kills      = exact per wave: wave_end.kills once the wave was cleared, else the
               kill-event count (still true for a final wave cut off by game
               over). Legacy logs without either fall back to count - leaks on
               cleared waves; the legacy final wave stays "-".
  build/sell = activity while that wave was the current one (its run plus the
               following break); pre-wave-1 building is shown separately.
               `builds` counts all pieces (guns and walls); `walls` is the
               subset, rendered as `n(wm)` in the wave table. `upgrade`,
               `clear` and `overcharge` counts feed the per-wave `spend`
               column (any spending action that wave).
  money      = wave_end money_start -> money_end when present; legacy logs show
               the balance at the wave's last leak instead (leak events carry it).

Usage:
  python3 analyze_run.py [file.jsonl | dir ...]

Without arguments it prefers telemetry_local/*.jsonl (kept copies, gitignored)
and falls back to the Godot user directory's run_*.jsonl.
"""
import glob
import json
import os
import re
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
PROJECT = os.path.normpath(os.path.join(HERE, ".."))
LOCAL_DIR = os.path.join(PROJECT, "telemetry_local")
PROJECT_GODOT = os.path.join(PROJECT, "project.godot")


def project_name():
    """Read config/name from project.godot (used for the user:// directory)."""
    try:
        with open(PROJECT_GODOT, encoding="utf-8") as f:
            match = re.search(r'config/name="([^"]+)"', f.read())
        return match.group(1) if match else None
    except OSError:
        return None


def user_dir():
    name = project_name()
    if not name:
        return None
    if sys.platform == "win32":
        base = os.path.join(os.environ.get("APPDATA", ""), "Godot", "app_userdata")
    elif sys.platform == "darwin":
        base = os.path.expanduser("~/Library/Application Support/Godot/app_userdata")
    else:
        base = os.path.expanduser("~/.local/share/godot/app_userdata")
    return os.path.join(base, name)


def discover():
    files = sorted(glob.glob(os.path.join(LOCAL_DIR, "*.jsonl")))
    if files:
        return files
    directory = user_dir()
    if directory:
        return sorted(glob.glob(os.path.join(directory, "run_*.jsonl")))
    return []


def as_int(value):
    """Best-effort int for telemetry fields; None when not numeric."""
    try:
        return int(value)
    except (TypeError, ValueError):
        return None


def valid_cell(value):
    """True for a [x, y] pair of numbers (the kill-event cell shape)."""
    return (
        isinstance(value, (list, tuple))
        and len(value) == 2
        and as_int(value[0]) is not None
        and as_int(value[1]) is not None
    )


def wave_kills(run, n):
    """Exact kills when known; legacy count - leaks as the fallback."""
    summary = run["wave_ends"].get(n)
    if summary and summary["kills"] is not None:
        return summary["kills"]
    if n in run["kills_by_wave"]:
        return run["kills_by_wave"][n]
    cleared = run["order"] and n != run["order"][-1]
    if cleared and n in run["waves"]:
        return max(run["waves"][n]["count"] - len(run["leaks"].get(n, [])), 0)
    return None


def load_run(path):
    run = {
        "path": path,
        "seed": None,
        "source": None,       # run provenance ("local" / "harness", newer logs)
        "harness": None,      # bool flag from run_start
        "sends": 0,           # player-initiated wave starts (send events)
        "time_controls": 0,   # pause/speed events (emitted by the TimeControl slice)
        "overcharges": 0,     # vent overcharge casts (money-sink usage)
        "wave_ends": {},      # wave -> {kills, leaks, money_start, money_end}
        "kills_by_wave": {},  # wave -> kill-event count
        "kill_cells": {},     # (x, y) -> kill count
        "kill_kinds": {},     # kind -> kill-event count (T22 kinds in reports)
        "waves": {},          # wave -> {"count": int, "hp": number|None}
        "order": [],          # wave numbers in event order
        "leaks": {},          # wave -> [money, ...] (one entry per leak)
        "leak_kinds": {},     # kind -> leak-event count
        "builds": 0,
        "sells": 0,
        "clears": 0,          # pay-to-clear rock removals
        "upgrades": 0,        # in-match tower upgrades
        "walls": 0,           # subset of builds (kind == "wall")
        "builds_during": {},  # wave -> builds while it was the current wave
        "sells_during": {},
        "clears_during": {},
        "upgrades_during": {},
        "walls_during": {},
        "overcharges_during": {},
        "run_end": None,
        "mission_cleared": None,
        "harness_start": None,
        "harness_end": None,
        "run_starts": 0,
        "run_ends": 0,
        "unknown": {},
        "bad_lines": 0,
    }
    current = 0
    with open(path, encoding="utf-8", errors="replace") as f:
        for line in f:
            line = line.strip()
            if not line:
                continue
            try:
                event = json.loads(line)
            except json.JSONDecodeError:
                run["bad_lines"] += 1
                continue
            if not isinstance(event, dict) or not event.get("t"):
                run["bad_lines"] += 1
                continue
            kind = event["t"]
            if kind == "run_start":
                run["run_starts"] += 1
                run["seed"] = event.get("seed")
                run["source"] = event.get("source")
                run["harness"] = event.get("harness")
            elif kind == "wave":
                n = as_int(event.get("wave"))
                count = as_int(event.get("count"))
                if n is None or count is None:
                    run["bad_lines"] += 1
                    continue
                run["waves"][n] = {"count": count, "hp": event.get("hp")}
                if n not in run["order"]:
                    run["order"].append(n)
                current = n
            elif kind == "build":
                run["builds"] += 1
                run["builds_during"][current] = run["builds_during"].get(current, 0) + 1
                if (event.get("kind") or "gun") == "wall":
                    run["walls"] += 1
                    run["walls_during"][current] = run["walls_during"].get(current, 0) + 1
            elif kind == "sell":
                run["sells"] += 1
                run["sells_during"][current] = run["sells_during"].get(current, 0) + 1
            elif kind == "clear":
                run["clears"] += 1
                run["clears_during"][current] = run["clears_during"].get(current, 0) + 1
            elif kind == "upgrade":
                run["upgrades"] += 1
                run["upgrades_during"][current] = run["upgrades_during"].get(current, 0) + 1
            elif kind == "leak":
                n = as_int(event.get("wave"))
                if n is None:
                    run["bad_lines"] += 1
                    continue
                run["leaks"].setdefault(n, []).append(event.get("money"))
                leak_kind = event.get("kind")
                if isinstance(leak_kind, str) and leak_kind:
                    run["leak_kinds"][leak_kind] = run["leak_kinds"].get(leak_kind, 0) + 1
            elif kind == "kill":
                n = as_int(event.get("wave"))
                cell = event.get("cell")
                if n is None or not valid_cell(cell):
                    run["bad_lines"] += 1
                    continue
                run["kills_by_wave"][n] = run["kills_by_wave"].get(n, 0) + 1
                key = (as_int(cell[0]), as_int(cell[1]))
                run["kill_cells"][key] = run["kill_cells"].get(key, 0) + 1
                kill_kind = event.get("kind")
                if isinstance(kill_kind, str) and kill_kind:
                    run["kill_kinds"][kill_kind] = run["kill_kinds"].get(kill_kind, 0) + 1
            elif kind == "wave_end":
                n = as_int(event.get("wave"))
                if n is None:
                    run["bad_lines"] += 1
                    continue
                run["wave_ends"][n] = {
                    "kills": as_int(event.get("kills")),
                    "leaks": as_int(event.get("leaks")),
                    "money_start": event.get("money_start"),
                    "money_end": event.get("money_end"),
                }
            elif kind == "send":
                if as_int(event.get("wave")) is None:
                    run["bad_lines"] += 1
                    continue
                run["sends"] += 1
            elif kind == "time_control":
                # TimeControl (T19): pause/resume/speed; counted and rendered.
                run["time_controls"] += 1
            elif kind == "overcharge":
                run["overcharges"] += 1
                run["overcharges_during"][current] = (
                    run["overcharges_during"].get(current, 0) + 1
                )
            elif kind == "run_end":
                run["run_ends"] += 1
                run["run_end"] = {
                    "wave": as_int(event.get("wave")),
                    "money": event.get("money"),
                    "result": event.get("result"),
                    "endless": bool(event.get("endless")),
                }
            elif kind == "mission_cleared":
                run["mission_cleared"] = {
                    "wave": as_int(event.get("wave")),
                    "money": event.get("money"),
                    "kills": event.get("kills"),
                    "leaks": event.get("leaks"),
                }
            elif kind == "harness_start":
                run["harness_start"] = {
                    "waves": as_int(event.get("waves")),
                    "money": event.get("money"),
                    "dt": event.get("dt"),
                    "towers": event.get("towers"),
                    "walls": event.get("walls"),
                }
            elif kind == "harness_end":
                run["harness_end"] = {
                    "wave": as_int(event.get("wave")),
                    "reason": event.get("reason"),
                    "money": event.get("money"),
                    "kills": event.get("kills"),
                    "leaks": event.get("leaks"),
                    "steps": as_int(event.get("steps")),
                }
            else:
                run["unknown"][kind] = run["unknown"].get(kind, 0) + 1
    return run


def fmt_number(value, fmt="%g"):
    if value is None:
        return "-"
    if isinstance(value, (int, float)):
        return fmt % value
    return str(value)


def report_run(run):
    lines = []
    name = os.path.basename(run["path"])
    seed = run["seed"] if run["seed"] is not None else "?"
    mission = run["mission_cleared"]
    harness = run["harness_end"]
    if run["run_end"]:
        if mission and run["run_end"].get("endless"):
            result = "mission cleared at wave %s; endless ended at wave %s (money %s)" % (
                fmt_number(mission["wave"]),
                fmt_number(run["run_end"]["wave"]),
                run["run_end"]["money"],
            )
        else:
            result = "game over at wave %s (money %s)" % (
                fmt_number(run["run_end"]["wave"]),
                run["run_end"]["money"],
            )
    elif harness:
        reason = harness.get("reason") or "?"
        if mission:
            result = "mission cleared at wave %s; harness stopped at wave %s (%s)" % (
                fmt_number(mission["wave"]),
                fmt_number(harness["wave"]),
                reason,
            )
        else:
            result = "harness stopped at wave %s (%s)" % (fmt_number(harness["wave"]), reason)
    elif mission:
        last_wave = run["order"][-1] if run["order"] else mission["wave"]
        if last_wave is not None and last_wave > (mission["wave"] or 0):
            result = (
                "mission cleared at wave %s; endless reached wave %s "
                "(no run_end - window closed?)"
                % (fmt_number(mission["wave"]), fmt_number(last_wave))
            )
        else:
            result = "mission cleared at wave %s (money %s) - stopped at win screen" % (
                fmt_number(mission["wave"]),
                mission["money"],
            )
    else:
        result = "INCOMPLETE (no run_end - window closed?)"
    total_leaks = sum(len(m) for m in run["leaks"].values())
    provenance = " [%s]" % run["source"] if run["source"] and run["source"] != "local" else ""
    sends = " - sends %d" % run["sends"] if run["sends"] else ""
    time_controls = " - time %d" % run["time_controls"] if run["time_controls"] else ""
    lines.append(
        "%s - seed %s%s - %s%s%s"
        % (name, seed, provenance, result, sends, time_controls)
    )
    kill_values = [wave_kills(run, n) for n in run["order"]]
    if any(value is not None for value in kill_values):
        total_kills = sum(value for value in kill_values if value is not None)
        lines.append(
            "  builds %d (walls %d), sells %d, clears %d, upgrades %d, leaks %d, kills %d"
            % (
                run["builds"],
                run["walls"],
                run["sells"],
                run["clears"],
                run["upgrades"],
                total_leaks,
                total_kills,
            )
        )
    else:
        lines.append(
            "  builds %d (walls %d), sells %d, clears %d, upgrades %d, leaks %d"
            % (
                run["builds"],
                run["walls"],
                run["sells"],
                run["clears"],
                run["upgrades"],
                total_leaks,
            )
        )

    special_kinds = {"normal", "fast", "tank"}
    kind_parts = []
    for label, counts in (("kills", run["kill_kinds"]), ("leaks", run["leak_kinds"])):
        special = sorted(k for k in counts if k not in special_kinds)
        if special:
            kind_parts.append(
                "%s %s" % (label, ", ".join("%s %d" % (k, counts[k]) for k in special))
            )
    if kind_parts:
        lines.append("  kinds: " + " | ".join(kind_parts))

    if run["run_starts"] != 1:
        lines.append("  WARNING: %d run_start events (expected 1)" % run["run_starts"])
    if run["run_ends"] > 1:
        lines.append("  WARNING: %d run_end events (duplicate game-over?)" % run["run_ends"])
    if run["bad_lines"]:
        lines.append("  WARNING: %d unparsable lines" % run["bad_lines"])
    if run["unknown"]:
        lines.append("  note: unknown events %s" % run["unknown"])

    if not run["order"]:
        lines.append("  (no wave events)")
        return lines

    pre_builds = run["builds_during"].get(0, 0)
    pre_sells = run["sells_during"].get(0, 0)
    pre_upgrades = run["upgrades_during"].get(0, 0)
    if pre_builds or pre_sells or pre_upgrades or run["walls_during"].get(0, 0):
        lines.append(
            "  before wave 1: builds %d (walls %d), sells %d, upgrades %d"
            % (pre_builds, run["walls_during"].get(0, 0), pre_sells, pre_upgrades)
        )

    lines.append("  wave  count  hp       kills  leaks  money        build  sell   upg  spend")
    for n in run["order"]:
        wave = run["waves"][n]
        leaks = len(run["leaks"].get(n, []))
        summary = run["wave_ends"].get(n)
        if summary:
            money = "%s->%s" % (
                fmt_number(summary["money_start"]),
                fmt_number(summary["money_end"]),
            )
        else:
            money = fmt_number(run["leaks"][n][-1] if run["leaks"].get(n) else None)
        spend = (
            run["builds_during"].get(n, 0)
            + run["upgrades_during"].get(n, 0)
            + run["clears_during"].get(n, 0)
            + run["overcharges_during"].get(n, 0)
        )
        builds = run["builds_during"].get(n, 0)
        walls = run["walls_during"].get(n, 0)
        build_col = "%d(w%d)" % (builds, walls) if walls else str(builds)
        lines.append(
            "  %-5d %-6d %-8s %-6s %-6d %-12s %-6s %-6d %-5d %d"
            % (
                n,
                wave["count"],
                fmt_number(wave["hp"]),
                fmt_number(wave_kills(run, n)),
                leaks,
                money,
                build_col,
                run["sells_during"].get(n, 0),
                run["upgrades_during"].get(n, 0),
                spend,
            )
        )

    bars = []
    for n in run["order"]:
        leaks = len(run["leaks"].get(n, []))
        bars.append("W%d %d %s" % (n, leaks, "#" * min(leaks, 30)))
    lines.append("  leaks per wave: " + "   ".join(bars))

    if run["kill_cells"]:
        top = sorted(run["kill_cells"].items(), key=lambda item: (-item[1], item[0]))[:5]
        lines.append(
            "  kill zones: "
            + "  ".join("(%d,%d):%d" % (cell[0], cell[1], count) for cell, count in top)
        )
    return lines


def report_aggregate(runs):
    lines = ["Aggregate (%d run%s)" % (len(runs), "" if len(runs) == 1 else "s")]
    waves = sorted({n for run in runs for n in run["order"]})
    if waves:
        lines.append("  wave  runs  cleared  avg leaks  avg leak%  avg kills")
        for n in waves:
            subset = [run for run in runs if n in run["waves"]]
            if not subset:
                continue
            cleared = [run for run in subset if n != run["order"][-1] or n in run["wave_ends"]]
            count = sum(run["waves"][n]["count"] for run in subset) / len(subset)
            leaks = sum(len(run["leaks"].get(n, [])) for run in subset) / len(subset)
            known = [wave_kills(run, n) for run in subset]
            known = [value for value in known if value is not None]
            kills = sum(known) / len(known) if known else None
            lines.append(
                "  %-5d %-5d %-8d %-10.1f %-10.1f %s"
                % (
                    n,
                    len(subset),
                    len(cleared),
                    leaks,
                    100.0 * leaks / count if count else 0.0,
                    fmt_number(kills, "%.1f"),
                )
            )
    ended = [run for run in runs if run["run_end"] or run["mission_cleared"] or run["harness_end"]]
    if ended:
        reached = []
        for run in ended:
            # Session-end wave: the run_end if the session ended, else the
            # harness stop wave, else the furthest wave seen (mission clear or
            # an endless segment that was cut off without a run_end).
            if run["run_end"]:
                wave = run["run_end"]["wave"]
            elif run["harness_end"]:
                wave = run["harness_end"]["wave"]
            elif run["mission_cleared"]:
                wave = run["mission_cleared"]["wave"]
                if run["order"] and run["order"][-1] is not None:
                    wave = max(wave or 0, run["order"][-1])
            else:
                wave = None
            if wave is not None:
                reached.append(wave)
        summary = "  runs ended: %d/%d" % (len(ended), len(runs))
        if reached:
            summary += " - avg reached wave %.1f" % (sum(reached) / len(reached))
        lines.append(summary)
    return lines


def main(argv):
    inputs = argv[1:]
    files = []
    for path in inputs:
        if os.path.isdir(path):
            files.extend(sorted(glob.glob(os.path.join(path, "*.jsonl"))))
        else:
            files.append(path)
    if inputs and not files:
        print("No .jsonl files found in the given path(s): %s" % ", ".join(inputs))
        return 1
    if not files:
        files = discover()
    if not files:
        print("No run files found.")
        print("Looked in: %s" % LOCAL_DIR)
        directory = user_dir()
        print("       and: %s" % (directory or "(project.godot config/name not found)"))
        print("Play a run to game over (writes user://run_<seed>.jsonl),")
        print("or copy logs into telemetry_local/.")
        return 1

    runs = []
    for path in files:
        try:
            runs.append(load_run(path))
        except (OSError, ValueError) as error:
            print("SKIP %s: %s" % (path, error))
    if not runs:
        return 1

    for run in runs:
        print("\n".join(report_run(run)))
        print()
    print("\n".join(report_aggregate(runs)))
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))
