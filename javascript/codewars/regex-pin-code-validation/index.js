/**
 * Validates a PIN to ensure it contains exactly 4 or 6 digits.
 *
 * @param {string} pin The PIN to validate.
 * @returns {boolean} True if the PIN is valid, false otherwise.
 */
﻿function validatePIN(pin) {
    return /^(\d{4}|\d{6})$/.test(pin);
}

export default validatePIN;