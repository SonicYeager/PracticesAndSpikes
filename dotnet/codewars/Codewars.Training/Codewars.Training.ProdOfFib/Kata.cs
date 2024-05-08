using System.Collections.Generic;

namespace Codewars.Training.ProdOfFib;

public static class Kata
{
    public static ulong[] ProductFib(ulong prod)
    {
        var previousNumbers = new Stack<ulong>();
        previousNumbers.Push(1);
        previousNumbers.Push(0);

        ulong lastProd;

        do
        {
            var previous = previousNumbers.Pop();
            var fib = previous + previousNumbers.Pop();

            if (previous * fib == prod)
                return
                [
                    previous, fib, 1,
                ];

            lastProd = previous * fib;

            previousNumbers.Push(previous);
            previousNumbers.Push(fib);
        } while (lastProd < prod);

        var lastFib = previousNumbers.Pop();
        var lastPrevious = previousNumbers.Pop();
        return
        [
            lastPrevious, lastFib, 0,
        ];
    }
}