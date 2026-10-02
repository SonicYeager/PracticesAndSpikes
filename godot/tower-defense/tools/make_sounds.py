#!/usr/bin/env python3
"""Generate simple synthesized SFX as 16-bit mono WAVs (stdlib only).

Same philosophy as make_placeholders.py: every sound is a few lines of
synthesis below — edit, rerun, done. Hand-recorded sounds drop into
../audio/ under the same filenames, no code changes needed.

Usage: python3 make_sounds.py   (writes ../audio/*.wav)
"""
import math
import os
import random
import struct
import wave

HERE = os.path.dirname(os.path.abspath(__file__))
OUT = os.path.normpath(os.path.join(HERE, "..", "audio"))
RATE = 22050
RNG = random.Random(7)


def osc(shape, phase):
    if shape == "sine":
        return math.sin(phase)
    if shape == "square":
        return 1.0 if math.sin(phase) >= 0.0 else -1.0
    if shape == "saw":
        return 2.0 * ((phase / (2 * math.pi)) % 1.0) - 1.0
    raise ValueError(shape)


def sweep(f_start, f_end, dur, amp, shape="sine", decay=12.0, noise_mix=0.0, rng=RNG):
    """One tone whose frequency sweeps linearly; exponential amplitude decay."""
    n = int(RATE * dur)
    out = []
    phase = 0.0
    for i in range(n):
        t = i / RATE
        f = f_start + (f_end - f_start) * (i / max(n - 1, 1))
        phase += 2.0 * math.pi * f / RATE
        s = osc(shape, phase)
        if noise_mix > 0.0:
            s = (1.0 - noise_mix) * s + noise_mix * rng.uniform(-1.0, 1.0)
        out.append(amp * math.exp(-decay * t) * s)
    return out


def noise(dur, amp, decay=20.0, rng=RNG):
    n = int(RATE * dur)
    return [amp * math.exp(-decay * (i / RATE)) * rng.uniform(-1.0, 1.0) for i in range(n)]


def mix(*tracks):
    length = max(len(t) for t in tracks)
    out = [0.0] * length
    for track in tracks:
        for i, v in enumerate(track):
            out[i] += v
    return out


def seq(*tracks):
    out = []
    for track in tracks:
        out.extend(track)
    return out


def normalize(samples, peak=0.9):
    top = max(abs(s) for s in samples) or 1.0
    return [s * peak / top for s in samples]


def write_wav(path, samples):
    frames = b"".join(
        struct.pack("<h", int(max(-1.0, min(1.0, s)) * 32767)) for s in samples
    )
    with wave.open(path, "wb") as f:
        f.setnchannels(1)
        f.setsampwidth(2)
        f.setframerate(RATE)
        f.writeframes(frames)


def build_sounds():
    rng = random.Random(7)
    return {
        # Quiet zap: fires often, must not get annoying.
        "shoot": normalize(mix(
            sweep(880, 260, 0.09, 0.5, "square", decay=30.0, noise_mix=0.25, rng=rng),
            noise(0.04, 0.15, 70.0, rng),
        ), 0.7),
        "hit": normalize(noise(0.06, 0.6, 45.0, rng), 0.6),
        "kill": normalize(mix(
            noise(0.35, 0.7, 11.0, rng),
            sweep(160, 60, 0.30, 0.5, "sine", decay=9.0),
        ), 0.85),
        "leak": normalize(mix(
            sweep(320, 150, 0.35, 0.6, "square", decay=5.0),
            sweep(640, 300, 0.35, 0.2, "sine", decay=6.0),
        ), 0.8),
        "build": normalize(seq(
            noise(0.03, 0.5, 80.0, rng),
            sweep(660, 660, 0.06, 0.4, "square", decay=18.0),
            sweep(990, 990, 0.08, 0.4, "square", decay=14.0),
        ), 0.75),
        "sell": normalize(seq(
            sweep(880, 880, 0.05, 0.4, "square", decay=16.0),
            sweep(1320, 1320, 0.09, 0.4, "square", decay=12.0),
        ), 0.75),
        "denied": normalize(sweep(150, 120, 0.14, 0.6, "square", decay=10.0), 0.6),
        "wave": normalize(seq(
            sweep(330, 330, 0.16, 0.5, "saw", decay=6.0),
            sweep(440, 440, 0.22, 0.5, "saw", decay=5.0),
        ), 0.8),
        "gameover": normalize(seq(
            sweep(392, 392, 0.28, 0.5, "sine", decay=4.0),
            sweep(311, 311, 0.28, 0.5, "sine", decay=4.0),
            sweep(262, 262, 0.50, 0.5, "sine", decay=3.0),
        ), 0.85),
        # Vent overcharge (T10): deep whoosh plus crackle. Appended last so
        # the RNG draw order of the existing sounds stays stable.
        "overcharge": normalize(mix(
            sweep(120, 44, 0.50, 0.7, "saw", decay=6.0, rng=rng),
            sweep(240, 88, 0.35, 0.3, "sine", decay=8.0),
            noise(0.50, 0.5, 7.0, rng),
        ), 0.9),
    }


def main():
    os.makedirs(OUT, exist_ok=True)
    sounds = build_sounds()
    for name, samples in sounds.items():
        write_wav(os.path.join(OUT, name + ".wav"), samples)
    print(f"wrote {len(sounds)} sounds to {OUT}")


if __name__ == "__main__":
    main()
