namespace Codewars.Training.MergedStringChecker;

public static class Kata
{
    public static bool IsMerge(string s, string part1, string part2)
    {
        if (string.IsNullOrEmpty(s))
            return string.IsNullOrEmpty(part1) && string.IsNullOrEmpty(part2);

        var match1 = part1.Length > 0 && s[0] == part1[0] && IsMerge(s[1..], part1[1..], part2);
        var match2 = part2.Length > 0 && s[0] == part2[0] && IsMerge(s[1..], part1, part2[1..]);

        return match1 || match2;
    }
}