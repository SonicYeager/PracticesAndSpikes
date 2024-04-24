using System.Linq;

namespace Codewars.Training.WhichAreIn;

public static class Kata
{
    public static string[] InArray(string[] array1, string[] array2)
    {
        var subStrings = array1
            .Where(arr1 => array2.Any(arr2 => arr2.Contains(arr1)))
            .OrderBy(static s => s)
            .ToHashSet();
        return subStrings.ToArray();
    }
}