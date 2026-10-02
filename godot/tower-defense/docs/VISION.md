# Vision — Tower Defense

*Workshop 2026-10-02 (3 Runden). Nordstern für jede Slice-Entscheidung —
vor Frameworks und Best Practices.*

**In einem Satz:** Ein rußiges, lebendiges Diorama, in dem du als Ingenieur ein
Maze tüftelst, ihm beim Arbeiten zusiehst — und wenn es bricht, wechselst du
die Seiten.

## Erfahrungsziel

Ruhige Neugier mit Puls: Man baut, probiert, schaut zu und will wissen, was als
Nächstes passiert. Kein Meistern um jeden Preis, keine Hektik — aber immer ein
leichter Druck, damit das Diorama lebendig bleibt.

## Pillars (das Was — jede Idee muss eines stützen)

### P1 — „Das Maze ist die Werkbank"

Bauen ist Experimentieren: Jede Mauer beantwortet eine Frage („Was, wenn ich
hier zumache?"). Umlenken, Routen-Tricks, Juggling, Optimieren.

- Konkret: freies Mazing, Reroute, geseedete Maps, eigene Tricks entdecken.
- Schließt aus: reine DPS-Checks, Lock&Key-Gegner, „richtige" Lösungen.

### P2 — „Die Schmiede lebt"

Atmosphäre ist ein Ziel, nicht Dekoration: dreckig, glühend, hörbar; der
Board-Zustand erzählt den Run (Decals, Vents, Rauch).

- Konkret: Ember Foundry, Partikel, Decals, Sound; Zustand akkumuliert sichtbar.
- Schließt aus: Atmosphäre auf Kosten der Lesbarkeit — bei Konflikt gewinnt die
  Lesbarkeit.

### P3 — „Neugier als Motor"

Man spielt, um zu sehen, was passiert: neue Gegner-Verhalten, Ereignisse und
Modifier, Überraschungen pro Run. Ruhig mit Puls, nie Reflexdruck.

- Konkret: Discovery-Fokus auf Gegner + Events + Map-Zufall.
- Schließt aus: Twitch-Anforderungen, Grind, Content-Treadmill.

## Values (das Wie)

- **Nichts passiert ohne Grund**: deterministisch, erklärbar, Seed im Log —
  Überraschung ja, Willkür nein.
- **Verlieren verwandelt sich**: Scheitern ist Material, nicht Strafe
  (Rollenwechsel-Twist, siehe unten).
- **Kein Grind, keine Hektik**: Systemtiefe statt Dauerbeschäftigung.

## Anti-Pillars (was das Spiel NICHT ist)

- Keine Helden-/Einheiten-Steuerung — ich baue und schaue zu.
- Kein Content-Treadmill — Tiefe vor Breite.
- Keine Story-Kampagne.
- Kein Twitch-/Reflexspiel.

## Kalibrierungen (Workshop-Ergebnis)

- **Intensität**: zwischen „ruhig, Action läuft nebenher" und „leichter
  konstanter Druck".
- **Referenzgefühl**: Bau-/Fabrik-Spiele (Tüfteln), Emergenz/Sandbox
  (Überraschung), klassisches TD (Wellen als Bühne).
- **Run-Ende**: Game over bleibt (Geld < 0) — aber Scheitern wird zum
  **Rollenwechsel**: du wirst Angreifer, die KI verteidigt. Einstieg als
  **Epilog-Prototyp**: Drone-Budget + eine Rache-Welle nach Game over.
  Kill-Kriterium: Zieht die Symmetrie? Fühlt sich der Epilog wie ein neuer
  Blick an — oder wie ein Fremdkörper?
- **Meta-Progression**: zwischen „Vielfalt freischalten" (Map-Typen, Gegner-
  Verhalten, Event-Pools) und „echter, leichter Progression" — noch zu
  kalibrieren; kein Grind.

## Nächste Prototyp-Fragen (Priorität)

1. **Epilog-Twist** (P1/P3): Rache-Welle nach Game over — zieht die Symmetrie?
2. **Druckkurve** (P3): Zwischenzustände statt binär — Senken und
   Ereignisse/Modifier als Puls. *Erste Antwort in T10 (Modifier +
   Overcharge); Tuning via Telemetrie offen.*
3. **Lesbarkeit** (P2): Decal-Verschmierung entschärfen. *T10: Per-Cell-Cap;
   der ursprüngliche Befund war teils ein Schnellspul-Harness-Artefakt.*
4. **Meta-Kalibrierung**: was genau zwischen Vielfalt und Progression?
5. **Total Time Control** (P3): Pause/Speed als „ruhig mit Puls"-Werkzeug?

## Anwendung

- Jede Slice-Entscheidung zuerst gegen P1–P3 prüfen; kein Pillar → verwerfen
  oder in `docs/IDEAS.md` parken.
- Frameworks und Praktiken: `docs/GAMEDEV.md` (Workspace).
