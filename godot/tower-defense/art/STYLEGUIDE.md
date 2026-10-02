# Art Styleguide — Xeno Tactic (XT-Pass)

## Richtung

Kühl, technisch, Labor-Containment. Dunkle blaugraue Stahl-Kacheln mit
Rasterlinien, blaue Spieler-Türme, weiß-grüne Alien-Drohnen (rotes Auge),
warme Pixel-Effekte als Kontrast. Gold/Cyan nur für UI und Gameplay-Signale.
Lesbarkeit > Deko. Der XT-Pass (T11, 2026-10-02) hat den früheren
Ember-Foundry-Look ersetzt.

## Palette

| Slot             | Hex       | Nutzung                          |
|------------------|-----------|----------------------------------|
| Outline          | `#0b1016` | 1px Kontur um alle Sprites       |
| Floor Base       | `#1b222c` | Boden Standard (dunkler Stahl)   |
| Floor Alt        | `#212934` | Boden Variante / Kanten          |
| Rivet / Steel    | `#4a5666` | Nieten, heller Stahl             |
| Scratch          | `#2a3340` | Kratzer, dunkle Panels           |
| Panel Blue       | `#2f3b4a` | Panel-Flächen (Props)            |
| Player Blue      | `#5a8fc8` | Türme (hell)                     |
| Player Shade     | `#335e8c` | Türme (Unterseite)               |
| Gunmetal         | `#7a8696` | Läufe, Rahmen, HUD-Icons         |
| Cyan Highlight   | `#9fd8ff` | Mündung, UI-Akzente, Projektile  |
| Fire Orange      | `#ff8a2e` | Mündungsfeuer, Explosionen       |
| Ember            | `#ffb44a` | Glut/Funken (Texte getintet)     |
| Alien White      | `#d8dee6` | Standard-Drohne                  |
| Alien Shade      | `#9aa4b0` | Drohnen-Unterseite               |
| Acid Green       | `#8ee04a` | Schnelle Drohne (Pfeilform!)     |
| Tank Gray-Blue   | `#5a6672` | Tank-Drohne (breite Form!)       |
| Tank Shade       | `#3a4450` | Tank-Unterseite                  |
| Eye Red / Hazard | `#ff3b30` | Rotes Auge — alle Gegner; Hazard |
| Breach Green     | `#54e08a` | Spawn-Portal-Ring                |
| White Hot        | `#ffffff` | Effekt-Kerne                     |
| Gold             | `#ffd75e` | Geld, Coins                      |
| Gold Shade       | `#b88a2e` | Gold-Schatten                    |
| Hazard White     | `#e8e4da` | Hazard-Streifen                  |

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
- Spawn vs. Basis: dunkles Breach-Portal mit grünem Ring (`spawn`) gegen
  Stahl-Containment-Tür mit Hazard-Streifen (`base`) — Form UND Farbe
  unterscheiden sich; beide pulsieren im Code (Scale), damit sie nie
  verwechselt werden. T08: markieren jetzt jede Entry- bzw. Exit-Zelle der
  jeweiligen Seite.
- HUD-Icons 16×16, 2× skaliert: `hud_coin` (Geld), `hud_wave` (Chevron),
  `hud_space` (Leertaste), `hud_mouse_left`/`hud_mouse_right` (Spieler-Blau);
  Gun-Icon nutzt `gun_base`.
- UI-Font: VT323 (`fonts/VT323-Regular.ttf`, OFL — Lizenztext daneben),
  projektweit via `gui/theme/custom_font`; HUD-Panels dunkelblau mit
  Stahl-Rahmen, Geld gold, Titel/Buttons cyan.
- Effekte: `impact` (8×8 Trefferblitz, Pop von 0.7×), `muzzle` (8×8
  Mündungsblitz, 0.05 s + Skalierungs-Pop), `explosion_0/1` (16×16 Kill).
- Terrain (T09): Blocker als solide Fels-/Geröll-/Vent-Cluster (geseedet,
  nie Entry-verstopfend), Deko + Decals als Grime-Schicht (z −1); Vents
  glühen (C/H), alle Partikel nutzen `ember` (8×8, getintet).
- Animation minimal: 2-Frame-Bob (Drohnen) + Code-Waddle/Flip (normal/tank)
  bzw. Rotation entlang des Pfads (fast), Flash-Frames (Muzzle/Impact/
  Explosion), HP-Balken als 1px-Textur-Sprites (kein Asset).

## UI/HUD (T12)

- Panels: dunkler Stahl `#141b24` (~94 % Deckkraft), 2 px Rahmen `#4a5666`,
  Hover-Rahmen Cyan `#6fb4d9`; 2 px Ecken, weicher Schatten — Vorbild ist der
  XT-HELP-Screen (dunkle Panels + Cyan-Linien).
- Schrift: VT323, ALL CAPS; Gold `#ffd75e` (Geld), Cyan `#9fd8ff`
  (Titel/Buttons), Text `#d8dee6`, gedimmt `#808991`.
- Wave-Bar: 24 Segmente, Acid-Grün `#8ee04a` auf Slot `#1b2530`, 2 px Lücke.
- Modifier-Chip färbt Text + Rahmen je Event (Ansturm orange, Schwarm grün,
  Blackout cyan, Kopfgeld gold).
- HUD-Icons: `hud_coin`, `gun_base`, `hud_space`, `hud_mouse_*`, `hud_wave`
  (16×16, 2× im HUD) — beim 32×32-Pass (T13) mitziehen.

## Referenz-Anker (Xeno Tactic)

Ursprungs-Inspiration (Flash-Maze-TD, Labor-Containment-Setting).

> **XT-Pass umgesetzt (T11, 2026-10-02):** Palette, Sprites, UI-Font und
> Sounds folgen dem XT-Zielbild; die Tabelle unten bleibt als verifizierte
> Referenz (Original-Studie, gitignored in `reference/xt/`).

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
| `gun_base` | 16×16 | Turm-Sockel: Stahl-Rahmen, blaue Kuppel, Cyan-Kern (zugleich HUD-Icon „Gun") |
| `gun_barrel` | 16×16 | Stahl-Lauf mit Cyan-Spitze, zeigt nach oben, Drehpunkt (8,12) |
| `drone_0/1` | 16×16 | Standard-Drohne: weißer Alien-Bug, rote Augen, 2-Frame-Bob |
| `drone_fast_0/1` | 16×16 | Schnelle Drohne (Acid-Grün, Pfeilform), rotiert entlang des Pfads |
| `drone_tank` | 16×16 | Tank-Drohne (grau-blau, breit), ein Frame |
| `spawn` | 16×16 | Breach-Portal mit grünem Ring (pulsiert, jede Entry-Zelle) |
| `base` | 16×16 | Containment-Tür mit Hazard-Streifen (pulsiert, jede Exit-Zelle) |
| `projectile` | 8×8 | Tracer, rotiert zur Flugrichtung |
| `muzzle` | 8×8 | Mündungsblitz (0.05 s, Pop + ±Rotation) |
| `impact` | 8×8 | Trefferblitz (0.12 s, Pop von 0.7×) |
| `explosion_0/1` | 16×16 | Kill-Explosion, 2 Frames alternierend |
| `vignette` | 640×360 | Randabdunkelung (Alpha-Gradient, linear gefiltert, HUD-Overlay) |
| `hud_coin` | 16×16 | HUD: Geld |
| `hud_wave` | 16×16 | HUD: Welle (Doppel-Chevron) |
| `hud_space` | 16×16 | HUD: Leertaste |
| `hud_mouse_left/right` | 16×16 | HUD: linke/rechte Maustaste (Spieler-Blau) |
| `rock` | 16×16 | Terrain-Blocker: Fels (solid, nie bebaubar) |
| `rubble` | 16×16 | Terrain-Blocker: Geröll |
| `vent` | 16×16 | Terrain-Blocker: Glut-Vent (glüht, Partikel-Emitter) |
| `decor_crack` | 16×16 | Deko: Riss (kosmetisch, z −1) |
| `decor_stain` | 16×16 | Deko: Fleck (kosmetisch, z −1) |
| `scorch` | 16×16 | Decal: Brandfleck (Kill, z −1) |
| `skid` | 16×16 | Decal: Schleifspur (Leak, z −1) |
| `debris` | 16×16 | Decal: Trümmer (Treffer, z −1) |
| `ember` | 8×8 | Partikel-Textur (Glut/Funke/Rauch, getintet) |

## Handoff (deine Skizzen → Projekt)

- PNG, 16×16-Vielfache, transparent, gleiche Dateinamen in `art/` ersetzen —
  kein Code-Umbau nötig (`gun_base.png`, `drone_0.png`, …).
- Neue Sprites: Namen + Größe hier eintragen, dann im Code referenzieren.
- Platzhalter-Generator: `tools/make_placeholders.py` (stdlib-only).
  Art als ASCII direkt im Skript editierbar — `python3 make_placeholders.py`.
