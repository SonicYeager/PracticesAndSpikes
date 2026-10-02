# Audio

All SFX are synthesized by `tools/make_sounds.py` — no samples, stdlib only
(`math`, `random`, `struct`, `wave`), same philosophy as the art
placeholders: editable in the script, regenerable at any time, replaceable
by hand-made recordings under the same filenames.

## Format

- 22050 Hz, mono, 16-bit PCM WAV (`audio/*.wav`, 9 files)
- Godot's default WAV import applies (no loop); the generated `.import`
  files are committed
- Synthesis: `sweep()` (linear frequency sweep + exponential decay, optional
  noise mix), `noise()`, `mix()`, `seq()`, `normalize()`

## Sounds

| File | Trigger | Character | Length | Runtime dB |
|---|---|---|---|---|
| `shoot.wav` | every shot | short square zap, 880→260 Hz, slight noise | 0.09 s | −15 |
| `hit.wav` | non-lethal hit | tiny noise tick | 0.06 s | −16 |
| `kill.wav` | drone killed | noise burst + 160→60 Hz thump | 0.35 s | −8 |
| `leak.wav` | drone reaches base | descending two-tone alarm | 0.35 s | −4 |
| `build.wav` | gun built | click + 660/990 Hz coin | 0.17 s | −8 |
| `sell.wav` | gun sold | 880/1320 Hz coin | 0.14 s | −8 |
| `denied.wav` | rejected build / no funds | low 150→120 Hz buzz | 0.14 s | −10 |
| `wave.wav` | wave starts | two-note saw horn (330→440 Hz) | 0.38 s | −6 |
| `gameover.wav` | run ends | descending sine (392/311/262 Hz) | 1.06 s | −4 |
| `overcharge.wav` | vent overcharged | deep saw whoosh (120→44 Hz) + crackle | 0.50 s | −6 |

Volume intent: `shoot` fires several times per second, so it is the quietest;
`leak` and `gameover` are the most important signals and the loudest.

## Zielbild: Xeno Tactic (Flash-Ära)

Referenz (SWF-Extraktion 2026-10-02): 30 kurze Original-Sounds, funktional
benannt (`THUNK`, `SPLAT`, `pop`, …). Charakter: trocken, knapp, Alarm vor
Melodie — kein Score, keine Loops.

- Unser stdlib-Synth (`tools/make_sounds.py`) trifft das schon; im XT-Pass
  werden die Rezepte in diese Richtung gezogen: Schuss = trockener Zap,
  Treffer = kurzer Thunk, Kill = Splat/Crunch, Leak = aufdringlicher Alarm,
  UI = harte Beeps.
- Keine Originaldateien übernehmen — nur den Charakter nachbilden.

## Runtime wiring (`scripts/game.gd`)

- `SFX` — const dictionary, one `preload()` per file
- `SFX_DB` — per-sound `volume_db`
- `_setup_audio()` — one `AudioStreamPlayer` per sound, `max_polyphony = 8`
  so rapid shots/hits overlap instead of cutting each other off
- `_play("name")` — null-safe trigger, called from the event sites listed
  above (build/sell/denied in `_unhandled_input`, wave in `_start_wave`,
  leak in `_on_leak`, hit/kill in `_apply_damage`, shoot in `_fire`,
  overcharge in `_try_overcharge`, gameover in `_end_run`)

There is no bus routing or music yet; the default Master bus is used.

## Regenerate / edit

```bash
python tools/make_sounds.py
godot --headless --path . --import --quit
```

## Add a new sound

1. Add a recipe to `build_sounds()` in `tools/make_sounds.py`.
2. Add `"<name>": preload("res://audio/<name>.wav")` to `SFX` in `game.gd`.
3. Add its `volume_db` to `SFX_DB`.
4. Call `_play("<name>")` at the trigger site.
5. Regenerate + import (commands above).

## Replace with recordings

Drop a file with the same name into `audio/`, keep it short and peak-normal,
rerun the import — no code changes. If the format differs (e.g. 44.1 kHz
stereo), Godot imports it fine; only the generator assumes 22050 Hz mono.
