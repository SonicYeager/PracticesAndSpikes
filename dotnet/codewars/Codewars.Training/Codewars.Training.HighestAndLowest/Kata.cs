using System.Linq;

namespace Codewars.Training.HighestAndLowest;

public static class Kata
{
    /// <summary>
    /// Finds the highest and lowest numbers in a string of space-separated numbers.
    /// </summary>
    /// <param name="numbers">The input string of space-separated numbers.</param>
    /// <returns>A string containing the highest and lowest numbers, separated by a space.</returns>
    public static string HighAndLow(string numbers)
    {
        var split = numbers.Split(' ').Select(int.Parse).Distinct().ToList();
        return $"{split.Max()} {split.Min()}";
    }
}