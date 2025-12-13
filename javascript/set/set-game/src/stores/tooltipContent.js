/**
 * TOOLTIP CONTENT - Mathematical Explanations
 * ============================================
 * 
 * This file contains all tooltip text for the SET game UI.
 * Tooltips are written in German, suitable for secondary school students.
 * 
 * STRUCTURE:
 * Each tooltip has a 'title' (heading) and 'content' (explanation).
 * Content uses HTML for formatting (bold, lists, etc.).
 */

export const TOOLTIPS = {

    // =========================================================================
    // GAME RULES
    // =========================================================================

    gameRules: {
        title: 'Was ist ein SET?',
        content: `
            <p>Ein <strong>SET</strong> besteht aus 3 Karten, bei denen <strong>jedes der 4 Merkmale</strong> folgende Regel erfüllt:</p>
            <ul>
                <li>Entweder sind alle drei <strong>gleich</strong></li>
                <li>Oder alle drei sind <strong>verschieden</strong></li>
            </ul>
            <p><strong>Beispiel für ein gültiges SET:</strong></p>
            <ul>
                <li>Anzahl: 1, 2, 3 → alle verschieden ✓</li>
                <li>Farbe: rot, rot, rot → alle gleich ✓</li>
                <li>Füllung: leer, gestreift, voll → alle verschieden ✓</li>
                <li>Form: Oval, Oval, Oval → alle gleich ✓</li>
            </ul>
            <p><strong>Kein SET:</strong> Zwei rote und eine grüne Karte (weder alle gleich noch alle verschieden!)</p>
        `,
    },

    // =========================================================================
    // VECTORS
    // =========================================================================

    vectors: {
        title: 'Vektoren im SET-Spiel',
        content: `
            <p><strong>Was ist ein Vektor?</strong></p>
            <p>Ein Vektor ist eine <em>geordnete Liste von Zahlen</em>. Im SET-Spiel nutzen wir 4-dimensionale Vektoren:</p>
            <pre>[Anzahl, Farbe, Füllung, Form]</pre>
            
            <p><strong>Beispiel:</strong></p>
            <p>2 rote gefüllte Rauten = <code>[1, 0, 2, 1]</code></p>
            <ul>
                <li>Anzahl 1 → 2 Symbole</li>
                <li>Farbe 0 → Rot</li>
                <li>Füllung 2 → Voll</li>
                <li>Form 1 → Raute</li>
            </ul>
            
            <p><strong>Der Vektorraum F₃⁴:</strong></p>
            <ul>
                <li>F₃ = nur Zahlen 0, 1, 2 erlaubt</li>
                <li>⁴ = 4 Dimensionen (4 Merkmale)</li>
                <li>Gesamt: 3×3×3×3 = <strong>81 Karten!</strong></li>
            </ul>
        `,
    },

    // =========================================================================
    // MODULO ARITHMETIC
    // =========================================================================

    modulo: {
        title: 'Modulo-Arithmetik',
        content: `
            <p><strong>Was bedeutet "mod 3"?</strong></p>
            <p>Modulo gibt den <em>Rest bei Division</em> an:</p>
            <ul>
                <li>7 mod 3 = <strong>1</strong> (denn 7 ÷ 3 = 2 Rest 1)</li>
                <li>6 mod 3 = <strong>0</strong> (denn 6 ÷ 3 = 2 Rest 0)</li>
                <li>5 mod 3 = <strong>2</strong> (denn 5 ÷ 3 = 1 Rest 2)</li>
            </ul>
            
            <p><strong>Anwendung im SET-Spiel:</strong></p>
            <p>Drei Zahlen sind "alle gleich" ODER "alle verschieden" genau dann, wenn:</p>
            <pre>(a + b + c) mod 3 = 0</pre>
            
            <table>
                <tr><th>Werte</th><th>Summe</th><th>mod 3</th><th>Gültig?</th></tr>
                <tr><td>0, 0, 0</td><td>0</td><td>0</td><td>✓ gleich</td></tr>
                <tr><td>0, 1, 2</td><td>3</td><td>0</td><td>✓ verschieden</td></tr>
                <tr><td>0, 0, 1</td><td>1</td><td>1</td><td>✗ gemischt</td></tr>
            </table>
        `,
    },

    // =========================================================================
    // COMBINATORICS
    // =========================================================================

    combinatorics: {
        title: 'Kombinatorik: Wie viele SETs?',
        content: `
            <p><strong>Im gesamten Deck (81 Karten):</strong></p>
            <ul>
                <li>Mögliche Kartenpaare: 81 × 80 / 2 = <strong>3240</strong></li>
                <li>Jedes Paar bestimmt genau eine dritte Karte</li>
                <li>Jedes SET wird 3× gezählt (einmal pro Paar)</li>
                <li>Gesamt: 3240 / 3 = <strong>1080 SETs</strong></li>
            </ul>
            
            <p><strong>Bei 12 Karten auf dem Tisch:</strong></p>
            <ul>
                <li>Mögliche Dreierkombinationen: C(12,3) = 220</li>
                <li>Davon sind im Schnitt <strong>2-4 gültige SETs</strong></li>
                <li>Wahrscheinlichkeit für mindestens ein SET: ~97%</li>
            </ul>
        `,
    },

    // =========================================================================
    // THIRD CARD CALCULATION
    // =========================================================================

    thirdCard: {
        title: 'Die dritte Karte berechnen',
        content: `
            <p><strong>Die Formel:</strong></p>
            <p>Wenn du zwei Karten kennst, kannst du die dritte berechnen!</p>
            <pre>C = (3 - (A + B)) mod 3</pre>
            
            <p><strong>Warum funktioniert das?</strong></p>
            <p>Wir brauchen: A + B + C ≡ 0 (mod 3)</p>
            <p>Umgestellt: C ≡ -(A + B) (mod 3)</p>
            
            <p><strong>Beispiel (für ein Merkmal):</strong></p>
            <ul>
                <li>A = 0, B = 1 → C = (3-1) mod 3 = <strong>2</strong></li>
                <li>Check: 0 + 1 + 2 = 3 → 3 mod 3 = 0 ✓</li>
            </ul>
            
            <p><strong>Intuition:</strong></p>
            <ul>
                <li>Beide gleich → dritte muss auch gleich sein</li>
                <li>Beide verschieden → dritte muss die fehlende sein</li>
            </ul>
        `,
    },

    // =========================================================================
    // HINT BUTTON
    // =========================================================================

    hintButton: {
        title: 'Hinweis-System',
        content: `
            <p>Der Hinweis-Button zeigt alle möglichen SETs auf dem Spielfeld.</p>
            
            <p><strong>Die Badges:</strong></p>
            <ul>
                <li>Jedes SET bekommt eine Farbe + Nummer</li>
                <li>Karten mit mehreren Badges gehören zu mehreren SETs</li>
            </ul>
            
            <p><strong>Tipp:</strong> Versuche erst selbst zu suchen, bevor du Hinweise nutzt!</p>
        `,
    },

    // =========================================================================
    // SCORE
    // =========================================================================

    score: {
        title: 'Punktestand',
        content: `
            <p><strong>Punkte sammeln:</strong></p>
            <ul>
                <li>Korrektes SET gefunden: <strong>+1 Punkt</strong></li>
            </ul>
            
            <p><strong>Strafe bei falschem SET:</strong></p>
            <ul>
                <li>Das zuletzt gefundene SET wird zurückgelegt</li>
                <li>Du verlierst <strong>1 Punkt</strong></li>
            </ul>
            
            <p>Das Spiel endet, wenn keine SETs mehr möglich sind.</p>
        `,
    },
};

/**
 * Get a random tooltip for variety
 * @returns {Object} Random tooltip object
 */
export function getRandomTooltip() {
    const keys = Object.keys(TOOLTIPS);
    const randomKey = keys[Math.floor(Math.random() * keys.length)];
    return TOOLTIPS[randomKey];
}
