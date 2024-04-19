using System.Linq;
using System.Text.RegularExpressions;

namespace Codewars.Training.ReverseWords;

public static class Kata
{
    public static string ReverseWords(string str)
    {
        var matches = Regex.Matches(str, @"[^ ]+|\s+");
        var reversed = matches.Select(static m => string.IsNullOrWhiteSpace(m.Value) ? m.Value : new(m.Value.Reverse().ToArray()));
        return string.Join("", reversed);
    }
}