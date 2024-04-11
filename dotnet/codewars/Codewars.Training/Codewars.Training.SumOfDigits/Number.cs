using System.Linq;

namespace Codewars.Training.SumOfDigits;

public static class Number
{
    public static int DigitalRoot(long n)
    {
        var nAsString = n.ToString();
        while (nAsString.Length > 1)
        {
            nAsString = nAsString.Select(static c => int.Parse(c.ToString())).Sum().ToString();
        }

        return int.Parse(nAsString);
    }
}