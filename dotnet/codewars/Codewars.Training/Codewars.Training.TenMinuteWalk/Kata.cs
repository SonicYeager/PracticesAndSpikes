using System.Linq;

namespace Codewars.Training.TenMinuteWalk;

public static class Kata
{
    public static bool IsValidWalk(string[] walk)
    {
        if (walk.Length != 10) return false;

        return walk
                .Select(static w =>
                {
                    return w switch
                    {
                        "n" => (0, 1),
                        "s" => (0, -1),
                        "e" => (1, 0),
                        "w" => (-1, 0),
                        _ => (0, 0),
                    };
                })
                .Aggregate(
                    (0, 0),
                    static (aggregated, elem) => (aggregated.Item1 + elem.Item1, aggregated.Item2 + elem.Item2))
            is (0, 0);
    }
}