# Art Styleguide — Ember Foundry

## Richtung

Warm, industriell, dunkel. Schmiede-/Foundry-Look: fast schwarzer, warmer
Grund, orangefarbene Guns, grüne Drohnen, glühende Akzente nur für
Gameplay-relevantes (Mündung, Kern, Spawn). Lesbarkeit > Deko.

## Palette

| Slot            | Hex       | Nutzung                          |
|-----------------|-----------|----------------------------------|
| Outline         | `#241610` | 1px Kontur um alle Sprites       |
| Floor Base      | `#1d1613` | Boden Standard                   |
| Floor Alt       | `#221a15` | Boden Variante / Kanten          |
| Rivet           | `#4a3a2c` | Nieten, Details                  |
| Scratch         | `#2e231c` | Kratzer, Patches                 |
| Gun Body        | `#ffa02e` | Gun-Korpus (blockig)             |
| Gun Shade       | `#b35c14` | Gun Unterseite                   |
| Highlight       | `#ffd166` | Kantenlicht, Kills, Coins        |
| Gunmetal        | `#4a4038` | Lauf, Rahmen, Spawn-Innen        |
| Hot Tip         | `#ff5a2e` | Laufspitze, Explosion            |
| Drone Green     | `#7ddf64` | Standard-Drohne                  |
| Drone Shade     | `#3f9142` | Drohnen-Unterseite               |
| Fast Lime       | `#d7ff5e` | Schnelle Drohne (spitze Form!)   |
| Tank Teal       | `#2e9e6b` | Tank-Drohne (breite Form!)       |
| Enemy Eye       | `#ff3b30` | Rotes Auge — alle Gegner         |
| Spawn Glow      | `#35d07f` | Spawn-Portal Ring                |
| Base Core       | `#ff7847` | Basis-Kern (Heimat, pulsiert)    |
| Projectile      | `#ffe08a` | Tracer, Muzzle-Flash             |

Form + Farbe codieren gemeinsam: rund = Standard, pfeilförmig = schnell,
breit = Tank. Nie nur über Farbe unterscheiden.

## Specs

- Basis 16×16 px (Effekte/Projektile 8×8), transparentes PNG, 1px Outline.
- Ingame 2× skaliert (TILE 32). `Nearest`-Filter ist im Projekt gesetzt.
- Pivot: Center; Ausnahme `gun_barrel` zeigt nach oben, Drehpunkt ≈ (8,12)
  via `Sprite2D.offset` (kommt mit T03 Kampf).
- Boden texturiert: `floor_0/1/2` rotieren im Schachbrett + Nieten/Kratzer,
  keine glatten Flächen. Vignette später als Overlay (T06).
- Animation minimal: 2-Frame-Bob (Drohnen), Flash-Frames (Muzzle/Explosion).

## Handoff (deine Skizzen → Projekt)

- PNG, 16×16-Vielfache, transparent, gleiche Dateinamen in `art/` ersetzen —
  kein Code-Umbau nötig (`gun_base.png`, `drone_0.png`, …).
- Neue Sprites: Namen + Größe hier eintragen, dann im Code referenzieren.
- Platzhalter-Generator: `tools/make_placeholders.py` (stdlib-only).
  Art als ASCII direkt im Skript editierbar — `python3 make_placeholders.py`.
