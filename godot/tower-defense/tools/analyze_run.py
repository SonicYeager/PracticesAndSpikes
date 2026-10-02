#!/usr/bin/env python3
"""Analyze Tower Defense telemetry runs (stdlib only).

Reads the JSONL files written by scripts/telemetry.gd (`user://run_<seed>.jsonl`)
and prints a per-run report plus an aggregate across runs.

Events: run_start {seed} · wave {wave,count,hp} · build {cell} · sell {cell}
        · leak {wave,money} · run_end {wave,money}

Derived numbers:
  kills      = count - leaks for every wave that was fully cleared (a later
               wave started); the final wave stays "-" because the run ended
               mid-wave and surviving drones are not telemetered.
  build/sell = activity while that wave was the current one (its run plus the
               following break); pre-wave-1 building is shown separately.
  money@leak = balance right after the wave's last leak (leak events carry the
               balance; kills/builds after that are not included).

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


def load_run(path):
    run = {
        "path": path,
        "seed": None,
        "waves": {},          # wave -> {"count": int, "hp": number|None}
        "order": [],          # wave numbers in event order
        "leaks": {},          # wave -> [money, ...] (one entry per leak)
        "builds": 0,
        "sells": 0,
        "builds_during": {},  # wave -> builds while it was the current wave
        "sells_during": {},
        "run_end": None,
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
            elif kind == "sell":
                run["sells"] += 1
                run["sells_during"][current] = run["sells_during"].get(current, 0) + 1
            elif kind == "leak":
                n = as_int(event.get("wave"))
                if n is None:
                    run["bad_lines"] += 1
                    continue
                run["leaks"].setdefault(n, []).append(event.get("money"))
            elif kind == "run_end":
                run["run_ends"] += 1
                run["run_end"] = {
                    "wave": as_int(event.get("wave")),
                    "money": event.get("money"),
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
    if run["run_end"]:
        result = "game over at wave %s (money %s)" % (
            fmt_number(run["run_end"]["wave"]),
            run["run_end"]["money"],
        )
    else:
        result = "INCOMPLETE (no run_end - window closed?)"
    total_leaks = sum(len(m) for m in run["leaks"].values())
    lines.append("%s - seed %s - %s" % (name, seed, result))
    lines.append("  builds %d, sells %d, leaks %d" % (run["builds"], run["sells"], total_leaks))

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
    if pre_builds or pre_sells:
        lines.append("  before wave 1: builds %d, sells %d" % (pre_builds, pre_sells))

    lines.append("  wave  count  hp       leaks  kills  money@leak  build  sell")
    for index, n in enumerate(run["order"]):
        wave = run["waves"][n]
        leaks = len(run["leaks"].get(n, []))
        resolved = index < len(run["order"]) - 1
        kills = str(max(wave["count"] - leaks, 0)) if resolved else "-"
        money = run["leaks"][n][-1] if run["leaks"].get(n) else None
        lines.append(
            "  %-5d %-6d %-8s %-6d %-6s %-11s %-6d %d"
            % (
                n,
                wave["count"],
                fmt_number(wave["hp"]),
                leaks,
                kills,
                fmt_number(money),
                run["builds_during"].get(n, 0),
                run["sells_during"].get(n, 0),
            )
        )

    bars = []
    for n in run["order"]:
        leaks = len(run["leaks"].get(n, []))
        bars.append("W%d %d %s" % (n, leaks, "#" * min(leaks, 30)))
    lines.append("  leaks per wave: " + "   ".join(bars))
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
            cleared = [run for run in subset if n != run["order"][-1]]
            count = sum(run["waves"][n]["count"] for run in subset) / len(subset)
            leaks = sum(len(run["leaks"].get(n, [])) for run in subset) / len(subset)
            kills = (
                sum(max(run["waves"][n]["count"] - len(run["leaks"].get(n, [])), 0) for run in cleared)
                / len(cleared)
                if cleared
                else None
            )
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
    ended = [run for run in runs if run["run_end"]]
    if ended:
        reached = [run["run_end"]["wave"] for run in ended if run["run_end"]["wave"] is not None]
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
