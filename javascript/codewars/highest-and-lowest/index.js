/**
 * Given a string of space-separated numbers, this function finds the highest and lowest numbers and returns them as a string.
 *
 * @param {string} numbers A string of space-separated numbers.
 * @returns {string} A string containing the highest and lowest numbers, separated by a space.
 */
function highAndLow(numbers) {
  const arr = numbers.split(" ").map(Number);
  return `${Math.max(...arr)} ${Math.min(...arr)}`;
}

export default highAndLow;
