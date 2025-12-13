import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import {
    generateDeck,
    shuffleArray,
    dealCards,
    isSet,
    hasSet,
} from '@/logic/SetLogic.js';

export const useGameStore = defineStore('game', () => {
    // State
    const deck = ref([]);
    const board = ref([]);
    const selectedCards = ref([]);
    const foundSets = ref([]);
    const score = ref(0);
    const hintMessage = ref(null);
    const hintType = ref('info'); // 'info', 'success', 'error'
    const gameOver = ref(false);

    // Getters
    const selectedCardIds = computed(() =>
        selectedCards.value.map(card => card.id)
    );

    const cardsInDeck = computed(() => deck.value.length);

    const setsFound = computed(() => foundSets.value.length);

    const hasSetOnBoard = computed(() => hasSet(board.value));

    // Actions

    /**
     * Starts a new game: generates and shuffles deck, deals 12 cards
     */
    function startGame() {
        // Generate and shuffle a new deck
        const newDeck = generateDeck();
        shuffleArray(newDeck);
        deck.value = newDeck;

        // Deal 12 cards to the board
        board.value = dealCards(deck.value, 12);

        // Reset game state
        selectedCards.value = [];
        foundSets.value = [];
        score.value = 0;
        hintMessage.value = null;
        gameOver.value = false;

        // Ensure there's at least one SET on the board
        ensureSetsAvailable();
    }

    /**
     * Handles card selection
     */
    function selectCard(card) {
        clearHint();

        // Check if card is already selected
        const index = selectedCards.value.findIndex(c => c.id === card.id);

        if (index !== -1) {
            // Deselect the card
            selectedCards.value.splice(index, 1);
            return;
        }

        // Check if already 3 cards selected
        if (selectedCards.value.length >= 3) {
            showHint('Bitte wähle zuerst eine Karte ab!', 'info');
            return;
        }

        // Add card to selection
        selectedCards.value.push(card);

        // If 3 cards are now selected, automatically check for SET
        if (selectedCards.value.length === 3) {
            submitSet();
        }
    }

    /**
     * Checks if the 3 selected cards form a valid SET
     */
    function submitSet() {
        if (selectedCards.value.length !== 3) return;

        const [cardA, cardB, cardC] = selectedCards.value;

        if (isSet(cardA, cardB, cardC)) {
            handleCorrectSet();
        } else {
            handleWrongSet();
        }
    }

    /**
     * Handles a correct SET
     */
    function handleCorrectSet() {
        const setCards = [...selectedCards.value];

        // Add to found sets
        foundSets.value.push(setCards);

        // Increase score
        score.value++;

        // Remove cards from board
        const setIds = setCards.map(c => c.id);
        board.value = board.value.filter(c => !setIds.includes(c.id));

        // Deal new cards from deck
        const newCards = dealCards(deck.value, 3);
        board.value.push(...newCards);

        // Clear selection
        selectedCards.value = [];

        // Show success message
        showHint('Korrektes SET! +1 Punkt', 'success');

        // Ensure there's still a SET on the board
        ensureSetsAvailable();

        // Check for game over
        checkGameOver();
    }

    /**
     * Handles an incorrect SET attempt
     */
    function handleWrongSet() {
        // Clear selection
        selectedCards.value = [];

        // If player has found sets before, put the last one back
        if (foundSets.value.length > 0) {
            const lastSet = foundSets.value.pop();

            // Add cards back to the board
            board.value.push(...lastSet);

            // Decrease score
            score.value--;

            showHint('Falsches SET! Letztes SET wurde zurückgelegt.', 'error');
        } else {
            showHint('Falsches SET! Versuche es erneut.', 'error');

            // Even without penalty, ensure there's a SET available
            // (in case the board somehow has no SET)
            ensureSetsAvailable();
            checkGameOver();
        }
    }

    /**
     * Ensures there's at least one SET on the board.
     * Deals 3 more cards if no SET exists and deck has cards.
     */
    function ensureSetsAvailable() {
        while (!hasSet(board.value) && deck.value.length >= 3) {
            const newCards = dealCards(deck.value, 3);
            board.value.push(...newCards);
        }
    }

    /**
     * Adds 3 more cards to the board (manual action, if allowed)
     */
    function dealThreeMore() {
        if (deck.value.length >= 3 && !hasSetOnBoard.value) {
            const newCards = dealCards(deck.value, 3);
            board.value.push(...newCards);
            showHint('3 neue Karten hinzugefügt.', 'info');

            // After adding cards, check if there's now a SET
            // If not, keep adding until there is one (or deck is empty)
            ensureSetsAvailable();

            // Check for game over if still no SET possible
            checkGameOver();
        } else if (hasSetOnBoard.value) {
            showHint('Es gibt noch ein SET auf dem Spielfeld!', 'info');
        } else {
            showHint('Keine Karten mehr im Deck.', 'info');
        }
    }

    /**
     * Checks if the game is over
     */
    function checkGameOver() {
        if (deck.value.length === 0 && !hasSet(board.value)) {
            gameOver.value = true;
            showHint(`Spiel beendet! Endpunktzahl: ${score.value}`, 'success');
        }
    }

    /**
     * Shows a hint message
     */
    function showHint(message, type = 'info') {
        hintMessage.value = message;
        hintType.value = type;

        // Auto-hide after 3 seconds
        setTimeout(() => {
            if (hintMessage.value === message) {
                clearHint();
            }
        }, 3000);
    }

    /**
     * Clears the hint message
     */
    function clearHint() {
        hintMessage.value = null;
    }

    return {
        // State
        deck,
        board,
        selectedCards,
        foundSets,
        score,
        hintMessage,
        hintType,
        gameOver,
        // Getters
        selectedCardIds,
        cardsInDeck,
        setsFound,
        hasSetOnBoard,
        // Actions
        startGame,
        selectCard,
        submitSet,
        dealThreeMore,
        showHint,
        clearHint,
    };
});
