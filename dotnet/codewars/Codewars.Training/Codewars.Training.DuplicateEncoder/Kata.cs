using System.Linq;

namespace Codewars.Training.DuplicateEncoder;

public static class Kata
{
    public static string DuplicateEncode(string word)
    {
        var lowered = word.ToLower();
        var encoded = lowered.ToLower().Select(f => lowered.Count(c => f == c) > 1 ? ')' : '(');
        return encoded.Aggregate("", static (current, c) => current + c);
    }
}