#!/usr/bin/env python3
"""Generate Xeno-Tactic-inspired placeholder sprites as 16x16 (or 8x8) PNGs.

Stdlib only (zlib + struct), no Pillow. Each sprite is ASCII art:
one char per pixel, mapped through PAL. Edit the art, rerun, done.

Palette/look: Xeno Tactic reference (see ../art/STYLEGUIDE.md "Zielbild") —
dark steel lab tiles, blue player turrets, white/green alien bugs, warm
pixel effects. Study reference only: nothing is copied from the original.

Usage: python3 make_placeholders.py   (writes ../art/*.png)
"""
import math
import os
import struct
import zlib

HERE = os.path.dirname(os.path.abspath(__file__))
OUT = os.path.normpath(os.path.join(HERE, "..", "art"))

# XT palette (see ../art/STYLEGUIDE.md)
PAL = {
    ".": None,                  # transparent
    "K": (0x0B, 0x10, 0x16),    # outline, near-black blue
    "F": (0x1B, 0x22, 0x2C),    # floor base, dark steel
    "f": (0x21, 0x29, 0x34),    # floor alt
    "R": (0x4A, 0x56, 0x66),    # rivet / light steel
    "S": (0x2A, 0x33, 0x40),    # scratch / dark panel
    "B": (0x2F, 0x3B, 0x4A),    # panel blue (props)
    "O": (0x5A, 0x8F, 0xC8),    # player blue
    "D": (0x33, 0x5E, 0x8C),    # player blue shade
    "M": (0x7A, 0x86, 0x96),    # gunmetal light
    "H": (0x9F, 0xD8, 0xFF),    # cyan highlight
    "T": (0xFF, 0x8A, 0x2E),    # fire orange
    "C": (0xFF, 0xB4, 0x4A),    # ember
    "G": (0xD8, 0xDE, 0xE6),    # alien white
    "g": (0x9A, 0xA4, 0xB0),    # alien shade
    "Y": (0x8E, 0xE0, 0x4A),    # acid green
    "N": (0x5A, 0x66, 0x72),    # tank gray-blue
    "n": (0x3A, 0x44, 0x50),    # tank shade
    "E": (0xFF, 0x3B, 0x30),    # eye red / hazard red
    "V": (0x54, 0xE0, 0x8A),    # breach green
    "P": (0x9F, 0xD8, 0xFF),    # projectile cyan
    "W": (0xFF, 0xFF, 0xFF),    # white hot core
    "U": (0xFF, 0xD7, 0x5E),    # gold
    "u": (0xB8, 0x8A, 0x2E),    # gold shade
    "Z": (0xE8, 0xE4, 0xDA),    # hazard white
}

FLOOR_0 = [
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "SSSSSSSSSSSSSSSS",
]

FLOOR_1 = [
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFRFFFFFFFFFFRFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFRFFFFFFFFFFRFS",
    "FFFFFFFFFFFFFFFS",
    "SSSSSSSSSSSSSSSS",
]

FLOOR_2 = [
    "FFFFFFFFFFFFFFFS",
    "FFSSSSFFFFFFFFFS",
    "FFSSSSSFFFFFFFFS",
    "FFFSSSSFFFFFFFFS",
    "FFFFSSFFFFFFFFFS",
    "FFFFFFFFFFFFSFFS",
    "FFFFFFFFFFFSFFFS",
    "FFFFFFFFFFSFFFFS",
    "FFFFFFFFFSFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "FFFFFFFFFFFFFFFS",
    "SSSSSSSSSSSSSSSS",
]

GUN_BASE = [
    "................",
    "................",
    "................",
    "....KKKKKKKK....",
    "..KKMMMMMMMMKK..",
    "..KMMRRRRRRMMK..",
    "..KMSSSSSSSSMK..",
    "..KMSSOOOOOSMK..",
    "..KMSSOHHOSSMK..",
    "..KMSSOHHOSSMK..",
    "..KMSSOOOOOSMK..",
    "..KMSSSSSSSSMK..",
    "..KMMRRRRRRMMK..",
    "..KKMMMMMMMMKK..",
    "....KKKKKKKK....",
    "................",
]

# Barrel points up; pivot ~ (8, 12) via Sprite2D offset (T03).
GUN_BARREL = [
    ".....KKKK.......",
    ".....KHHK.......",
    ".....KHHK.......",
    ".....KMKK.......",
    ".....KMKK.......",
    ".....KMKK.......",
    ".....KMKK.......",
    ".....KMKK.......",
    ".....KMKK.......",
    ".....KMKK.......",
    ".....KMKK.......",
    ".....KMKK.......",
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

# Dark breach with a green rim: "they come from here".
SPAWN = [
    "................",
    "................",
    ".....KKKKKK.....",
    "...KKVVVVVVKK...",
    "..KVVKKKKKKVVK..",
    "..KVKKKKKKKKVK..",
    ".KVVKKKKKKKKVVK.",
    ".KVKKKKKKKKKKVK.",
    ".KVKKKKKKKKKKVK.",
    ".KVVKKKKKKKKVVK.",
    "..KVKKKKKKKKVK..",
    "..KVVKKKKKKVVK..",
    "...KKVVVVVVKK...",
    ".....KKKKKK.....",
    "................",
    "................",
]

# Containment door with hazard stripes: "they want to get out".
BASE = [
    "................",
    "................",
    "..KKKKKKKKKKKK..",
    "..KMMMMMMMMMMK..",
    "..KMEEZZEEZZMK..",
    "..KMZZEEZZEEMK..",
    "..KMEEZZEEZZMK..",
    "..KMZZEEZZEEMK..",
    "..KMEEZZEEZZMK..",
    "..KMMMMMMMMMMK..",
    "..KMRRRRRRRRMK..",
    "..KMMMMMMMMMMK..",
    "..KKKKKKKKKKKK..",
    "................",
    "................",
    "................",
]

IMPACT = [
    "........",
    "...TT...",
    "..TCCT..",
    ".TCWWCT.",
    ".TCWWCT.",
    "..TCCT..",
    "...TT...",
    "........",
]

HUD_COIN = [
    "................",
    "................",
    ".....KKKKKK.....",
    "...KKUUUUUUKK...",
    "..KWUUUUUUUUUK..",
    "..KWUUUUUUUUUK..",
    ".KUUUuuuuuuUUUK.",
    ".KUUuuuuuuuuUUK.",
    ".KUUuuuuuuuuUUK.",
    ".KUUUuuuuuuUUUK.",
    "..KUUUUUUUUUUK..",
    "..KUUUUUUUUUUK..",
    "...KKUUUUUUKK...",
    ".....KKKKKK.....",
    "................",
    "................",
]

# Double chevron ">>": incoming wave.
HUD_WAVE = [
    "................",
    "................",
    "................",
    "..K........K....",
    "..KK......KK....",
    "..KHK....KHK....",
    "..KHHK..KHHK....",
    "..KHHHKKHHHK....",
    "..KHHHHHHHHK....",
    "..KHHHKKHHHK....",
    "..KHHK..KHHK....",
    "..KHK....KHK....",
    "..KK......KK....",
    "..K........K....",
    "................",
    "................",
]

HUD_SPACE = [
    "................",
    "................",
    "................",
    "..KKKKKKKKKKKK..",
    ".KMMMMMMMMMMMMK.",
    ".KMMMMMMMMMMMMK.",
    ".KMMKKKKKKKKMMK.",
    ".KMMKKKKKKKKMMK.",
    ".KMMMMMMMMMMMMK.",
    ".KMMMMMMMMMMMMK.",
    ".KSSSSSSSSSSSSK.",
    "..KKKKKKKKKKKK..",
    "................",
    "................",
    "................",
    "................",
]

HUD_MOUSE_LEFT = [
    "................",
    "................",
    "....KKKKKKKK....",
    "...KOOOOMMMMK...",
    "..KOOOOOMMMMMK..",
    "..KOOOOOMMMMMK..",
    "..KOOOOOMMMMMK..",
    "..KOOOOOMMMMMK..",
    "..KOOOOOMMMMMK..",
    "..KOOOOOMMMMMK..",
    "..KOOOOOMMMMMK..",
    "..KOOOOOMMMMMK..",
    "..KKKKKKKKKKKK..",
    "................",
    "................",
    "................",
]

HUD_MOUSE_RIGHT = [
    "................",
    "................",
    "....KKKKKKKK....",
    "...KMMMMOOOOK...",
    "..KMMMMMOOOOOK..",
    "..KMMMMMOOOOOK..",
    "..KMMMMMOOOOOK..",
    "..KMMMMMOOOOOK..",
    "..KMMMMMOOOOOK..",
    "..KMMMMMOOOOOK..",
    "..KMMMMMOOOOOK..",
    "..KMMMMMOOOOOK..",
    "..KKKKKKKKKKKK..",
    "................",
    "................",
    "................",
]

# T09 terrain: blocking clusters + cosmetic decor + battle decals (lab look).
ROCK = [
    "................",
    "................",
    "..KKKKKKKKKKKK..",
    "..KMMMMMMMMMMK..",
    "..KMSSSSSSSSMK..",
    "..KMSBBBBBBBMK..",
    "..KMSBBBBBBBMK..",
    "..KMSBBBBBBBMK..",
    "..KMSSSSSSSSMK..",
    "..KMSSSSSSSSMK..",
    "..KMMMMMMMMMMK..",
    "..KRRRRRRRRRRK..",
    "..KKKKKKKKKKKK..",
    "................",
    "................",
    "................",
]

RUBBLE = [
    "................",
    "................",
    "................",
    "................",
    "................",
    "................",
    ".....KK...KK....",
    "....KSSK.KRSK...",
    "..KKKSSKKKSSKK..",
    ".KSSSSSSSSSSSSK.",
    ".KSSRSSSSRSSSSK.",
    "..KSSSSSSSSSSK..",
    "...KKKKKKKKKK...",
    "................",
    "................",
    "................",
]

VENT = [
    "................",
    "................",
    "..KKKKKKKKKKKK..",
    "..KMMMMMMMMMMK..",
    "..KMKKKKKKKKMK..",
    "..KMKCCCCCCKMK..",
    "..KMKCHHHHCKMK..",
    "..KMKCHWWHCKMK..",
    "..KMKCHWWHCKMK..",
    "..KMKCHHHHCKMK..",
    "..KMKCCCCCCKMK..",
    "..KMKKKKKKKKMK..",
    "..KMMMMMMMMMMK..",
    "..KKKKKKKKKKKK..",
    "................",
    "................",
]

DECOR_CRACK = [
    "................",
    "................",
    "........S.......",
    "........S.......",
    ".......S........",
    ".......S........",
    "......SS........",
    "......S.........",
    ".......S........",
    ".......S.S......",
    "........S.......",
    "........S.......",
    "........S.......",
    "................",
    "................",
    "................",
]

DECOR_STAIN = [
    "................",
    "................",
    "................",
    "....SS...S......",
    "...SSSS.SSS.....",
    "..SSSSSSSSSS....",
    "..SSSSSSSSSS....",
    "...SSSSSSSSS....",
    "....SSSSSSS.....",
    "...SS..SSS......",
    "........S.......",
    "................",
    "................",
    "................",
    "................",
    "................",
]

SCORCH = [
    "................",
    "................",
    ".....KKKKK......",
    "...KKSSSSSKK....",
    "..KSSSSSSSSSK...",
    "..KSSSSSSSSSK...",
    ".KSSSSSSSSSSSK..",
    ".KSSSSSSSSSSSK..",
    ".KSSSSSTSSSSSK..",
    "..KSSSSSSSSSK...",
    "..KSSKSSSSSK....",
    "...KKSKSSKK.....",
    ".....KKKK.......",
    "................",
    "................",
    "................",
]

SKID = [
    "................",
    "................",
    "................",
    "................",
    ".....S..........",
    "....SS..........",
    "...SS...........",
    "..SS............",
    "..S.............",
    ".SS.............",
    ".S..............",
    "................",
    "................",
    "................",
    "................",
    "................",
]

DEBRIS = [
    "................",
    "................",
    "................",
    "................",
    "................",
    ".......S........",
    "....S.SSS.......",
    "...SSS.S.S......",
    "....S..SS.......",
    ".......S........",
    "................",
    "................",
    "................",
    "................",
    "................",
    "................",
]

# Neutral white dot: tinted per emitter (embers, sparks, smoke).
EMBER = [
    "...WW...",
    "..WWWW..",
    ".WWWWWW.",
    ".WWWWWW.",
    "..WWWW..",
    "...WW...",
    "........",
    "........",
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
    "...HH...",
    "HHHHHHHH",
    "...HH...",
    "...HH...",
    "...HH...",
]

EXPLOSION_0 = [
    "................",
    "................",
    "................",
    ".....TTTTT......",
    "...TTTCCCTTT....",
    "..TTCCCWWCCTT...",
    "..TCCWWWWWCCT...",
    "..TCWWWWWWWCT...",
    "..TCWWWWWWWCT...",
    "..TCCWWWWWCCT...",
    "...TTCCCWWTT....",
    ".....TTTTT......",
    "................",
    "................",
    "................",
    "................",
]

EXPLOSION_1 = [
    "................",
    "................",
    "......C.C.......",
    "...C.CWCWC.C....",
    "....CWWWWWC.....",
    "..C.WWWWWWW.C...",
    "....WWWWWWW.....",
    "..C.WWWWWWW.C...",
    "....CWWWWWC.....",
    "...C.CWCWC.C....",
    "......C.C.......",
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
    "impact": IMPACT,
    "explosion_0": EXPLOSION_0,
    "explosion_1": EXPLOSION_1,
    "hud_coin": HUD_COIN,
    "hud_wave": HUD_WAVE,
    "hud_space": HUD_SPACE,
    "hud_mouse_left": HUD_MOUSE_LEFT,
    "hud_mouse_right": HUD_MOUSE_RIGHT,
    "rock": ROCK,
    "rubble": RUBBLE,
    "vent": VENT,
    "decor_crack": DECOR_CRACK,
    "decor_stain": DECOR_STAIN,
    "scorch": SCORCH,
    "skid": SKID,
    "debris": DEBRIS,
    "ember": EMBER,
}


def write_png_raw(path, w, h, raw):
    """Write an 8-bit RGBA PNG from pre-filtered scanline bytes."""
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
    write_png_raw(path, w, h, raw)


# Vignette: near-black blue alpha ramp towards the corners (screen overlay).
VIGNETTE_W, VIGNETTE_H = 640, 360
VIGNETTE_INNER = 0.45  # normalized distance where darkening starts
VIGNETTE_OUTER = 1.15  # normalized distance at full opacity
VIGNETTE_ALPHA = 0.55  # max edge opacity


def vignette_raw(w, h):
    cx, cy = w * 0.5, h * 0.5
    base = PAL["K"]
    raw = bytearray()
    for y in range(h):
        raw.append(0)  # filter: none
        dy = (y + 0.5 - cy) / cy
        for x in range(w):
            dx = (x + 0.5 - cx) / cx
            t = (math.hypot(dx, dy) - VIGNETTE_INNER) / (VIGNETTE_OUTER - VIGNETTE_INNER)
            t = min(max(t, 0.0), 1.0)
            raw.extend((*base, int(255 * VIGNETTE_ALPHA * t * t)))
    return raw


def main():
    os.makedirs(OUT, exist_ok=True)
    for name, art in SPRITES.items():
        write_png(os.path.join(OUT, name + ".png"), art)
    raw = vignette_raw(VIGNETTE_W, VIGNETTE_H)
    write_png_raw(os.path.join(OUT, "vignette.png"), VIGNETTE_W, VIGNETTE_H, raw)
    print(f"wrote {len(SPRITES)} sprites + vignette to {OUT}")


if __name__ == "__main__":
    main()
