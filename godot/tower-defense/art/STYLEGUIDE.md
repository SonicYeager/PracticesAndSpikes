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
- Pivot: Center; Ausnahme `gun_barrel` zeigt nach oben, Drehpunkt (8,12)
  via `Sprite2D.offset` (T03 umgesetzt).
- Boden texturiert: `floor_0/1/2` rotieren im Schachbrett + Nieten/Kratzer,
  keine glatten Flächen. Vignette als Overlay umgesetzt (T06, `vignette.png`,
  640×360 Alpha-Gradient, linear gefiltert).
- Spawn vs. Basis: runder grüner Portal-Ring (`spawn`) gegen eckigen Bunker
  mit Ember-Reaktor (`base`) — Form UND Farbe unterscheiden sich; beide
  pulsieren im Code (Scale), damit sie nie verwechselt werden. T08: markieren
  jetzt jede Entry- bzw. Exit-Zelle der jeweiligen Seite.
- HUD-Icons 16×16, 2× skaliert: `hud_coin` (Geld), `hud_wave` (Chevron),
  `hud_space` (Leertaste), `hud_mouse_left`/`hud_mouse_right` (jeweils die
  aktive Taste orange); Gun-Icon nutzt `gun_base`.
- Effekte: `impact` (8×8 Trefferblitz, Pop von 0.7×), `muzzle` (8×8
  Mündungsblitz, 0.05 s + Skalierungs-Pop), `explosion_0/1` (16×16 Kill).
- Terrain (T09): Blocker als solide Fels-/Geröll-/Vent-Cluster (geseedet,
  nie Entry-verstopfend), Deko + Decals als Grime-Schicht (z −1); Vents
  glühen (C/H), alle Partikel nutzen `ember` (8×8, getintet).
- Animation minimal: 2-Frame-Bob (Drohnen) + Code-Waddle/Flip (normal/tank)
  bzw. Rotation entlang des Pfads (fast), Flash-Frames (Muzzle/Impact/
  Explosion), HP-Balken als 1px-Textur-Sprites (kein Asset).

## Referenz-Anker (Xeno Tactic)

Ursprungs-Inspiration (Flash-Maze-TD, Labor-Containment-Setting).

> **Kalibrierung 2026-10-02:** Audio und Visuelles **so nah wie möglich** an
> Xeno Tactic; Fiktion/UI-Text nur grob ankern, iterativ nach Gefühl. Der
> Look unten ist **Zielbild** — der Ember-Foundry-Stand wird im **XT-Pass
> (T11)** ersetzt. Bis dahin beschreiben die Abschnitte darunter den Ist-Stand.

### Zielbild Xeno Tactic (verifiziert am Original, 2026-10-02)

Referenzmaterial: Original-SWF (`xenotactic.swf`, XGen) per FFDec extrahiert —
**nur Studienmaterial, nichts davon wird übernommen oder committet**.
Quelle: Titel-/Help-Screen, Kachel-/Effekt-Sprites, eingebettete Texte.

| Element | Ziel |
|---|---|
| Titel/Menü | Neon-Cyan-Schrift auf dunkelblauem Stahl-Textur-Hintergrund, blaue Turret-Render-Optik |
| Boden | Dunkle blaugraue Metall-Kacheln mit Rasterlinien; Varianten: dunkelrot; Hazard-Streifen rot/weiß |
| Gegner | Weiß-graue Alien-Bugs (dunkle Outline), grüne Variante; Swarm-Optik, Form+Farbe kodiert |
| Türme | Graue Metall-Silhouetten mit farbigem Kopf; Titel zeigt eine blaue Render-Turret als Vorbild |
| UI | Dunkelblaue Metall-Panels; goldene Pixel-Schrift (ALL CAPS); Icon-Grid; segmentierte grüne Upgrade-Bars; GOLD-Zähler + Coin; „SEND NEXT WAVE“; grüner GO-Pfeil-Cursor; Cyan-Highlights |
| Effekte | Pixelige Feuerbälle (orange/gelb), Ringe, Säure-/Blut-Splats (grün/rot) |
| Palette (Vorschlag) | Steel `#2e3742`, Steel hell `#3d4856`, Grid `#161b21`, Dark-Red `#3a1f1f`, Alien-White `#d7dde3`, Acid `#86c34a`, Health `#6fdc4f`, Gold `#ffd75e`, Neon `#aef6ff`, Hazard `#c8433a` |
| Sound | Kurze, trockene Blips/Alarme, kein Score — siehe `docs/AUDIO.md` |

- Lesbarkeit vor Effekten: der XT-Look war schlicht, aber sofort erfassbar.
- Kein Asset-Kopieren: Stil nachahmen, Assets selbst generieren. XT nennt als
  Vorbild „Desktop Tower Defense by Paul Preece“ — ein Hommage-Kredit ist fair.
- Referenz-Ablage (falls behalten): `reference/xt/` (gitignored), nie im Build.

## Sprite-Inventar

| Datei | Größe | Verwendung |
|---|---|---|
| `floor_0/1/2` | 16×16 | Bodenkacheln, deterministisches Muster `(x*7+y*13) % 3` |
| `gun_base` | 16×16 | Turm-Sockel (zugleich HUD-Icon „Gun") |
| `gun_barrel` | 16×16 | Turm-Lauf, zeigt nach oben, Drehpunkt (8,12) |
| `drone_0/1` | 16×16 | Standard-Drohne, 2-Frame-Bob |
| `drone_fast_0/1` | 16×16 | Schnelle Drohne (Pfeilform), rotiert entlang des Pfads |
| `drone_tank` | 16×16 | Tank-Drohne (breit), ein Frame |
| `spawn` | 16×16 | Spawn-Portal (pulsiert, jede Entry-Zelle) |
| `base` | 16×16 | Basis-Bunker mit Ember-Reaktor (pulsiert, jede Exit-Zelle) |
| `projectile` | 8×8 | Tracer, rotiert zur Flugrichtung |
| `muzzle` | 8×8 | Mündungsblitz (0.05 s, Pop + ±Rotation) |
| `impact` | 8×8 | Trefferblitz (0.12 s, Pop von 0.7×) |
| `explosion_0/1` | 16×16 | Kill-Explosion, 2 Frames alternierend |
| `vignette` | 640×360 | Randabdunkelung (Alpha-Gradient, linear gefiltert, HUD-Overlay) |
| `hud_coin` | 16×16 | HUD: Geld |
| `hud_wave` | 16×16 | HUD: Welle (Doppel-Chevron) |
| `hud_space` | 16×16 | HUD: Leertaste |
| `hud_mouse_left/right` | 16×16 | HUD: linke/rechte Maustaste (aktive Taste orange) |
| `rock` | 16×16 | Terrain-Blocker: Fels (solid, nie bebaubar) |
| `rubble` | 16×16 | Terrain-Blocker: Geröll |
| `vent` | 16×16 | Terrain-Blocker: Ember-Vent (glüht, Partikel-Emitter) |
| `decor_crack` | 16×16 | Deko: Riss (kosmetisch, z −1) |
| `decor_stain` | 16×16 | Deko: Fleck (kosmetisch, z −1) |
| `scorch` | 16×16 | Decal: Brandfleck (Kill, z −1) |
| `skid` | 16×16 | Decal: Schleifspur (Leak, z −1) |
| `debris` | 16×16 | Decal: Trümmer (Treffer, z −1) |
| `ember` | 8×8 | Partikel-Textur (Ember/Funke/Rauch, getintet) |

## Handoff (deine Skizzen → Projekt)

- PNG, 16×16-Vielfache, transparent, gleiche Dateinamen in `art/` ersetzen —
  kein Code-Umbau nötig (`gun_base.png`, `drone_0.png`, …).
- Neue Sprites: Namen + Größe hier eintragen, dann im Code referenzieren.
- Platzhalter-Generator: `tools/make_placeholders.py` (stdlib-only).
  Art als ASCII direkt im Skript editierbar — `python3 make_placeholders.py`.
