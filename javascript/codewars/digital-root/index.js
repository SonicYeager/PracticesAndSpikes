function digitalRoot(n) {
    const digits = n.toString().split('').map(Number);

    const sum = digits.reduce((acc, digit) => acc + digit, 0);

    if (sum < 10) {
        return sum;
    }

    return digitalRoot(sum);
}

export default digitalRoot;