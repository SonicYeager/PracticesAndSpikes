using System.Collections.Generic;

namespace Codewars.Training.SquaresIntoSquares;

public static class Kata
{
    public static string? Decompose(long n)
    {
        var result = DecomposeHelper(n, n * n);
        return result == null ? null : string.Join(" ", result);
    }

    private static List<long>? DecomposeHelper(long n, long remaining)
    {
        if (remaining == 0)
            return new List<long>();

        for (var i = n - 1; i > 0; i--)
        {
            var square = i * i;
            if (remaining - square >= 0)
            {
                var result = DecomposeHelper(i, remaining - square);
                if (result != null)
                {
                    result.Add(i);
                    return result;
                }
            }
        }

        return null;
    }
}