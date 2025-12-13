# 🗺️ Umsetzungsplan: Vue.js Set-Spiel

> **Status:** ✅ Bereit zur Umsetzung

## Projektübersicht

Dieses Projekt implementiert das klassische **Set-Kartenspiel** als moderne Web-Applikation mit Vue.js 3. Das Spiel nutzt die mathematische Struktur des Spiels (Vektorraum $\mathbb{F}_3^4$) für eine elegante Implementierung.

### Technologie-Stack

| Komponente | Technologie |
|:---|:---|
| **Framework** | Vue 3 mit Composition API |
| **State Management** | Pinia |
| **Styling** | Tailwind CSS |
| **Build Tool** | Vite (empfohlen) |

---

## Phase 1: Datenmodell & Logik (Modul `SetLogic.js`)

Diese Phase implementiert die gesamte Spiel- und Kartenlogik isoliert vom UI.

### Schritt 1.1: Kartendarstellung

Definiere die numerische Darstellung einer Karte als Vektor mit 4 Merkmalen.

**Merkmalsnamen (sprechend):**
- `count` — Anzahl der Symbole (0=1, 1=2, 2=3)
- `color` — Farbe (0=Rot, 1=Grün, 2=Lila)
- `fill` — Füllung (0=Leer, 1=Gestreift, 2=Voll)
- `shape` — Form (0=Oval, 1=Raute, 2=Welle)

**Beispiel-Datenstruktur:**
```javascript
const card = {
  id: 0,           // Eindeutige ID (0-80)
  count: 0,        // 0, 1, oder 2
  color: 1,        // 0, 1, oder 2
  fill: 2,         // 0, 1, oder 2
  shape: 0         // 0, 1, oder 2
};
```

---

### Schritt 1.2: Alle Karten generieren

Erstelle die Menge aller 81 Karten als Array.

**Funktion:** `generateDeck()`
- Generiert alle Kombinationen von `[0,0,0,0]` bis `[2,2,2,2]`
- Jede Karte erhält eine eindeutige `id`
- Rückgabe: Array mit 81 Kartenobjekten

---

### Schritt 1.3: Set-Validierung

**Funktion:** `isSet(cardA, cardB, cardC)`

Kernfunktion zur Überprüfung der Set-Regel.

**Mathematische Formel:** $A + B + C = \mathbf{0} \pmod 3$

**Implementierung:** Für jedes Merkmal muss gelten:
- Entweder alle drei Werte sind **gleich**
- Oder alle drei Werte sind **unterschiedlich**

```javascript
// Für jedes Merkmal prüfen:
(a + b + c) % 3 === 0
```

---

### Schritt 1.4: Dritte Karte berechnen

**Funktion:** `findRequiredCard(cardA, cardB)`

Hilfsfunktion, die die dritte Karte eines Sets berechnet.

**Mathematische Formel:** $C = -(A + B) \pmod 3$ bzw. $C = (3 - (A + B) \mod 3) \mod 3$

---

### Schritt 1.5: Sets finden

**Funktion:** `findSets(cards)`

Sucht alle möglichen Sets in einer gegebenen Kartenmenge.

**Effizienter Algorithmus:**
1. Iteriere über alle Kartenpaare ($n \choose 2$)
2. Berechne die benötigte dritte Karte mit `findRequiredCard()`
3. Prüfe, ob diese Karte in `cards` enthalten ist

**Rückgabe:** Array von Sets (jeweils Array mit 3 Karten)

---

### Schritt 1.6: Deck-Management

**Funktionen:**

- `shuffleDeck(deck)` — Mischt das Deck (Fisher-Yates)
- `dealCards(deck, count)` — Entfernt `count` Karten aus dem Deck und gibt sie zurück

**Entscheidung:** Karten werden beim Austeilen **vollständig aus dem Deck entfernt** (`.splice()`).

---

## Phase 2: Vue-Komponenten (Frontend)

### Schritt 2.1: `Card.vue` — Die einzelne Karte

**Verantwortlichkeiten:**
- Mappt numerische Werte auf visuelle Darstellung
- Zeigt 1-3 Symbole basierend auf `count`
- Stellt Farbe, Füllung und Form dar
- Verwaltet den Klick-Zustand (ausgewählt/nicht ausgewählt)

**Visuelle Merkmale:**

| Wert | `color` | `fill` | `shape` |
|:---:|:---|:---|:---|
| 0 | Rot | Leer (nur Umriss) | Oval |
| 1 | Grün | Gestreift | Raute |
| 2 | Lila | Voll (gefüllt) | Welle |

**Props:**
- `card: Object` — Das Kartenobjekt
- `isSelected: Boolean` — Ob die Karte ausgewählt ist

**Events:**
- `@click` — Emittiert bei Klick auf die Karte

**SVG-Symbole:** Erstelle SVG-Komponenten für Oval, Raute und Welle mit konfigurierbarer Füllung.

---

### Schritt 2.2: `GameBoard.vue` — Das Spielbrett

**Verantwortlichkeiten:**
- Zeigt die aktuell ausgelegten Karten an (12, 15 oder 18)
- Rendert `Card.vue` Komponenten in einem Grid

**Layout:** 4-spaltige Anordnung (4x3, 4x4 oder 4x5 je nach Kartenanzahl)

**Props:**
- `cards: Array` — Die aktuell ausgelegten Karten
- `selectedCards: Array` — Die aktuell ausgewählten Karten

**Events:**
- `@card-click` — Leitet Kartenklick-Events weiter

---

### Schritt 2.3: `ScoreBoard.vue` — Spielinformationen

**Anzeige:**
- Aktuelle Punktzahl
- Verbleibende Kartenanzahl im Deck
- Anzahl der gefundenen Sets

**Hinweis:** Kein Timer implementiert.

---

### Schritt 2.4: `HintBanner.vue` — Hinweis-Banner

**Verantwortlichkeiten:**
- Zeigt kontextabhängige Hinweise an
- Z.B. "Bitte wähle zuerst eine Karte ab!" bei 4. Kartenklick

---

### Schritt 2.5: `App.vue` — Hauptkomponente

**Verantwortlichkeiten:**
- Integriert alle Unterkomponenten
- Verbindet UI mit dem Pinia Store

---

## Phase 3: Spielzustand & Interaktion (Pinia Store)

### Schritt 3.1: Zustandsdefinition

**Store:** `useGameStore`

```javascript
const state = {
  deck: [],              // Karten im Nachziehstapel
  board: [],             // Aktuell ausgelegte Karten (12-18)
  score: 0,              // Punktestand
  selectedCards: [],     // Aktuell ausgewählte Karten (max. 3)
  foundSets: [],         // Array aller gefundenen Sets
  hintMessage: null      // Aktuelle Hinweisnachricht
};
```

---

### Schritt 3.2: Aktionen (Actions)

| Aktion | Beschreibung |
|:---|:---|
| `startGame()` | Initialisiert ein neues Spiel: Deck generieren, mischen, 12 Karten auslegen |
| `selectCard(cardId)` | Fügt Karte zur Auswahl hinzu oder entfernt sie |
| `submitSet()` | Prüft die 3 ausgewählten Karten auf Set |
| `dealThreeMore()` | Legt 3 weitere Karten aus (wenn kein Set auf dem Board) |

---

### Schritt 3.3: `selectCard()` Logik

**Verhalten bei Kartenauswahl:**

1. **Karte bereits ausgewählt:** → Karte abwählen
2. **Weniger als 3 Karten ausgewählt:** → Karte zur Auswahl hinzufügen
3. **Bereits 3 Karten ausgewählt:** → **Klick ignorieren** und Hinweis anzeigen: *"Bitte wähle zuerst eine Karte ab!"*

**Bei 3 ausgewählten Karten:** Automatisch `submitSet()` aufrufen.

---

### Schritt 3.4: `submitSet()` Logik

**Bei korrektem Set:**
1. Punkte erhöhen (+1)
2. Set zu `foundSets` hinzufügen
3. Karten vom Board entfernen
4. 3 neue Karten aus dem Deck nachziehen (falls vorhanden)
5. Auswahl zurücksetzen

**Bei falschem Set:**
1. Das **zuletzt gefundene korrekte Set** (aus `foundSets`) wird **zurück auf das Board gelegt**
2. Das Set wird aus `foundSets` entfernt
3. Punkte werden um 1 reduziert
4. Auswahl zurücksetzen
5. Visuelles Feedback (kurz rot markieren)

> **Hinweis:** Falls noch kein Set gefunden wurde, gibt es keine Strafe außer visuelles Feedback.

---

### Schritt 3.5: Automatisches Nachziehen

**Spielbrett-Management (offizielle Regel):**

Nach jedem gefundenen Set prüfen:
1. Gibt es noch ein Set auf dem Board?
2. Falls **nein** und das Deck nicht leer ist: automatisch 3 Karten hinzufügen
3. Board kann 12, 15 oder 18 Karten enthalten

**Funktion:** `ensureSetsAvailable()`

---

### Schritt 3.6: Spielende

**Das Spiel endet wenn:**
- Das Deck leer ist **UND**
- Kein Set mehr auf dem Board liegt

**Aktion:** `checkGameOver()` — Zeigt Endbildschirm mit finaler Punktzahl

---

## Dateistruktur (Vorschlag)

```
set/
├── index.html
├── package.json
├── vite.config.js
├── tailwind.config.js
├── postcss.config.js
├── src/
│   ├── main.js
│   ├── App.vue
│   ├── logic/
│   │   └── SetLogic.js          # Phase 1: Spiellogik
│   ├── stores/
│   │   └── gameStore.js         # Phase 3: Pinia Store
│   ├── components/
│   │   ├── Card.vue             # Einzelne Karte
│   │   ├── GameBoard.vue        # Spielbrett
│   │   ├── ScoreBoard.vue       # Punktestand
│   │   ├── HintBanner.vue       # Hinweis-Banner
│   │   └── symbols/
│   │       ├── OvalSymbol.vue   # SVG-Symbol
│   │       ├── DiamondSymbol.vue
│   │       └── WaveSymbol.vue
│   └── assets/
│       └── styles/
│           └── main.css         # Tailwind-Imports
```

---

## Umsetzungsreihenfolge

1. **Projekt-Setup:** Vite + Vue 3 + Pinia + Tailwind CSS initialisieren
2. **Phase 1:** `SetLogic.js` implementieren und mit Unit-Tests validieren
3. **Phase 3.1:** Pinia Store Grundstruktur erstellen
4. **Phase 2.1:** SVG-Symbole und `Card.vue` erstellen
5. **Phase 2.2-2.4:** Weitere Komponenten erstellen
6. **Phase 3.2-3.6:** Store-Logik vervollständigen
7. **Integration:** Alle Komponenten in `App.vue` verbinden
8. **Polish:** Styling, Animationen, Endbildschirm

---

## Zusammenfassung der Entscheidungen

| Frage | Entscheidung |
|:---|:---|
| Merkmalsnamen | `count`, `color`, `fill`, `shape` (sprechend) |
| Deck-Management | Karten entfernen (`.splice()`) |
| Symbole | Oval, Raute, Welle (Standard) |
| Layout | 4-spaltiges Grid |
| Timer | Nein |
| 4. Karten-Klick | Ignorieren + Hinweis anzeigen |
| Nachziehen | Immer 3 Karten bei gefundenem Set |
| Framework | Vue 3 + Composition API + Pinia |
| Styling | Tailwind CSS |
| Falsches Set | Letztes korrektes Set zurücklegen |
| Board-Management | Ja, offizielle Regel (12/15/18 Karten) |