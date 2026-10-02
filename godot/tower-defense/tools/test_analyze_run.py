#!/usr/bin/env python3
"""Regression tests for analyze_run.py (stdlib unittest, no dependencies).

Run: python tools/test_analyze_run.py
"""
import os
import sys
import tempfile
import unittest

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import analyze_run as ar  # noqa: E402


def make_run(text, binary=False):
    fd, path = tempfile.mkstemp(suffix=".jsonl")
    mode = "wb" if binary else "w"
    kwargs = {} if binary else {"encoding": "utf-8", "newline": ""}
    with os.fdopen(fd, mode, **kwargs) as f:
        f.write(text)
    return path


class LoadRunTest(unittest.TestCase):
    def load(self, text, binary=False):
        path = make_run(text, binary)
        self.addCleanup(os.remove, path)
        return ar.load_run(path)

    def test_bad_json_and_non_dict_lines_counted(self):
        run = self.load('5\n[1, 2]\nnot json\n{"t":"wave","wave":1,"count":4}\n')
        self.assertEqual(run["bad_lines"], 3)
        self.assertEqual(run["order"], [1])

    def test_missing_type_counted_as_bad(self):
        run = self.load('{"foo": 1}\n{"t": null}\n')
        self.assertEqual(run["bad_lines"], 2)

    def test_null_wave_and_count_counted_as_bad(self):
        run = self.load(
            '{"t":"wave","wave":null,"count":4}\n'
            '{"t":"wave","wave":1,"count":null}\n'
            '{"t":"leak","wave":null,"money":90}\n'
        )
        self.assertEqual(run["bad_lines"], 3)

    def test_crlf_and_blank_lines(self):
        run = self.load('{"t":"run_start","seed":1}\r\n\r\n{"t":"wave","wave":1,"count":4}\r\n')
        self.assertEqual(run["seed"], 1)
        self.assertEqual(run["order"], [1])

    def test_undecodable_byte_does_not_abort(self):
        run = self.load(b'{"t":"run_start","seed":1}\n\xff\n', binary=True)
        self.assertEqual(run["seed"], 1)
        self.assertGreaterEqual(run["bad_lines"], 1)

    def test_build_sell_attribution_uses_current_wave(self):
        run = self.load(
            '{"t":"wave","wave":1,"count":4}\n'
            '{"t":"build","cell":[1,1]}\n'
            '{"t":"wave","wave":2,"count":6}\n'
            '{"t":"sell","cell":[1,1]}\n'
        )
        self.assertEqual(run["builds_during"], {1: 1})
        self.assertEqual(run["sells_during"], {2: 1})
        self.assertEqual(run["builds"], 1)
        self.assertEqual(run["sells"], 1)

    def test_pre_wave_activity_bucketed_at_zero(self):
        run = self.load('{"t":"build","cell":[1,1]}\n{"t":"wave","wave":1,"count":4}\n')
        self.assertEqual(run["builds_during"], {0: 1})

    def test_as_int(self):
        self.assertEqual(ar.as_int(3), 3)
        self.assertEqual(ar.as_int("3"), 3)
        self.assertIsNone(ar.as_int(None))
        self.assertIsNone(ar.as_int([1]))

    def test_report_smoke(self):
        run = self.load(
            '{"t":"run_start","seed":1}\n'
            '{"t":"wave","wave":1,"count":4,"hp":20.0}\n'
            '{"t":"build","cell":[1,1]}\n'
            '{"t":"wave","wave":2,"count":6,"hp":23.0}\n'
            '{"t":"leak","wave":2,"money":78}\n'
            '{"t":"run_end","wave":2,"money":-2}\n'
        )
        text = "\n".join(ar.report_run(run))
        self.assertIn("game over at wave 2 (money -2)", text)
        self.assertIn("hp       kills", text)
        self.assertIn("leaks per wave: W1 0    W2 1 #", text)

    def test_aggregate_smoke(self):
        run = self.load('{"t":"wave","wave":1,"count":4}\n{"t":"run_end","wave":1,"money":-1}\n')
        text = "\n".join(ar.report_aggregate([run]))
        self.assertIn("Aggregate (1 run)", text)
        self.assertIn("runs ended: 1/1", text)

    def test_kill_and_wave_end_parsed(self):
        run = self.load(
            '{"t":"wave","wave":1,"count":4,"hp":20.0}\n'
            '{"t":"kill","wave":1,"cell":[3,4],"kind":"fast"}\n'
            '{"t":"kill","wave":1,"cell":[3,4],"kind":"normal"}\n'
            '{"t":"leak","wave":1,"money":96}\n'
            '{"t":"leak","wave":1,"money":92}\n'
            '{"t":"wave_end","wave":1,"kills":2,"leaks":2,"money_start":100,"money_end":112}\n'
            '{"t":"wave","wave":2,"count":6,"hp":23.0}\n'
        )
        self.assertEqual(run["kills_by_wave"], {1: 2})
        self.assertEqual(run["wave_ends"][1]["kills"], 2)
        text = "\n".join(ar.report_run(run))
        self.assertIn("100→112", text)
        self.assertIn("leaks 2, kills 2", text)
        self.assertIn("kill zones: (3,4):2", text)

    def test_kill_events_give_kills_without_summary(self):
        run = self.load(
            '{"t":"wave","wave":1,"count":4}\n'
            '{"t":"kill","wave":1,"cell":[1,1],"kind":"tank"}\n'
            '{"t":"kill","wave":1,"cell":[2,2],"kind":"normal"}\n'
            '{"t":"kill","wave":1,"cell":[1,1],"kind":"normal"}\n'
        )
        self.assertEqual(ar.wave_kills(run, 1), 3)

    def test_provenance_and_sends_rendered(self):
        run = self.load(
            '{"t":"run_start","seed":7,"source":"harness","harness":true}\n'
            '{"t":"send","wave":1}\n'
            '{"t":"send","wave":2}\n'
            '{"t":"wave","wave":1,"count":4}\n'
            '{"t":"run_end","wave":1,"money":-1}\n'
        )
        text = "\n".join(ar.report_run(run))
        self.assertIn("[harness]", text)
        self.assertIn("sends 2", text)

    def test_time_control_is_known(self):
        run = self.load(
            '{"t":"wave","wave":1,"count":4}\n'
            '{"t":"time_control","action":"pause"}\n'
            '{"t":"time_control","action":"resume"}\n'
        )
        self.assertEqual(run["time_controls"], 2)
        self.assertEqual(run["unknown"], {})

    def test_bad_kill_and_wave_end_counted(self):
        run = self.load(
            '{"t":"kill","wave":null,"cell":[1,1]}\n'
            '{"t":"kill","wave":1,"cell":"nope"}\n'
            '{"t":"kill","wave":1,"cell":[1]}\n'
            '{"t":"wave_end","wave":null,"kills":1}\n'
            '{"t":"send","wave":null}\n'
        )
        self.assertEqual(run["bad_lines"], 5)

    def test_legacy_kills_still_derived(self):
        run = self.load(
            '{"t":"wave","wave":1,"count":4}\n'
            '{"t":"wave","wave":2,"count":6}\n'
            '{"t":"leak","wave":1,"money":90}\n'
        )
        self.assertEqual(ar.wave_kills(run, 1), 3)
        self.assertIsNone(ar.wave_kills(run, 2))


if __name__ == "__main__":
    unittest.main(verbosity=2)
