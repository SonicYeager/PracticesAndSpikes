/**
 * ============================================================================
 * SET GAME LOGIC MODULE
 * ============================================================================
 * 
 * This module implements all the mathematical and game logic for the SET card
 * game. It is completely independent from the UI (Vue components) and can be
 * tested or used separately.
 * 
 * ============================================================================
 * MATHEMATICAL BACKGROUND: VECTOR SPACES
 * ============================================================================
 * 
 * WHAT IS A VECTOR?
 * -----------------
 * A vector is an ordered list of numbers. In this game, each card is 
 * represented as a 4-dimensional vector because it has 4 properties:
 * 
 *   Card = [count, color, fill, shape]
 *   
 *   Example: [1, 0, 2, 1] = 2 red solid diamonds
 * 
 * THE VECTOR SPACE F₃⁴
 * --------------------
 * - F₃ (also written as Z₃ or GF(3)) is the "field with 3 elements"
 *   It means we only use the numbers 0, 1, 2 and do arithmetic modulo 3
 *   
 * - F₃⁴ means 4-dimensional vectors over F₃
 *   Each component can be 0, 1, or 2
 *   
 * - Total number of vectors: 3 × 3 × 3 × 3 = 81 = number of cards!
 * 
 * WHY DOES THE SET RULE WORK MATHEMATICALLY?
 * ------------------------------------------
 * The SET rule says: "For each property, all three cards must be the same
 * or all different."
 * 
 * Mathematically, "all same or all different" is equivalent to:
 *   (a + b + c) mod 3 = 0
 * 
 * Proof:
 *   - All same: a = b = c → a + b + c = 3a → 3a mod 3 = 0 ✓
 *   - All different: {a,b,c} = {0,1,2} → 0 + 1 + 2 = 3 → 3 mod 3 = 0 ✓
 *   - Two same, one different: e.g. 0 + 0 + 1 = 1 → 1 mod 3 = 1 ✗
 * 
 * This is why the isSet() function simply checks if the sum mod 3 = 0
 * for each property!
 * 
 * ============================================================================
 * COMBINATORICS: HOW MANY SETS EXIST?
 * ============================================================================
 * 
 * Total number of ways to choose 2 cards from 81: C(81,2) = 81×80/2 = 3240
 * 
 * For any pair of cards, there is EXACTLY ONE third card that completes a SET.
 * (This is because the equation A + B + C ≡ 0 has a unique solution for C)
 * 
 * Each SET has 3 pairs, so we count each SET 3 times.
 * Total SETs = 3240 / 3 = 1080
 * 
 * Fun fact: With 12 cards on the board, there's about a 97% chance of having
 * at least one SET. The game adds cards when no SET exists.
 * 
 * ============================================================================
 * ALGORITHM EFFICIENCY
 * ============================================================================
 * 
 * NAIVE APPROACH: O(n³)
 * Check all possible triplets: n × (n-1) × (n-2) / 6 combinations
 * For 12 cards: 220 triplets to check
 * 
 * OUR APPROACH: O(n²)  
 * 1. Build a HashMap of all cards by their properties: O(n)
 * 2. For each pair of cards: O(n²)
 *    - Calculate the required third card: O(1)
 *    - Look it up in the HashMap: O(1)
 * 
 * For 12 cards: 66 pairs to check (much faster!)
 * 
 * ============================================================================
 */

// ============================================================================
// CARD PROPERTY CONSTANTS
// ============================================================================

/**
 * Human-readable labels for each property value.
 * Index 0, 1, 2 maps to the corresponding label.
 */
export const COUNTS = ['one', 'two', 'three'];
export const COLORS = ['red', 'green', 'purple'];
export const FILLS = ['empty', 'striped', 'solid'];
export const SHAPES = ['oval', 'diamond', 'wave'];

// ============================================================================
// CARD CREATION
// ============================================================================

/**
 * Creates a card object from numeric values.
 * 
 * JAVASCRIPT CONCEPTS:
 * - Object shorthand: { id } is the same as { id: id }
 * - Array indexing: COUNTS[count] gets the label at position 'count'
 * 
 * VECTOR REPRESENTATION:
 * The card represents the vector [count, color, fill, shape] in F₃⁴
 * 
 * @param {number} id - Unique card ID (0-80)
 * @param {number} count - 0 (one), 1 (two), or 2 (three symbols)
 * @param {number} color - 0 (red), 1 (green), or 2 (purple)
 * @param {number} fill - 0 (empty), 1 (striped), or 2 (solid)
 * @param {number} shape - 0 (oval), 1 (diamond), or 2 (wave)
 * @returns {Object} Card object with both numeric and label properties
 */
export function createCard(id, count, color, fill, shape) {
    return {
        id,
        count,
        color,
        fill,
        shape,
        // Human-readable labels for the UI
        countLabel: COUNTS[count],
        colorLabel: COLORS[color],
        fillLabel: FILLS[fill],
        shapeLabel: SHAPES[shape],
    };
}

// ============================================================================
// DECK GENERATION
// ============================================================================

/**
 * Generates a complete deck of 81 unique cards.
 * 
 * MATHEMATICAL INSIGHT:
 * We're generating all 81 vectors in F₃⁴ by iterating through all possible
 * combinations of 4 properties with 3 values each.
 * 
 * NESTED LOOPS EXPLANATION:
 * 4 nested loops (one per property) × 3 iterations each = 3⁴ = 81 cards
 * 
 * The loops create combinations in order:
 * [0,0,0,0], [0,0,0,1], [0,0,0,2], [0,0,1,0], ... [2,2,2,2]
 * 
 * @returns {Array} Array of 81 card objects
 */
export function generateDeck() {
    const deck = [];
    let id = 0;

    // Four nested loops generate all 3⁴ = 81 combinations
    for (let count = 0; count < 3; count++) {
        for (let color = 0; color < 3; color++) {
            for (let fill = 0; fill < 3; fill++) {
                for (let shape = 0; shape < 3; shape++) {
                    deck.push(createCard(id++, count, color, fill, shape));
                }
            }
        }
    }

    return deck;
}

// ============================================================================
// SET VALIDATION
// ============================================================================

/**
 * Checks if three cards form a valid SET.
 * 
 * THE SET RULE:
 * For EACH of the 4 properties, the three cards must be:
 * - ALL THE SAME, or
 * - ALL DIFFERENT
 * 
 * MATHEMATICAL PROOF:
 * This is equivalent to checking: (a + b + c) mod 3 = 0
 * 
 * Why does this work?
 * - All same (e.g., 2+2+2 = 6): 6 mod 3 = 0 ✓
 * - All different (0+1+2 = 3): 3 mod 3 = 0 ✓
 * - Two same, one different (0+0+1 = 1): 1 mod 3 ≠ 0 ✗
 * 
 * JAVASCRIPT CONCEPTS:
 * - Array.every(): Returns true only if ALL elements satisfy the condition
 * - Arrow functions: (prop) => { ... } is a compact function syntax
 * - Modulo operator: % gives the remainder of division
 * 
 * @param {Object} cardA - First card
 * @param {Object} cardB - Second card
 * @param {Object} cardC - Third card
 * @returns {boolean} True if the three cards form a valid SET
 */
export function isSet(cardA, cardB, cardC) {
    const properties = ['count', 'color', 'fill', 'shape'];

    // Check each property: sum mod 3 must equal 0
    return properties.every(prop => {
        const sum = cardA[prop] + cardB[prop] + cardC[prop];
        return sum % 3 === 0;
    });
}

// ============================================================================
// THIRD CARD CALCULATION
// ============================================================================

/**
 * Calculates the third card needed to complete a SET with two given cards.
 * 
 * MATHEMATICAL DERIVATION:
 * We need: A + B + C ≡ 0 (mod 3)
 * Solving for C: C ≡ -A - B ≡ -(A + B) (mod 3)
 * 
 * Since we want a positive result (0, 1, or 2):
 * C = (3 - ((A + B) mod 3)) mod 3
 * 
 * EXAMPLES (for one property):
 * - A=0, B=0: C = (3 - 0) mod 3 = 0  → needs same value
 * - A=0, B=1: C = (3 - 1) mod 3 = 2  → needs the missing value
 * - A=0, B=2: C = (3 - 2) mod 3 = 1  → needs the missing value
 * - A=1, B=1: C = (3 - 2) mod 3 = 1  → needs same value
 * 
 * INTUITIVE UNDERSTANDING:
 * - If A and B are the same → C must also be the same
 * - If A and B are different → C must be the third option
 * 
 * @param {Object} cardA - First card
 * @param {Object} cardB - Second card
 * @returns {Object} Object with the required property values (no id/labels)
 */
export function findRequiredCard(cardA, cardB) {
    const properties = ['count', 'color', 'fill', 'shape'];
    const required = {};

    properties.forEach(prop => {
        const sum = cardA[prop] + cardB[prop];
        // Calculate C = -(A+B) mod 3, ensuring positive result
        required[prop] = (3 - (sum % 3)) % 3;
    });

    return required;
}

// ============================================================================
// SET FINDING ALGORITHMS
// ============================================================================

/**
 * Finds all valid SETs in a given set of cards.
 * 
 * ALGORITHM (O(n²) using HashMap):
 * 1. Build a HashMap: property-string → card
 * 2. For each pair of cards (i, j):
 *    a. Calculate what the third card must be
 *    b. Look it up in the HashMap
 *    c. If found and card.id > j.id, we have a SET (the id check avoids duplicates)
 * 
 * WHY IS THIS FASTER THAN O(n³)?
 * - Naive: Check all C(n,3) = n(n-1)(n-2)/6 triplets
 * - Our way: Check C(n,2) = n(n-1)/2 pairs, then O(1) lookup
 * 
 * JAVASCRIPT CONCEPTS:
 * - Map: A key-value data structure with O(1) lookup
 * - Template literals: `${value}` creates strings with embedded values
 * - Array.some(): Returns true if ANY element satisfies the condition
 * 
 * @param {Array} cards - Array of cards to search (usually 12-18 cards on board)
 * @returns {Array} Array of SETs, where each SET is an array of 3 card objects
 */
export function findSets(cards) {
    const sets = [];
    const cardMap = new Map();

    // Step 1: Build HashMap for O(1) card lookup
    // Key format: "count-color-fill-shape" (e.g., "0-1-2-0")
    cards.forEach(card => {
        const key = `${card.count}-${card.color}-${card.fill}-${card.shape}`;
        cardMap.set(key, card);
    });

    // Step 2: Check all pairs
    for (let i = 0; i < cards.length; i++) {
        for (let j = i + 1; j < cards.length; j++) {
            // Calculate what third card is needed
            const required = findRequiredCard(cards[i], cards[j]);
            const key = `${required.count}-${required.color}-${required.fill}-${required.shape}`;

            const thirdCard = cardMap.get(key);

            // Check if third card exists and has higher ID than BOTH other cards to avoid duplicates
            // Without checking against ALL other cards, we might count the same set multiple times
            // (e.g., once as [A, B] -> C, and again as [A, C] -> B)
            if (thirdCard && thirdCard.id > cards[j].id && thirdCard.id > cards[i].id) {
                sets.push([cards[i], cards[j], thirdCard]);
            }
        }
    }

    return sets;
}

/**
 * Checks if at least one valid SET exists in the given cards.
 * 
 * This is an optimized version of findSets() that returns early
 * as soon as it finds one SET (doesn't need to find all of them).
 * 
 * PERFORMANCE:
 * - Best case: O(1) if the first two cards form a SET with a third
 * - Worst case: O(n²) if no SET exists (must check all pairs)
 * - Average case: Much faster than worst case since SETs are common
 * 
 * @param {Array} cards - Array of cards to check
 * @returns {boolean} True if at least one SET exists
 */
export function hasSet(cards) {
    const cardMap = new Map();

    // Build lookup map
    cards.forEach(card => {
        const key = `${card.count}-${card.color}-${card.fill}-${card.shape}`;
        cardMap.set(key, card);
    });

    // Search for any SET, return immediately when found
    for (let i = 0; i < cards.length; i++) {
        for (let j = i + 1; j < cards.length; j++) {
            const required = findRequiredCard(cards[i], cards[j]);
            const key = `${required.count}-${required.color}-${required.fill}-${required.shape}`;

            if (cardMap.has(key)) {
                const thirdCard = cardMap.get(key);
                // Make sure third card isn't one of our pair!
                if (thirdCard.id !== cards[i].id && thirdCard.id !== cards[j].id) {
                    return true; // Found a SET, no need to continue
                }
            }
        }
    }

    return false; // No SET found after checking all pairs
}

// ============================================================================
// DECK UTILITIES
// ============================================================================

/**
 * Shuffles an array in-place using the Fisher-Yates algorithm.
 * 
 * THE FISHER-YATES SHUFFLE:
 * This is the standard algorithm for generating a uniform random permutation.
 * It's provably unbiased: every possible ordering is equally likely.
 * 
 * ALGORITHM:
 * 1. Start at the last element
 * 2. Swap it with a random element from index 0 to current
 * 3. Move to the previous element, repeat
 * 
 * WHY NOT JUST SORT WITH RANDOM COMPARISONS?
 * That approach is biased! Some orderings become more likely than others.
 * Fisher-Yates guarantees each of n! permutations has probability 1/n!
 * 
 * JAVASCRIPT CONCEPTS:
 * - Math.random(): Returns number in [0, 1)
 * - Math.floor(): Rounds down to nearest integer
 * - Destructuring swap: [a, b] = [b, a] swaps values without temp variable
 * 
 * @param {Array} array - Array to shuffle (will be mutated!)
 * @returns {Array} The same array, now shuffled
 */
export function shuffleArray(array) {
    // Iterate backwards from last element to second element
    for (let i = array.length - 1; i > 0; i--) {
        // Pick random index from 0 to i (inclusive)
        const j = Math.floor(Math.random() * (i + 1));
        // Swap elements at i and j using destructuring
        [array[i], array[j]] = [array[j], array[i]];
    }
    return array;
}

/**
 * Deals cards from the deck.
 * 
 * Removes and returns the specified number of cards from the FRONT of the deck.
 * 
 * JAVASCRIPT CONCEPTS:
 * - Array.splice(start, deleteCount): Removes elements and returns them
 * - Math.min(): Prevents trying to deal more cards than exist
 * 
 * SIDE EFFECT WARNING:
 * This function MUTATES the deck array! After calling dealCards(deck, 3),
 * the deck will have 3 fewer cards. This is intentional for game logic.
 * 
 * @param {Array} deck - The deck to deal from (WILL BE MUTATED)
 * @param {number} count - Number of cards to deal
 * @returns {Array} Array of dealt cards
 */
export function dealCards(deck, count) {
    // splice removes elements from array and returns them
    // Math.min prevents error if trying to deal more than deck has
    return deck.splice(0, Math.min(count, deck.length));
}
