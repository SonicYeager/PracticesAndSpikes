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
  (Überraschung), klassisches TD (Wellen als Bühne) — konkreter Anker:
  **Xeno Tactic** (Maze + Labor-Containment, siehe unten).
- **Run-Ende**: Game over bleibt (Geld < 0) — aber Scheitern wird zum
  **Rollenwechsel**: du wirst Angreifer, die KI verteidigt. Einstieg als
  **Epilog-Prototyp**: Drone-Budget + eine Rache-Welle nach Game over.
  Kill-Kriterium: Zieht die Symmetrie? Fühlt sich der Epilog wie ein neuer
  Blick an — oder wie ein Fremdkörper?
- **Meta-Progression**: entschieden als **leichte Unlocks** (Map-Typen,
  Gegner-Verhalten, Event-Pools) — Entscheidung 2026-10-02, siehe README
  „From prototype to game"; kein Grind. Als spätere Ausbaustufe denkbar:
  kosmetischer Discovery-Codex (P3).

## Nächste Prototyp-Fragen (Priorität)

1. **Epilog-Twist** (P1/P3): Rache-Welle nach Game over — zieht die Symmetrie?
2. **Druckkurve** (P3): Zwischenzustände statt binär — Senken und
   Ereignisse/Modifier als Puls. *Erste Antwort in T10 (Modifier +
   Overcharge); Tuning via Telemetrie offen.*
3. **Lesbarkeit** (P2): Decal-Verschmierung entschärfen. *T10: Per-Cell-Cap;
   der ursprüngliche Befund war teils ein Schnellspul-Harness-Artefakt.*
4. **Meta-Kalibrierung**: was genau zwischen Vielfalt und Progression?
5. **Total Time Control** (P3): Pause/Speed als „ruhig mit Puls"-Werkzeug?
6. **Loop-Erhalt nach dem Zubauen** (P1/P3, Playtest 2026-10-02): Ist das Maze
   optimal gebaut, endet die Entscheidung — das Diorama wird Zuschauen (Run des
   Maintainers: 73 Türme, Wellen 1–27 ohne Leak, danach nur Druck ohne
   Entscheidung). Welche **wiederkehrende** Entscheidung hält den Loop, ohne
   Content-Tretmühle? Kandidaten: wellen-getriebene Terrain-Disruptionen
   (Krater/Felsen/neue Front, mit Refund + Telegraphing), Commander-Fähigkeiten,
   Turm-Upgrades. Kill (amendiert 2026-10-03, ADR 0012): drei Wellen nach dem
   Bauplateau ohne Upgrade-Ausgabe bei steigendem Geld → Ansatz verwerfen;
   Evidenz über Human-Runs, Harness sobald verfügbar.

**Reihenfolge (Entscheidung 2026-10-02)**: Der Run-Frame (endliche Mission +
Time Control, M1) kommt vor dem Epilog-Twist (M3) — Twists brauchen erst ein
Ende, das sich lohnt. **Run-Frame geliefert 2026-10-03** (T18: Ziel-Welle 20,
Sieg/Niederlage + optionale Endlos-Verlängerung, `mission_cleared`); Time
Control bleibt offen. Verortung: README „From prototype to game". *Nachtrag
(Playtest 2026-10-02): Der Loop-Erhalt (Frage 6) ist die wichtigste offene
Richtungsentscheidung nach M1; Kandidaten in `docs/IDEAS.md`, Analyse unter
`context/tasks/tower-defense-map-boredom/`. *Entschieden 2026-10-03: Upgrades
zuerst (ADR 0012, T16); Disruption/Abilities bleiben Kandidaten.*

## Anwendung

- Jede Slice-Entscheidung zuerst gegen P1–P3 prüfen; kein Pillar → verwerfen
  oder in `docs/IDEAS.md` parken.
- Frameworks und Praktiken: `docs/GAMEDEV.md` (Workspace).

## Referenz: Xeno Tactic (Flash, 2007; XT2 2010)

*Vom Maintainer als Ursprungs-Inspiration benannt (2026-10-02): „Das Theme,
Setting und Gameplay davon hatte mir damals sehr gefallen." Wir empfinden es
in gewisser Weise nach und verfeinern es mit dieser Vision.*

**Was es ist:** Grid-basiertes Maze-Tower-Defense (inspiriert von Desktop
Tower Defense): Türme blockieren die Wege, Aliens laufen von den Spawns zum
Ausgang, man baut und upgradet Türme, Durchbrüche kosten Leben. Setting:
Forschungslabor — die Xenos versuchen auszubrechen; man richtet automatische
Verteidigung ein, um sie an der Flucht zur Oberfläche zu hindern. Missionen
mit 20–100 Wellen, fünf Schwierigkeitsgrade, Tower-Upgrades; Ground-Türme
(Vulcan/Missile/Plasma), Slow (Freeze), Flächen-Paralyse (Sonic), Anti-Air
(DCA/ThW); Splitters (zerfallen beim Tod), Flieger-Bosse.

**Was wir übernehmen (Anker):**

- **Fiktion „Eindämmung"**: Die Gegner wollen *entkommen* — unsere Exits
  sind genau das: Fluchtwege. „Wir halten sie drin."
- **Maze als Kern**: freies Bauen, Wege formen, das Feld umdrehen (Juggling)
  — P1 „Werkbank" ist im Kern Xeno Tactic.
- **Lesbares Grid**: top-down, klare Kacheln, Silhouetten statt Effekte.
- **Rollen statt Menge bei Türmen**: Wall/Ground/Slow — wenige, klare Rollen.
- **Langstrecken-Gefühl**: Runs dürfen groß werden (Endurance) mit einer sich
  aufbauenden Bedrohung.

**Was wir bewusst verfeinern/ändern:**

- **Kein Anti-Air-Lock&Key**: Ein-Lösungs-Gegner bleiben verboten — wenn je
  Flieger kommen, dann mit mindestens zwei Kontern. (Die Code-Analyse zeigt:
  XT war breiter als erinnert — 4 von 6 Towern trafen Luft; die Regel bleibt.)
- **Kein No-Save-Marathon**: XT-Läufe über 100 Wellen ohne Speichern waren
  zäh — wir setzen auf Sessions, Determinismus und Telemetrie.
- **Ton**: XT war hart (Leben, Boss-Wände) — unsere Vision ist „ruhig mit
  Puls" (Solo-Zen; Discovery/Sensory vor Challenge).
- **Eigene Ideen obendrauf**: Seiten-Entries/Exits mit Scatter, Overcharge,
  Wellen-Events, Dirty World — das ist die „Verfeinerung".

**Code-Analyse 2026-10-02** (deobfuskiertes SWF; Belege und Zahlen im
Research-Artefakt `context/tasks/research-td-xt-reference/research.md`):
Schwierigkeit ist auch Topologie (Easy einseitig, Normal+ öffnet oben/unten);
klarer Wellenrhythmus aus Normwellen + Eskalationswellen mit HP-Kompoundierung;
Early-Send-Bonus als Zeitdruck-Regler; Leben und Geld sind getrennte Achsen;
6 Missionen mit leichtem Unlock-Save; Lufttreffer sind breit (Vulcan/Plasma/
DCA/Freeze), nur Missile und Sonic sind blind — die „nur DCA"-Erinnerung ist zu
hart. Referenzzahlen bleiben Studienmaterial, keine Zielwerte.

**Grafik/Sound-Anker (pinnen):**

- **Kalibrierung (Maintainer, 2026-10-02): Audio und Visuelles so nah wie
  möglich an Xeno Tactic; Fiktion/UI-Text nur grob ankern, iterativ nach
  Gefühl.** Der Look wird also zum Zielbild, nicht bloß zur Inspiration.
- **XT-Look (aus Screenshots):** dunkler Stahl-/Labor-Look — blaugraue
  Nieten-Kacheln mit Rasterlinien, dunkelrote Map-Variante; Gegner als
  weiß-graue Alien-Bugs (grüne Variante), Form+Farbe kodiert; funktionales
  UI (segmentierter grüner Health-Balken, Gold-Zähler, Icon-Grid-Panel);
  Neon-Cyan-Akzente (Titel), Rot/Weiß-Hazard-Streifen.
- **Sound:** Flash-Ära = funktionale, kurze Blips/Alarme, kein Score —
  unser Synth-SFX-Ansatz trifft das; Ziel ist „lesbar und knapp".
- Umsetzung: **XT-Pass (T11, 2026-10-02)** ist durch — Palette, Sprites, UI
  und Sounds folgen dem Zielbild; `art/STYLEGUIDE.md` und `docs/AUDIO.md`
  tragen die Ist-Stand-Abschnitte.
