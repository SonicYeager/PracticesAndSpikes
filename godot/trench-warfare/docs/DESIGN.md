# Design-Analyse & Konzept — Trench Warfare (Arbeitstitel)

*Konzeptphase 2026-10-02. Inspiriert von „Warfare 1917" (ConArtists, Armor
Games, 2008) — Mechanik-Studie, keine Asset-/Code-Übernahme. Quellen in §7.*

## 1. Original-Analyse

### Steckbrief

- **Genre**: „Creep Game" — Einheiten werden am eigenen Ende rekrutiert, laufen
  zum feindlichen Ende und kämpfen automatisch.
- **Setting**: Westfront 1917; zwei Kampagnen (Briten/Deutsche) + Custom Battle.
- **Sieg**: (a) feindliche Moral auf 0 → Kapitulation, oder (b) gegnerisches
  Kartenende erobern.
- **Karte**: flacher Frontabschnitt mit Gräben, Stacheldraht und Minen.

### Core Loop

Verstärkung aufstellen → (autonom) vorrücken und Feuerkampf → Frontabschnitt
nehmen/halten → Feuerunterstützung zum Brechen des Grabens → Moral des Gegners
senken → Kapitulation oder Durchbruch.

### Systeme

**Rekrutierung**

- Zeitbasiert statt Geld: Jeder Einheitentyp hat einen eigenen Countdown.
  Sobald irgendeine Einheit rekrutiert wird, starten alle Countdowns neu —
  die Wahl ist damit immer auch eine Timing-Entscheidung.
- Kampagnen-Upgrades verkürzen Countdowns (z. B. „Rally"/„Reinforcement",
  „Post Haste") und verbessern Waffen, Moral und Feuerunterstützung.

**Einheiten (Rollen; Community-Quellen, Details können abweichen)**

| Rolle | Verhalten | Stark gegen | Schwach gegen |
|---|---|---|---|
| Riflemen (5–6er Squad) | Basis, Feuer auf Distanz | Infanterie | Tanks |
| Assault/Sturmtruppen (4er) | Sturm, Mills-Granaten | Gräben, auch Tanks | Feuerunterstützung, Sniper auf Distanz |
| MG-Team (1 MG + 2 Schützen) | Defensive, im Graben enorm stark | Infanterie (im Graben) | Sniper, Artillerie/Gas; fällt der MG-Schütze, bleibt ein halber Rifle-Squad |
| Sniper (Einzel) | Langdistanz, Ausschalten von MGs | Infanterie auf Distanz | Nahkampf, Sprengstoff; teuer im Unterhalt |
| Offizier (Einzel) | Aura: Moral + Genauigkeit/Schaden | (Support) | allein wehrlos |
| Tank (Mk IV / A7V) | „Land Battleship" | fast alles | Feuerunterstützung (Artillerie, Panzerabwehr, Granaten) |

**Gräben**

- Fassen max. 3 Squads; Einheiten im Graben sind deutlich geschützter,
  MG-Teams werden dort viel gefährlicher.
- Kommen mehr Truppen an, gehen sie automatisch „über die Brustwehr"
  (Überlauf); ansonsten bestimmt der Spieler den Zeitpunkt des Ausbruchs.
- **Außerhalb** des Grabens gibt es kein Halten: Einheiten greifen
  automatisch an und laufen weiter vor.

**Moral (zweite Siegbedingung)**

- Verluste senken die Moral; Offiziere heben sie und buffen Nachbarn.
- Tank: Anwesenheit hebt die eigene Moral, Zerstörung schadet stark.
- Moral regeneriert langsam; fällt sie auf 0, ist die Schlacht verloren.
- Die genaue Aufteilung (Seiten- vs. Einheitenmoral) ist in den Quellen nicht
  eindeutig — für uns eine offene Design-Frage.

**Feuerunterstützung** (Friendly Fire ist möglich!)

| Support | Wirkung | Rolle |
|---|---|---|
| Mörser | mittlere Fläche, Explosion | gut gegen Infanterie im Freien |
| Artillerie | schwere Fläche | gegen alle Ziele; ungenau, kann eigene Trefferlandung haben |
| Gas | Flächenwirkung über Zeit | am besten gegen eingegrabene Infanterie; Warnung „Gas! Gas!" |
| Panzerabwehr | Direktschuss, kürzester Cooldown | Tanks |

**Kampagnen-Rahmen**

- Erfahrungspunkte → Tech-Baum mit zwei Ästen (Infanterie/Tanks +
  Feuerunterstützung), z. B. Ballistics, Morale Boost, Tank Command,
  Gas-/Artillerie-Ausbau, Resupply.
- Gegner-KI nutzt dieselben Systeme, erhält in der Kampagne aber Boni
  (z. B. schnellerer Support-Cooldown).

### Design-Lektionen

- Geringe Interaktionsdichte funktioniert, weil **jede Rekrutierung eine echte
  Wahl** mit Counter-Bezug ist (Counter-Spiel statt DPS-Klicken).
- Gräben geben ein klares Ziel (halten/nehmen) und machen Verteidigung stark —
  der Angriff braucht Kombination (Artillerie/Gas + Assault + Tank).
- Moral ist der eigentliche Spannungsbogen: Tanks sind mächtig, aber
  moral-risikoreich; Verluste können das Blatt kippen.
- Zufall (Artillerie-Ungenauigkeit) erzeugt Geschichten, kann aber unfair
  wirken — der Remaster hat Gas z. B. abgeschwächt.
- „Custom Battle" ist bereits im Original die Grundlage — unser Sandbox-Fokus
  ist die konsequente Weiterentwicklung.

## 2. Unser Modell (Sandbox-first)

### Spielmodi & Rahmen

- **Sandbox/Custom ist der Standard**: Solo gegen KI oder lokaler Hotseat
  (zwei Spieler am selben Rechner); Regler statt Regeln.
- **Kampagne als optionaler Rahmen** (später): verpackt Sandbox-Setups als
  Missionen — kein Pflichtprogramm, keine Grind-Progression.
- Offen: Hotseat im Echtzeit-Gefecht braucht ein eigenes Format (geteilte
  Deploy-Leisten, Pause-Wechsel oder asynchron) — siehe §5.

### Übernehmen

- Creep-Kern und Squads (gut lesbar, klare Rollen).
- Gräben mit Kapazität 3, Halten/Ausbruch, Überlauf.
- Feuerunterstützung inkl. Friendly Fire.
- Moral als zweite Siegbedingung.
- Hazards (Minen, Stacheldraht) als spätere Option.

### Ändern/Erweitern

- **Datengetrieben**: Einheiten sind Datensätze (Stats, Waffen, Rolle,
  Visuals); „Fraktionen" werden Presets aus Pool + kleinen Boni.
- **Custom-Setup**: Map, Startmoral, Cooldown-Takt, Pools, Support-Loadout,
  Siegbedingungen — später als Szenario-Datei speicherbar.
- **Wild erlaubt**: Duplikate, extreme Regler, Kombos aus Einheiten + Support;
  Sandbox ist der Standardmodus. (Einheiten-Mix über Fraktionen: geparkt.)
- **Deterministische Simulation**: fester Tick + Seed; Replay/Telemetrie mit
  den Werkzeugen aus `tower-defense`.
- **Time Control**: Pause/Speed als Experimentierwerkzeug.
- **Lesbarkeit**: Frontlinien-Anzeige, Moral-Meter mit Ursachen, Timeline im
  After-Action-Report.

### Sandbox-Bausteine (Workshop-Priorität)

1. **Regler-Schrank** (Top-Priorität): Moral-Parameter (Startmoral,
   Regeneration, Kapitulationsschwelle), Cooldowns-/Wirtschaftstempo,
   Feuerunterstützung & Loadout (verfügbare Supports, Stärke/Ladungen).
2. **Pools & Loadouts**: Einheitenpool + Support als „Doktrin" wählen,
   optional asymmetrisch (Spieler vs. KI).
3. **Labor-Report**: After-Action mit Timeline (Moralverlauf, Verluste,
   Support-Einsätze) — Futter für die nächste wilde Idee.
4. **Szenario-Persistenz** (JSON/Resource) inkl. Seed → reproduzierbar;
   Teilen ist nicht priorisiert, Save/Load für Presets schon.

Geparkt (nicht in den Top-Reglern): Einheiten-Mix über Fraktionen, direkter
Stats-Editor, Karten-Editor → `docs/IDEAS.md`.

## 3. Technisches Konzept (Godot 4.7 / GDScript)

- **Muster wie `tower-defense`**: Kernlogik als `RefCounted`-Klassen ohne
  Node-Zugriff, dünner Scene-Controller für Darstellung; GUT-Tests für den
  Kern (inkl. Headless-Sim-Tick).
- **Fixed-Tick-Simulation** (z. B. 20–30 Hz) getrennt von der Darstellung;
  Seed-Logging, Replayfähigkeit, Analyzer.
- **Weltmodell** (ADR-Kandidat): horizontale Schlachtzone auf **einem
  Bildschirm** (kein Scroll; passt zum 5–15-Min-Session-Ziel), Entitäten mit
  Position + simplem Verhalten (vorgehen, Deckung, Feuerreichweite);
  Gräben/Zonen mit Kapazität. Lanes/Pfade vs. freies 2D vor S01 entscheiden.
- **Squads**: Eine Einheit = Squad-Entität mit Mitgliedern (HP-Slots, Waffen);
  Ausfälle bleiben sichtbar, Simulation bleibt simpel.
- **Kampfmodell**: Waffentypen (Gewehr, MG, Granate, Artillerie, Gas) ×
  Schutz (offen/Graben/Krater/Tank) mit klaren Counter-Beziehungen.
- **KI**: Utility-basiert, nutzt dieselben Systeme wie der Spieler;
  Schwierigkeit über Regler/Sloppiness, nicht über versteckte Cheats.
- **Wiederverwendung**: GUT-Vendor, Platzhalter-/Sound-Generatoren,
  Telemetrie + `analyze_run.py`-Muster, Doku-Layout aus `tower-defense`.
- **Performance-Budget**: MVP ≤ ~50 Squads; Ausbau messen, nicht raten.

## 4. Slice-Roadmap (Vorschlag)

Jede Slice: Vision-Frage + Kill-Kriterium (Workspace-Standard).

| Slice | Inhalt | Vision-Frage | Kill-Kriterium |
|---|---|---|---|
| S01 | Walking Skeleton: ein Bildschirm, je 1 Graben, 1 Einheitentyp (Rifle), Cooldown-Rekrutierung, Auto-Vorstoß + Kampf, Sieg bei Graben-Einnahme | Trägt Beobachten? | Langweilig — oder man *will* Mikro-Befehle (dann stimmt der Kern nicht) |
| S02 | Gräben & Front: 3 Linien, Kapazität/Halten/Ausbruch, Deckung, zweiter Typ (MG) | Entsteht Push/Pull? | Front kippt nur durch Masse, keine Kombos nötig |
| S03 | Moral & Support: Seitenmoral + Kapitulation, Mörser/Artillerie/Gas mit Friendly Fire | Support-Entscheidungen spannend? | Support ist reiner Zahlen-Button oder wirkt unfair |
| S04 | Sandbox-Lite: datengetriebene Units + Regler für Moral/Tempo/Support + Presets | Macht Ausprobieren Spaß? | Setup-Gefummel dominiert das Spiel |
| S05 | Labor: After-Action-Report, Szenario speichern/laden, ggf. Kartengenerator | Lernt man aus Schlachten? | Report wird nie angeschaut |
| Später | Hotseat (lokal), Kampagnen-Rahmen, Tanks/Anti-Tank, Minen/Stacheldraht, Art-Pass | — | — |

## 5. Offene Entscheidungen (ADR-Kandidaten)

- Deterministischer Fixed-Tick + Seed (Grundsatz) → ADR bei S01.
- Weltmodell-Details: freies 2D vs. Lanes/Pfade (Arena steht: ein Bildschirm).
- Moral: Seiten- vs. Einheiten-Ebene (oder beides kombiniert) — zentral, weil
  Moral-Parameter Top-Sandbox-Regler sind.
- Hotseat-Format im Echtzeit-Gefecht: geteilte Deploy-Leisten, Pause-Wechsel
  oder asynchron?
- Datenformat: Godot-`Resource` (editorfreundlich) vs. JSON (portabel);
  Teilen ist nicht priorisiert, Presets/Save-Load sind der Treiber.
- Reihenfolge: Sandbox-Umfang im MVP vs. Kampagnen-Rahmen.
- KI-Güte/Sloppiness: Wie verliert die KI „menschlich" statt dumm oder
  allwissend?

## 6. Risiken & Gegenmaßnahmen

| Risiko | Gegenmaßnahme |
|---|---|
| Zuschauen wirkt passiv | Kurze Gefechte, klare Support-Agency, Pause/Speed, nächste Rekrutierung als Taktgeber |
| Hotseat im Echtzeitspiel unklar | Format früh prototypisch klären (geteilte Leisten/Pause-Wechsel); notfalls asynchron |
| Autonome KI schwer zu balancieren | Determinismus + Telemetrie + Analyzer; wenige, klare Counter |
| Scope Creep (Kampagne, Art) | Anti-Pillars, Sandbox zuerst, jede Slice mit Kill-Kriterium |
| Rechtliches (Original) | Nur Mechanik-Inspiration; eigene Assets, Namen, Texte |
| Performance/Determinismus | Fixed Tick, Entitätsbudget, Headless-Sim-Tests |

## 7. Quellen

- Original (Steckbrief, Features): <https://armorgames.com/play/2267/warfare-1917>
- Mechanik-Review 2008 (Creep-Kern, Gräben, Moral, Zufall):
  <https://heterogenoustasks.wordpress.com/2008/12/13/warfare-1917>
- Einheiten/Feuerunterstützung/Upgrades (Community-Wiki):
  <https://en.namu.wiki/w/Warfare%201917>
- Taktik-Guides (Community):
  <https://armorgames.com/community/thread/1982381/warfare-1917-guides-and-tips>
- Workspace-Frameworks: `docs/GAMEDEV.md`; Architekturmuster:
  `godot/tower-defense/docs/ARCHITECTURE.md`
