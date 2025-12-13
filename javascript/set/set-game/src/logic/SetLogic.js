/**
 * SET Game Logic Module
 * 
 * Implements the mathematical structure of the SET card game
 * using the vector space representation (F₃⁴).
 * 
 * Each card is a 4-dimensional vector over the field with 3 elements.
 * A valid SET is three cards where A + B + C ≡ 0 (mod 3) for each dimension.
 */

/**
 * Card property mappings for visual representation
 */
export const COUNTS = ['one', 'two', 'three'];
export const COLORS = ['red', 'green', 'purple'];
export const FILLS = ['empty', 'striped', 'solid'];
export const SHAPES = ['oval', 'diamond', 'wave'];

/**
 * Creates a card object from numeric values.
 * @param {number} id - Unique card ID (0-80)
 * @param {number} count - 0, 1, or 2
 * @param {number} color - 0, 1, or 2
 * @param {number} fill - 0, 1, or 2
 * @param {number} shape - 0, 1, or 2
 * @returns {Object} Card object
 */
export function createCard(id, count, color, fill, shape) {
    return {
        id,
        count,
        color,
        fill,
        shape,
        // Human-readable properties for display
        countLabel: COUNTS[count],
        colorLabel: COLORS[color],
        fillLabel: FILLS[fill],
        shapeLabel: SHAPES[shape],
    };
}

/**
 * Generates a complete deck of 81 unique cards.
 * Each card represents one combination of the 4 properties with 3 values each.
 * 
 * @returns {Array} Array of 81 card objects
 */
export function generateDeck() {
    const deck = [];
    let id = 0;

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

/**
 * Checks if three cards form a valid SET.
 * A valid SET requires that for each property, the three cards
 * are either all the same or all different.
 * 
 * Mathematical formula: (a + b + c) mod 3 === 0
 * 
 * @param {Object} cardA - First card
 * @param {Object} cardB - Second card
 * @param {Object} cardC - Third card
 * @returns {boolean} True if the cards form a valid SET
 */
export function isSet(cardA, cardB, cardC) {
    const properties = ['count', 'color', 'fill', 'shape'];

    return properties.every(prop => {
        const sum = cardA[prop] + cardB[prop] + cardC[prop];
        return sum % 3 === 0;
    });
}

/**
 * Calculates the third card needed to complete a SET with two given cards.
 * 
 * Mathematical formula: C = -(A + B) mod 3 = (3 - (A + B) mod 3) mod 3
 * 
 * @param {Object} cardA - First card
 * @param {Object} cardB - Second card
 * @returns {Object} The required card properties (without id)
 */
export function findRequiredCard(cardA, cardB) {
    const properties = ['count', 'color', 'fill', 'shape'];
    const required = {};

    properties.forEach(prop => {
        const sum = cardA[prop] + cardB[prop];
        required[prop] = (3 - (sum % 3)) % 3;
    });

    return required;
}

/**
 * Finds all valid SETs in a given set of cards.
 * Uses an efficient algorithm: iterate over pairs and check if the
 * required third card exists in the collection.
 * 
 * @param {Array} cards - Array of cards to search
 * @returns {Array} Array of SETs, where each SET is an array of 3 cards
 */
export function findSets(cards) {
    const sets = [];
    const cardMap = new Map();

    // Build a map for O(1) card lookup by properties
    cards.forEach(card => {
        const key = `${card.count}-${card.color}-${card.fill}-${card.shape}`;
        cardMap.set(key, card);
    });

    // Iterate over all pairs
    for (let i = 0; i < cards.length; i++) {
        for (let j = i + 1; j < cards.length; j++) {
            const required = findRequiredCard(cards[i], cards[j]);
            const key = `${required.count}-${required.color}-${required.fill}-${required.shape}`;

            const thirdCard = cardMap.get(key);

            // Make sure the third card exists and has a higher index to avoid duplicates
            if (thirdCard && thirdCard.id > cards[j].id) {
                sets.push([cards[i], cards[j], thirdCard]);
            }
        }
    }

    return sets;
}

/**
 * Checks if at least one valid SET exists in the given cards.
 * 
 * @param {Array} cards - Array of cards to check
 * @returns {boolean} True if at least one SET exists
 */
export function hasSet(cards) {
    const cardMap = new Map();

    cards.forEach(card => {
        const key = `${card.count}-${card.color}-${card.fill}-${card.shape}`;
        cardMap.set(key, card);
    });

    for (let i = 0; i < cards.length; i++) {
        for (let j = i + 1; j < cards.length; j++) {
            const required = findRequiredCard(cards[i], cards[j]);
            const key = `${required.count}-${required.color}-${required.fill}-${required.shape}`;

            if (cardMap.has(key)) {
                const thirdCard = cardMap.get(key);
                if (thirdCard.id !== cards[i].id && thirdCard.id !== cards[j].id) {
                    return true;
                }
            }
        }
    }

    return false;
}

/**
 * Shuffles an array in-place using the Fisher-Yates algorithm.
 * 
 * @param {Array} array - Array to shuffle
 * @returns {Array} The shuffled array (same reference)
 */
export function shuffleArray(array) {
    for (let i = array.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [array[i], array[j]] = [array[j], array[i]];
    }
    return array;
}

/**
 * Deals cards from the deck.
 * Removes and returns the specified number of cards from the front of the deck.
 * 
 * @param {Array} deck - The deck to deal from (will be mutated)
 * @param {number} count - Number of cards to deal
 * @returns {Array} Array of dealt cards
 */
export function dealCards(deck, count) {
    return deck.splice(0, Math.min(count, deck.length));
}
