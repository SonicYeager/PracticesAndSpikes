using System.Linq;

namespace Codewars.Training.HighestAndLowest;

public static class Kata
{
    public static string HighAndLow(string numbers)
    {
        var split = numbers.Split(' ').Select(int.Parse).Distinct().ToList();
        return $"{split.Max()} {split.Min()}";
    }
}