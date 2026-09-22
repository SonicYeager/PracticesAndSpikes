#!/usr/bin/env python3
"""Generate Ember Foundry placeholder sprites as 16x16 (or 8x8) PNGs.

Stdlib only (zlib + struct), no Pillow. Each sprite is ASCII art:
one char per pixel, mapped through PAL. Edit the art, rerun, done.

Usage: python3 make_placeholders.py   (writes ../art/*.png)
"""
import os
import struct
import zlib

HERE = os.path.dirname(os.path.abspath(__file__))
OUT = os.path.normpath(os.path.join(HERE, "..", "art"))

# Ember Foundry palette (see ../art/STYLEGUIDE.md)
PAL = {
    ".": None,  # transparent
    "K": (0x24, 0x16, 0x10),  # outline, near-black warm
    "F": (0x1D, 0x16, 0x13),  # floor base
    "f": (0x22, 0x1A, 0x15),  # floor alt
    "R": (0x4A, 0x3A, 0x2C),  # rivet
    "S": (0x2E, 0x23, 0x1C),  # scratch / dark patch
    "O": (0xFF, 0xA0, 0x2E),  # gun body orange
    "D": (0xB3, 0x5C, 0x14),  # gun shade
    "H": (0xFF, 0xD1, 0x66),  # highlight / hot
    "M": (0x4A, 0x40, 0x38),  # gunmetal
    "T": (0xFF, 0x5A, 0x2E),  # hot tip / explosion
    "G": (0x7D, 0xDF, 0x64),  # drone green
    "g": (0x3F, 0x91, 0x42),  # drone shade
    "Y": (0xD7, 0xFF, 0x5E),  # fast drone lime
    "N": (0x2E, 0x9E, 0x6B),  # tank teal-green
    "n": (0x1E, 0x5F, 0x44),  # tank shade
    "E": (0xFF, 0x3B, 0x30),  # enemy eye red
    "V": (0x35, 0xD0, 0x7F),  # spawn vent glow
    "C": (0xFF, 0x78, 0x47),  # base core ember
    "P": (0xFF, 0xE0, 0x8A),  # projectile / muzzle
    "W": (0xFF, 0xFF, 0xFF),  # white hot core
}

FLOOR_0 = [
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "ffffffffffffffff",
]

FLOOR_1 = [
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFRFFFFFFFFFFRFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFRFFFFFFFFFFRFF",
    "FFFFFFFFFFFFFFFF",
    "ffffffffffffffff",
]

FLOOR_2 = [
    "FFFFFFFFFFFFFFFF",
    "FFSSSSFFFFFFFFFF",
    "FFSSSSSFFFFFFFFF",
    "FFFSSSSFFFFFFFFF",
    "FFFFSSFFFFFFFFFF",
    "FFFFFFFFFFFFSFFF",
    "FFFFFFFFFFFSFFFF",
    "FFFFFFFFFFSFFFFF",
    "FFFFFFFFFSFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "FFFFFFFFFFFFFFFF",
    "ffffffffffffffff",
]

GUN_BASE = [
    "................",
    "................",
    "................",
    "................",
    "..KKKKKKKKKKKK..",
    "..KHHHHHHHHHHK..",
    "..KHOOOOOOOOOH..",
    "..KOOOOMMMOOOK..",
    "..KOOOOMMMOOOK..",
    "..KOOOOOOOOOOK..",
    "..KOOOKKKKOOOK..",
    "..KOOOKKKKOOOK..",
    "..KDDDDDDDDDDK..",
    "..KDDDDDDDDDDK..",
    "..KKKKKKKKKKKK..",
    "................",
]

# Barrel points up; pivot ~ (8, 12) via Sprite2D offset (T03).
GUN_BARREL = [
    ".....KKKK.......",
    ".....KTTK.......",
    ".....KTTK.......",
    ".....KMMK.......",
    ".....KMMK.......",
    ".....KMMK.......",
    ".....KMMK.......",
    ".....KMMK.......",
    ".....KMMK.......",
    ".....KMMK.......",
    ".....KMMK.......",
    ".....KMMK.......",
    ".....KKKK.......",
    "................",
    "................",
    "................",
]

DRONE_0 = [
    "................",
    "................",
    "................",
    "................",
    "................",
    ".....KKKKK......",
    "...KKGGGGGK.....",
    "..KGGE EGGK......".replace(" ", ""),
    "..KGGE EGGK......".replace(" ", ""),
    "..KGGGGGGGK.....",
    "..KgGGGGGgK.....",
    "...KKKKKKK......",
    "................",
    "................",
    "................",
    "................",
]

DRONE_1 = [
    "................",
    "................",
    "................",
    "................",
    "................",
    ".....KKKKK......",
    "...KKGGGGGK.....",
    "..KGGGGGGGK.....",
    "..KGGE EGGK......".replace(" ", ""),
    "..KGGE EGGK......".replace(" ", ""),
    "..KgGGGGGgK.....",
    "...KKKKKKK......",
    "................",
    "................",
    "................",
    "................",
]

FAST_0 = [
    "................",
    "................",
    "................",
    "................",
    "................",
    ".......KK.......",
    ".....KKYYKK.....",
    "...KKYYYYYYKK...",
    "...KYYEEYYYYK...",
    "...KKYYYYYYKK...",
    ".....KKYYKK.....",
    ".......KK.......",
    "................",
    "................",
    "................",
    "................",
]

FAST_1 = [
    "................",
    "................",
    "................",
    "................",
    "................",
    "................",
    ".......KK.......",
    ".....KKYYKK.....",
    "...KKYYYYYYKK...",
    "...KYYEEYYYYK...",
    "...KKYYYYYYKK...",
    ".....KKYYKK.....",
    ".......KK.......",
    "................",
    "................",
    "................",
]

TANK = [
    "................",
    "................",
    "................",
    "................",
    "..KKKKKKKKKKKK..",
    ".KNNNNNNNNNNNNK.",
    ".KNNEENNNNEENNK.",
    ".KNNEENNNNEENNK.",
    ".KNNNNNNNNNNNNK.",
    ".KnNNNNNNNNNNnK.",
    "..KKKKKKKKKKKK..",
    "................",
    "................",
    "................",
    "................",
    "................",
]

SPAWN = [
    "................",
    "................",
    "..KKKKKKKKKKKK..",
    ".KVVVVVVVVVVVVK.",
    ".KVKKKKKKKKKKVK.",
    ".KVK........KVK.",
    ".KVK........KVK.",
    ".KVK........KVK.",
    ".KVK........KVK.",
    ".KVK........KVK.",
    ".KVK........KVK.",
    ".KVKKKKKKKKKKVK.",
    ".KVVVVVVVVVVVVK.",
    "..KKKKKKKKKKKK..",
    "................",
    "................",
]

BASE = [
    "................",
    "................",
    "..KKKKKKKKKKKK..",
    ".KMMMMMMMMMMMMK.",
    ".KMKCCCCCCCCKMK.",
    ".KMKCHHHHCCCKMK.",
    ".KMKCHWWHCCCKMK.",
    ".KMKCHWWHCCCKMK.",
    ".KMKCHHHHCCCKMK.",
    ".KMKCCCCCCCCKMK.",
    ".KMKCCCCCCCCKMK.",
    ".KMMMMMMMMMMMMK.",
    "..KKKKKKKKKKKK..",
    "................",
    "................",
    "................",
]

PROJECTILE = [
    "...PP...",
    "..PWWP..",
    ".PWWWWP.",
    "PPWWWWPP",
    ".PWWWWP.",
    "..PWWP..",
    "...PP...",
    "........",
]

MUZZLE = [
    "H..HH..H",
    "...HH...",
    "...HH...",
    "HHHHHHHH",
    "...HH...",
    "...HH...",
    "H..HH..H",
    "........",
]

EXPLOSION_0 = [
    "................",
    "................",
    "................",
    ".....TTTTT......",
    "...TTTOOOOTTT...",
    "..TTOOHHHHOOTT..",
    "..TOHHHHHHHOTT..",
    "..TOHHHWWHHOTT..",
    "..TOHHHWWHHOTT..",
    "..TOHHHHHHHOTT..",
    "...TTOOHHOOTT...",
    ".....TTTTT......",
    "................",
    "................",
    "................",
    "................",
]

EXPLOSION_1 = [
    "................",
    "................",
    "......H.H.......",
    "...H.HWHWH.H....",
    "....HW WWWH......".replace(" ", ""),
    "..H.WWWWWWW.H...",
    "....WWWWWWW.....",
    "..H.WWWWWWW.H...",
    "....HW WWWH......".replace(" ", ""),
    "...H.HWHWH.H....",
    "......H.H.......",
    "................",
    "................",
    "................",
    "................",
    "................",
]

SPRITES = {
    "floor_0": FLOOR_0,
    "floor_1": FLOOR_1,
    "floor_2": FLOOR_2,
    "gun_base": GUN_BASE,
    "gun_barrel": GUN_BARREL,
    "drone_0": DRONE_0,
    "drone_1": DRONE_1,
    "drone_fast_0": FAST_0,
    "drone_fast_1": FAST_1,
    "drone_tank": TANK,
    "spawn": SPAWN,
    "base": BASE,
    "projectile": PROJECTILE,
    "muzzle": MUZZLE,
    "explosion_0": EXPLOSION_0,
    "explosion_1": EXPLOSION_1,
}


def write_png(path, art):
    h = len(art)
    w = len(art[0])
    assert (w, h) in [(16, 16), (8, 8)], f"bad size {(w, h)} in {path}"
    for row in art:
        assert len(row) == w, f"ragged row in {path}: {row!r}"
        for ch in row:
            assert ch in PAL, f"unknown char {ch!r} in {path}"
    raw = bytearray()
    for row in art:
        raw.append(0)  # filter: none
        for ch in row:
            px = PAL[ch]
            if px is None:
                raw.extend((0, 0, 0, 0))
            else:
                raw.extend((*px, 255))
    ihdr = struct.pack(">IIBBBBB", w, h, 8, 6, 0, 0, 0)

    def chunk(tag, data):
        c = struct.pack(">I", len(data)) + tag + data
        return c + struct.pack(">I", zlib.crc32(tag + data) & 0xFFFFFFFF)

    png = (
        b"\x89PNG\r\n\x1a\n"
        + chunk(b"IHDR", ihdr)
        + chunk(b"IDAT", zlib.compress(bytes(raw), 9))
        + chunk(b"IEND", b"")
    )
    with open(path, "wb") as f:
        f.write(png)


def main():
    os.makedirs(OUT, exist_ok=True)
    for name, art in SPRITES.items():
        write_png(os.path.join(OUT, name + ".png"), art)
    print(f"wrote {len(SPRITES)} sprites to {OUT}")


if __name__ == "__main__":
    main()
