using System.Linq;

namespace Codewars.Training.ReplaceWithAlphabetPosition;

public static class Kata
{
    public static string AlphabetPosition(string text)
    {
        return string.Join(" ", text.ToLower().Where(char.IsLetter).Select(static c => c - 96));
    }
}