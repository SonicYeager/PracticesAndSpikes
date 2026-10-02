#!/usr/bin/env python3
"""Generate Xeno-Tactic-inspired placeholder sprites as 32x32 (fx 16x16) PNGs.

Stdlib only (zlib + struct), no Pillow. Sprites are painted with a small
pixel canvas (rect/disc/ellipse/ring/dither/bevel helpers) in the XT palette;
hand art can replace any file under the same name, no code changes needed.

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
C = {
    "outline": (0x0B, 0x10, 0x16),   # near-black blue
    "floor": (0x1B, 0x22, 0x2C),     # dark steel
    "floor_alt": (0x21, 0x29, 0x34),
    "rivet": (0x4A, 0x56, 0x66),     # light steel
    "scratch": (0x2A, 0x33, 0x40),   # dark panel
    "panel": (0x2F, 0x3B, 0x4A),     # panel blue
    "blue": (0x5A, 0x8F, 0xC8),      # player blue
    "blue_dark": (0x33, 0x5E, 0x8C),
    "gunmetal": (0x7A, 0x86, 0x96),
    "cyan": (0x9F, 0xD8, 0xFF),
    "orange": (0xFF, 0x8A, 0x2E),
    "ember": (0xFF, 0xB4, 0x4A),
    "alien": (0xD8, 0xDE, 0xE6),
    "alien_shade": (0x9A, 0xA4, 0xB0),
    "acid": (0x8E, 0xE0, 0x4A),
    "tank": (0x5A, 0x66, 0x72),
    "tank_dark": (0x3A, 0x44, 0x50),
    "red": (0xFF, 0x3B, 0x30),
    "green": (0x54, 0xE0, 0x8A),
    "white": (0xFF, 0xFF, 0xFF),
    "gold": (0xFF, 0xD7, 0x5E),
    "gold_dark": (0xB8, 0x8A, 0x2E),
    "hazard": (0xE8, 0xE4, 0xDA),
}


def mix(a, b, t):
    return tuple(int(round(x + (y - x) * t)) for x, y in zip(a, b))


def lighten(color, t):
    return mix(color, (255, 255, 255), t)


def darken(color, t):
    return mix(color, (0, 0, 0), t)


class Canvas:
    """Tiny RGBA pixel canvas; None is transparent."""

    def __init__(self, w=32, h=32):
        self.w, self.h = w, h
        self.px = [[None] * w for _ in range(h)]

    def in_bounds(self, x, y):
        return 0 <= x < self.w and 0 <= y < self.h

    def set(self, x, y, color):
        if self.in_bounds(x, y):
            self.px[y][x] = color

    def get(self, x, y):
        return self.px[y][x] if self.in_bounds(x, y) else None

    def fill(self, color):
        for y in range(self.h):
            for x in range(self.w):
                self.px[y][x] = color

    def rect(self, x0, y0, x1, y1, color):
        for y in range(y0, y1 + 1):
            for x in range(x0, x1 + 1):
                self.set(x, y, color)

    def hline(self, x0, x1, y, color):
        for x in range(x0, x1 + 1):
            self.set(x, y, color)

    def vline(self, x, y0, y1, color):
        for y in range(y0, y1 + 1):
            self.set(x, y, color)

    def disc(self, cx, cy, r, color):
        for y in range(self.h):
            for x in range(self.w):
                if (x - cx) ** 2 + (y - cy) ** 2 <= r * r + 0.25:
                    self.set(x, y, color)

    def ellipse(self, cx, cy, rx, ry, color):
        for y in range(self.h):
            for x in range(self.w):
                if ((x - cx) / rx) ** 2 + ((y - cy) / ry) ** 2 <= 1.0:
                    self.set(x, y, color)

    def diamond(self, cx, cy, rx, ry, color):
        for y in range(self.h):
            for x in range(self.w):
                if abs(x - cx) / rx + abs(y - cy) / ry <= 1.0:
                    self.set(x, y, color)

    def ring(self, cx, cy, r, color, thickness=1.0):
        for y in range(self.h):
            for x in range(self.w):
                if abs(math.hypot(x - cx, y - cy) - r) <= thickness / 2.0:
                    self.set(x, y, color)

    def dither(self, x0, y0, x1, y1, color, phase=0):
        for y in range(y0, y1 + 1):
            for x in range(x0, x1 + 1):
                if (x + y + phase) % 2 == 0:
                    self.set(x, y, color)

    def bevel(self, light=0.22, dark=0.28):
        """1px rim light on top edges, shade on bottom edges."""
        out = [row[:] for row in self.px]
        for y in range(self.h):
            for x in range(self.w):
                color = self.px[y][x]
                if color is None or color == C["outline"]:
                    continue
                above = self.get(x, y - 1)
                below = self.get(x, y + 1)
                if above is None or above == C["outline"]:
                    out[y][x] = lighten(color, light)
                elif below is None or below == C["outline"]:
                    out[y][x] = darken(color, dark)
        self.px = out

    def outline(self, color=None):
        """1px contour around every opaque region."""
        color = color or C["outline"]
        out = [row[:] for row in self.px]
        for y in range(self.h):
            for x in range(self.w):
                if self.px[y][x] is not None:
                    continue
                neighbors = (
                    self.get(x + 1, y), self.get(x - 1, y),
                    self.get(x, y + 1), self.get(x, y - 1),
                )
                if any(n is not None for n in neighbors):
                    out[y][x] = color
        self.px = out


def rivet(c, x, y):
    c.rect(x - 1, y - 1, x + 1, y + 1, C["rivet"])
    c.set(x, y, C["outline"])


def chevron(c, x0, yc, size, color):
    """A '>' band of thickness 4."""
    for i in range(size):
        c.vline(x0 + i, yc - size + i, yc - size + i + 3, color)
        c.vline(x0 + i, yc + size - i - 3, yc + size - i, color)


# ---------------------------------------------------------------- sprites

def floor_tile(variant):
    c = Canvas()
    c.fill(C["floor"])
    for y in range(32):
        for x in range(32):
            if (x * 7 + y * 13) % 29 == 0:
                c.set(x, y, C["floor_alt"])
    c.hline(0, 31, 31, C["scratch"])
    c.vline(31, 0, 31, C["scratch"])
    if variant == 0:
        rivet(c, 6, 6)
        rivet(c, 25, 25)
    elif variant == 1:
        rivet(c, 5, 5)
        rivet(c, 26, 5)
        rivet(c, 5, 26)
        rivet(c, 26, 26)
    else:
        for i in range(11):
            c.set(7 + i, 23 - i, C["scratch"])
            c.set(7 + i, 24 - i, C["scratch"])
        c.disc(23, 8, 4, darken(C["floor"], 0.2))
        c.disc(24, 9, 2, darken(C["floor"], 0.32))
    return c


def gun_base():
    c = Canvas()
    c.disc(16, 16, 13, C["gunmetal"])
    c.ring(16, 16, 12, C["rivet"])
    c.disc(16, 16, 10, C["scratch"])
    c.disc(16, 16, 8, C["blue"])
    c.disc(16, 16, 3, C["cyan"])
    for x, y in ((16, 4), (16, 27), (4, 16), (27, 16)):
        c.disc(x, y, 2, C["rivet"])
        c.set(x, y, C["outline"])
    c.bevel(0.18, 0.22)
    c.outline()
    return c


def gun_barrel():
    c = Canvas()
    c.rect(10, 22, 21, 27, C["gunmetal"])          # base flange
    c.rect(12, 19, 19, 23, C["rivet"])             # collar
    c.rect(13, 4, 18, 21, C["gunmetal"])           # barrel
    c.vline(14, 4, 20, lighten(C["gunmetal"], 0.28))
    c.vline(18, 4, 20, darken(C["gunmetal"], 0.32))
    c.rect(7, 6, 9, 17, C["gunmetal"])             # left prong
    c.vline(9, 6, 17, C["cyan"])
    c.rect(22, 6, 24, 17, C["gunmetal"])           # right prong
    c.vline(22, 6, 17, C["cyan"])
    c.rect(7, 5, 9, 6, C["cyan"])                  # prong tips (shorter
    c.rect(22, 5, 24, 6, C["cyan"])                #  than the muzzle)
    c.rect(10, 12, 12, 14, C["gunmetal"])          # claw-to-barrel struts
    c.rect(19, 12, 21, 14, C["gunmetal"])
    c.rect(14, 2, 17, 4, C["cyan"])                # muzzle
    c.rect(15, 1, 16, 2, C["white"])
    c.set(12, 25, C["outline"])
    c.set(19, 25, C["outline"])
    c.bevel(0.15, 0.2)
    c.outline()
    return c


def drone_normal(frame):
    c = Canvas()
    step = 1 if frame else 0
    # Three leg pairs; the gait alternates with the frame.
    for i, y in enumerate((12, 17, 22)):
        dy = step * (1 if i % 2 == 0 else -1)
        c.rect(4, y + dy, 6, y + 2 + dy, C["alien_shade"])
        c.rect(25, y - dy, 27, y + 2 - dy, C["alien_shade"])
    c.ellipse(16, 17, 9, 6, C["alien"])            # abdomen
    c.ellipse(16, 10, 5, 3, C["alien_shade"])      # head
    c.rect(10, 13, 12, 15, C["red"])               # eyes
    c.rect(19, 13, 21, 15, C["red"])
    c.bevel(0.3, 0.3)
    c.outline()
    return c


def drone_fast(frame):
    c = Canvas()
    step = 1 if frame else 0
    c.rect(3, 13, 6, 18 - step, darken(C["acid"], 0.25))     # fins
    c.rect(25, 13 + step, 28, 18, darken(C["acid"], 0.25))
    c.diamond(16, 16, 11 - step, 7, C["acid"])
    c.rect(11, 12, 13, 14, C["red"])               # eyes
    c.rect(19, 12, 21, 14, C["red"])
    c.bevel(0.32, 0.3)
    c.outline()
    return c


def drone_tank():
    c = Canvas()
    for y in (11, 17, 23):                          # thick armored legs
        c.rect(3, y, 6, y + 2, C["tank_dark"])
        c.rect(25, y, 28, y + 2, C["tank_dark"])
    c.ellipse(16, 17, 11, 7, C["tank"])             # shell
    c.ellipse(16, 10, 6, 3, C["tank_dark"])         # head plate
    c.rect(9, 15, 12, 17, C["red"])                 # eyes
    c.rect(19, 15, 22, 17, C["red"])
    c.bevel(0.25, 0.28)
    c.outline()
    return c


def spawn():
    c = Canvas()
    c.disc(16, 16, 11, C["outline"])
    c.ring(16, 16, 10, C["green"], 2)
    c.disc(16, 16, 7, C["scratch"])
    c.disc(16, 16, 4, C["outline"])
    for x, y in ((16, 5), (16, 26), (5, 16), (26, 16)):
        c.rect(x - 1, y - 1, x + 1, y + 1, C["green"])
    c.outline()
    return c


def base():
    c = Canvas()
    c.rect(3, 3, 28, 28, C["gunmetal"])
    c.rect(6, 6, 25, 25, C["scratch"])
    for x in range(6, 26):
        if ((x - 6) // 3) % 2 == 0:
            c.vline(x, 6, 25, C["red"] if ((x - 6) // 6) % 2 == 0 else C["hazard"])
    c.rect(3, 3, 28, 5, C["rivet"])
    for x, y in ((5, 5), (26, 5), (5, 26), (26, 26)):
        c.set(x, y, C["outline"])
    c.bevel(0.16, 0.2)
    c.outline()
    return c


def rock():
    c = Canvas()
    c.rect(4, 6, 27, 27, C["scratch"])
    c.rect(4, 6, 27, 12, C["gunmetal"])
    c.rect(8, 14, 23, 21, C["panel"])
    c.rect(4, 24, 27, 27, C["rivet"])
    c.set(10, 9, C["outline"])
    c.set(11, 9, C["outline"])
    c.set(20, 26, C["outline"])
    c.bevel(0.2, 0.25)
    c.outline()
    return c


def rubble():
    c = Canvas()
    c.disc(10, 21, 6, C["scratch"])
    c.disc(21, 22, 6, C["gunmetal"])
    c.disc(16, 16, 5, C["scratch"])
    c.disc(6, 24, 3, C["rivet"])
    c.disc(26, 24, 3, C["rivet"])
    c.bevel(0.22, 0.25)
    c.outline()
    return c


def vent():
    c = Canvas()
    c.rect(4, 4, 27, 27, C["gunmetal"])
    c.rect(7, 7, 24, 24, C["outline"])
    for x in range(9, 24, 3):
        c.vline(x, 9, 22, C["ember"])
        c.vline(x + 1, 9, 22, C["orange"])
    c.rect(14, 13, 17, 18, C["white"])
    c.rect(4, 4, 27, 6, C["rivet"])
    c.bevel(0.15, 0.2)
    c.outline()
    return c


def decor_crack():
    c = Canvas()
    points = [(23, 5), (21, 9), (22, 12), (19, 16), (20, 20), (17, 24), (18, 28)]
    for x, y in points:
        c.set(x, y, C["scratch"])
        c.set(x, y + 1, C["scratch"])
    for x, y in ((21, 9), (19, 16), (20, 20)):
        c.set(x - 2, y, C["scratch"])
    return c


def decor_stain():
    c = Canvas()
    c.disc(13, 15, 6, darken(C["floor"], 0.22))
    c.disc(20, 19, 7, darken(C["floor"], 0.18))
    c.disc(15, 22, 5, darken(C["floor"], 0.25))
    c.dither(8, 12, 26, 26, darken(C["floor"], 0.3), phase=1)
    return c


def scorch():
    c = Canvas()
    c.disc(16, 16, 9, C["outline"])
    c.disc(11, 19, 5, C["outline"])
    c.disc(21, 19, 5, C["outline"])
    c.disc(16, 16, 5, C["scratch"])
    for x, y in ((13, 13), (19, 15), (16, 20), (10, 17), (22, 22)):
        c.set(x, y, C["ember"])
    return c


def skid():
    c = Canvas()
    for i in range(15):
        c.set(7 + i, 25 - i, C["scratch"])
        c.set(10 + i, 25 - i, C["scratch"])
        if i > 3 and i < 12:
            c.set(8 + i, 25 - i, C["scratch"])
    c.dither(5, 23, 9, 27, C["scratch"])
    return c


def debris():
    c = Canvas()
    for x, y in ((12, 14), (18, 12), (15, 18), (20, 20), (11, 21), (16, 23)):
        c.rect(x, y, x + 1, y + 1, C["scratch"])
        c.set(x, y, C["rivet"])
    c.set(22, 16, C["scratch"])
    c.set(9, 17, C["scratch"])
    return c


def hud_coin():
    c = Canvas()
    c.disc(16, 16, 13, C["gold"])
    c.ring(16, 16, 10, C["gold_dark"])
    c.disc(16, 16, 8, C["gold_dark"])
    c.disc(16, 16, 6, C["gold"])
    c.rect(11, 9, 14, 10, C["white"])
    c.dither(10, 12, 16, 14, C["white"])
    c.bevel(0.25, 0.3)
    c.outline()
    return c


def hud_wave():
    c = Canvas()
    chevron(c, 7, 16, 9, C["cyan"])
    chevron(c, 16, 16, 9, C["cyan"])
    c.outline()
    return c


def hud_space():
    c = Canvas()
    c.rect(3, 5, 28, 26, C["gunmetal"])
    c.rect(7, 9, 24, 20, C["outline"])
    c.rect(9, 11, 22, 18, C["scratch"])
    c.rect(3, 23, 28, 26, C["scratch"])
    c.bevel(0.2, 0.25)
    c.outline()
    return c


def hud_mouse(highlight_left):
    c = Canvas()
    c.rect(9, 4, 22, 27, C["gunmetal"])
    if highlight_left:
        c.rect(9, 4, 15, 13, C["blue"])
    else:
        c.rect(16, 4, 22, 13, C["blue"])
    c.vline(16, 4, 27, C["outline"])
    c.hline(9, 22, 14, C["outline"])
    c.rect(15, 7, 16, 11, C["outline"])
    c.bevel(0.2, 0.25)
    c.outline()
    return c


def explosion_0():
    c = Canvas()
    c.disc(16, 16, 13, C["orange"])
    c.disc(16, 16, 10, C["ember"])
    c.disc(16, 16, 6, C["white"])
    for y in range(32):
        for x in range(32):
            d = math.hypot(x - 15.5, y - 15.5)
            if 10 < d <= 13 and (x + y) % 2 == 0:
                c.set(x, y, C["ember"])
            elif 6 < d <= 10 and (x + y) % 2 == 0:
                c.set(x, y, C["white"])
    return c


def explosion_1():
    c = Canvas()
    c.ring(16, 16, 11, C["ember"], 2)
    for y in range(32):
        for x in range(32):
            d = math.hypot(x - 15.5, y - 15.5)
            if 9 <= d <= 13 and (x + y) % 2 == 0:
                c.set(x, y, C["orange"])
    c.disc(16, 16, 4, C["white"])
    for x, y in ((9, 9), (23, 9), (9, 23), (23, 23), (16, 4), (16, 28)):
        c.rect(x - 1, y - 1, x, y, C["white"])
    return c


def projectile():
    c = Canvas(16, 16)
    c.disc(8, 8, 6, C["cyan"])
    c.disc(8, 8, 4, C["blue"])
    c.disc(8, 8, 2, C["white"])
    c.ring(8, 8, 6, C["white"])
    return c


def muzzle():
    c = Canvas(16, 16)
    c.hline(0, 15, 8, C["ember"])
    c.vline(8, 0, 15, C["ember"])
    c.disc(8, 8, 3, C["white"])
    for x, y in ((3, 3), (12, 3), (3, 12), (12, 12)):
        c.set(x, y, C["ember"])
    return c


def impact():
    c = Canvas(16, 16)
    c.disc(8, 8, 5, C["orange"])
    c.ring(8, 8, 5, C["ember"])
    c.disc(8, 8, 2, C["white"])
    for x, y in ((2, 8), (13, 8), (8, 2), (8, 13)):
        c.set(x, y, C["ember"])
    return c


def ember():
    c = Canvas(16, 16)
    c.disc(8, 8, 4, C["white"])
    for y in range(16):
        for x in range(16):
            d = math.hypot(x - 8, y - 8)
            if 4 < d <= 6 and (x + y) % 2 == 0:
                c.set(x, y, C["white"])
    return c


SPRITES = {
    "floor_0": floor_tile(0),
    "floor_1": floor_tile(1),
    "floor_2": floor_tile(2),
    "gun_base": gun_base(),
    "gun_barrel": gun_barrel(),
    "drone_0": drone_normal(0),
    "drone_1": drone_normal(1),
    "drone_fast_0": drone_fast(0),
    "drone_fast_1": drone_fast(1),
    "drone_tank": drone_tank(),
    "spawn": spawn(),
    "base": base(),
    "projectile": projectile(),
    "muzzle": muzzle(),
    "impact": impact(),
    "explosion_0": explosion_0(),
    "explosion_1": explosion_1(),
    "hud_coin": hud_coin(),
    "hud_wave": hud_wave(),
    "hud_space": hud_space(),
    "hud_mouse_left": hud_mouse(True),
    "hud_mouse_right": hud_mouse(False),
    "rock": rock(),
    "rubble": rubble(),
    "vent": vent(),
    "decor_crack": decor_crack(),
    "decor_stain": decor_stain(),
    "scorch": scorch(),
    "skid": skid(),
    "debris": debris(),
    "ember": ember(),
}


# ---------------------------------------------------------------- output

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


def write_canvas(path, canvas):
    assert (canvas.w, canvas.h) in [(32, 32), (16, 16)], f"bad size {path}"
    raw = bytearray()
    for y in range(canvas.h):
        raw.append(0)  # filter: none
        for x in range(canvas.w):
            px = canvas.get(x, y)
            if px is None:
                raw.extend((0, 0, 0, 0))
            else:
                raw.extend((*px, 255))
    write_png_raw(path, canvas.w, canvas.h, raw)


# Vignette: near-black blue alpha ramp towards the corners (screen overlay).
VIGNETTE_W, VIGNETTE_H = 640, 360
VIGNETTE_INNER = 0.45  # normalized distance where darkening starts
VIGNETTE_OUTER = 1.15  # normalized distance at full opacity
VIGNETTE_ALPHA = 0.55  # max edge opacity


def vignette_raw(w, h):
    cx, cy = w * 0.5, h * 0.5
    base = C["outline"]
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
    for name, canvas in SPRITES.items():
        write_canvas(os.path.join(OUT, name + ".png"), canvas)
    raw = vignette_raw(VIGNETTE_W, VIGNETTE_H)
    write_png_raw(os.path.join(OUT, "vignette.png"), VIGNETTE_W, VIGNETTE_H, raw)
    print(f"wrote {len(SPRITES)} sprites + vignette to {OUT}")


if __name__ == "__main__":
    main()
