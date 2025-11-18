/**
 * Splits the given string into pairs of two characters.
 * If the string has an odd number of characters, it appends an underscore to the end to complete the final pair.
 *
 * @param {string} str The string to split.
 * @returns {string[]} An array of two-character strings.
 */
﻿function solution(str) {
    return (str+"_").match(/.{2}/g)||[]
}

export default solution;