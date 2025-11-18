/**
 * Given n, take the sum of the digits of n.
 * If that value has more than one digit, continue reducing in this way until a single-digit number is produced.
 * The input will be a non-negative integer.
 *
 * @param {number} n The number to calculate the digital root of.
 * @returns {number} The digital root of the given number.
 */
function digitalRoot(n) {
    const digits = n.toString().split('').map(Number);

    const sum = digits.reduce((acc, digit) => acc + digit, 0);

    if (sum < 10) {
        return sum;
    }

    return digitalRoot(sum);
}

export default digitalRoot;