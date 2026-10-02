# Vision — Trench Warfare (Arbeitstitel)

*Workshop-Runde 1, 2026-10-02 (Chat-Input, kein Präsenz-Workshop). Grundlage:
„erstmal nah am Original, mehr Sandbox: Freiheit für kreative Strategien und
wilde Ideen- und Einheiten-Kombos".*

**In einem Satz:** Ein WW1-Frontabschnitt als lebendes Uhrwerk — du setzt
Verstärkungen ein, die von selbst kämpfen, und baust dir zugleich ein
Schlachtfeld-Labor, in dem ungewöhnliche Kombinationen ausdrücklich erwünscht
sind.

## Thema & Ton

Stilisiert, respektvoll im Kern: Pixel-/Vektor-Optik, klar als Spiel erkennbar.
Der Krieg wird nicht verharmlost, aber auch kein Grauen-Simulator — Gewalt ist
Teil des Systems, nicht das Spektakel. (Haltung des Originals, hier bewusster
als Design-Leitlinie.)

## Erfahrungsziel

Taktisches Beobachten mit Experimentierdrang: Man liest die Front, wirft
Verstärkungen und Feuerunterstützung ins Rennen — und will danach sofort
wissen, wie sich eine andere Kombination oder ein anderer Regler anfühlt.

## Spielmodi

- **Sandbox/Custom (Standard)**: Solo gegen KI oder lokaler Hotseat (zwei
  Spieler am selben Rechner); Regler statt Regeln.
- **Kampagne (optional, später)**: verpackt Sandbox-Setups als Missionen —
  kein Pflichtprogramm, keine Grind-Progression.
- Offen: **Hotseat im Echtzeit-Gefecht** braucht ein eigenes Format (geteilte
  Deploy-Leisten, Pause-Wechsel oder asynchron) — siehe offene Fragen.

## Schauplatz & Session

Ein Bildschirm, alle Grabenlinien sichtbar; ein Gefecht dauert ca. 5–15
Minuten. Größere Karten/Scroll sind eine spätere Option, kein MVP-Ziel.

## Pillars (das Was — jede Idee muss eines stützen)

### P1 — „Die Front läuft von selbst"

Du bist Kommandeur, nicht Puppenspieler: Einheiten rücken vor, suchen Deckung
und kämpfen autonom; deine Entscheidungen sind Zusammensetzung, Timing und
Feuerunterstützung. Das Original-Gefühl (Creep-Game, Gräben, Moral) ist die
Basis.

- Konkret: Squads statt Einzelsoldaten, autonomer Vorstoß, Halten im Graben.
- Schließt aus: Einheiten-Mikromanagement, Klick-Challenges, Reflexdruck.

### P2 — „Sandbox-Werkbank"

Ausprobieren ist kein Cheat-Modus, sondern ein First-Class-Ziel: Einheiten
datengetrieben, Custom-Battle-Setup mit Reglern, Einheitenpools frei
kombinierbar — auch „wilde" Kombos, die das Original so nie erlaubt hätte.

- Konkret: datengetriebene Unit-Definitionen, Custom-Setup (Cooldowns,
  Startmoral, Pools, Support-Loadout), Szenarien speichern.
- Erste Regler-Priorität (Workshop): Moral-Parameter, Cooldowns-/Wirtschafts-
  tempo, Feuerunterstützung & Loadout.
- Schließt aus: harte Fraktions-Locks als Standard, Grind-Freischaltungen als
  Zugang zu Inhalten, „die eine richtige" Meta-Spielweise.

### P3 — „Lesbare Eskalation"

Der Zustand der Schlacht ist auf einen Blick erfassbar (Frontlinie, Moral,
Stärkeverhältnis); Spannung entsteht durch Eskalation und Moral-Kipppunkte,
nicht durch Hektik oder Zahlen-Raten.

- Konkret: Total Information (Stats/Timeline einsehbar), Pause/Speed als
  Sandbox-Werkzeug, klare Counter-Kommunikation.
- Schließt aus: Twitch-Anforderungen, Off-Screen-Zufall, versteckte Boni.

## Values (das Wie)

- **Nichts passiert ohne Grund**: deterministische Simulation, Seed im Log,
  Replays möglich — Überraschung ja, Willkür nein.
- **Scheitern ist Material**: Telemetrie + After-Action-Report statt
  Bestrafung; aus Niederlagen lernt das Design.
- **Tiefe vor Breite**: Systeme (Interaktionen, Regler) statt Content-Treadmill.

## Anti-Pillars (was das Spiel nicht ist)

- Kein Mikro-RTS — ich setze ein, ich steuere nicht jeden Schuss.
- Kein Kampagnen-Pflichtprogramm — Sandbox zuerst; die Kampagne ist ein
  optionaler Rahmen, kein Gate.
- Keine Assets, Namen oder Sounds aus *Warfare 1917* — eigene Platzhalter,
  eigene Identität.
- Kein Twitch- und kein Grind-Spiel.

## Kalibrierungen

- **Nähe zum Original**: Start bewusst nah dran (WW1, Creep-Kern, Gräben,
  Moral, Feuerunterstützung); die Sandbox ist der eigene Twist.
- **Sandbox-Priorität**: Moral, Tempo/Cooldowns, Support & Loadout zuerst;
  Pool-Mix, Stats-Editor und Karten-Editor geparkt (`docs/IDEAS.md`).
- **Wildheit**: Kombos dürfen broken sein — solange der Sandbox-Modus sie
  sichtbar und verhandelbar macht. Balance-Ziel ist Vielfalt (viele spielbare
  Pläne), nicht Gleichheit.
- **Meta**: keine Progression nötig; wenn, dann Vielfalt freischalten (Karten,
  Regler, Szenarien) statt Machtstufen.
- **Steuerung**: Deployen + Support-Ziele + Pause/Speed; keine Befehle pro
  Einheit (auch nicht als Pflicht).

## Nächste Prototyp-Fragen (Priorität)

1. **Trägt das Zuschauen?** Bleibt das autonome Gefecht spannend, wenn
   Entscheidungen selten sind? (Kandidat: S01)
2. **Moral-Modell**: Seiten- vs. Einheiten-Moral — zentral, weil
   Moral-Parameter der Top-Sandbox-Regler sind. (Kandidat: S03)
3. **Hotseat-Format**: Wie spielt sich Echtzeit zu zweit an einem Rechner —
   geteilte Deploy-Leisten, Pause-Wechsel oder asynchron?
4. **Regler-Form**: Presets, Slider oder Datei — wie werden Moral/Tempo/Support
   verhandelbar?
5. **Szenario-Persistenz**: Save/Load von Presets — Teilen ist nicht
   priorisiert, reproduzierbare Setups schon.

## Anwendung

- Jede Slice-Entscheidung zuerst gegen P1–P3 prüfen; kein Pillar → verwerfen
  oder in `docs/IDEAS.md` parken.
- Frameworks und Praktiken: `docs/GAMEDEV.md` (Workspace); Mechanik-Analyse
  und Roadmap: `docs/DESIGN.md`.
