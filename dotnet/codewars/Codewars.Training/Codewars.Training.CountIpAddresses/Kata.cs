using System.Linq;

namespace Codewars.Training.CountIpAddresses;

public static class Kata
{
    public static long IpsBetween(string start, string end)
    {
        //parse segments
        var startSegments = start.Split('.').Select(int.Parse).ToList();
        var endSegments = end.Split('.').Select(int.Parse).ToList();

        //calc diff at each segment and multiply by difference of segment and power of 2 beginning with ^0
        return startSegments
            .Zip(endSegments, static (s, e) => e - s)
            .Aggregate(0L, static (acc, diff) => (acc << 8) + diff);
    }
}