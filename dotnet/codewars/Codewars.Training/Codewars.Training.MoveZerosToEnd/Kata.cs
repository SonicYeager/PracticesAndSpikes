using System.Linq;

namespace Codewars.Training.MoveZerosToEnd;

public static class Kata
{
    public static int[] MoveZeroes(int[] arr)
    {
        var nonZeroes = arr.Where(static x => x != 0).ToArray();
        var zeroes = arr.Where(static x => x == 0).ToArray();
        return nonZeroes.Concat(zeroes).ToArray();
    }
}