function highAndLow(numbers) {
  const arr = numbers.split(" ").map(Number);
  return `${Math.max(...arr)} ${Math.min(...arr)}`;
}

export default highAndLow;
