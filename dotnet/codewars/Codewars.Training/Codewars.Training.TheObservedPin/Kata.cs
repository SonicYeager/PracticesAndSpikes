using System.Collections.Generic;
using System.Linq;

namespace Codewars.Training.TheObservedPin;

public static class Kata
{
    private static readonly Dictionary<char, char[]> AdjacentDigits = new()
    {
        {
            '1', [
                '1', '2', '4',
            ]
        },
        {
            '2', [
                '1', '2', '3', '5',
            ]
        },
        {
            '3', [
                '2', '3', '6',
            ]
        },
        {
            '4', [
                '1', '4', '5', '7',
            ]
        },
        {
            '5', [
                '2', '4', '5', '6', '8',
            ]
        },
        {
            '6', [
                '3', '5', '6', '9',
            ]
        },
        {
            '7', [
                '4', '7', '8',
            ]
        },
        {
            '8', [
                '5', '7', '8', '9', '0',
            ]
        },
        {
            '9', [
                '6', '8', '9',
            ]
        },
        {
            '0', [
                '0', '8',
            ]
        },
    };

    public static List<string> GetPINs(string observed)
    {
        var result = new List<string>
        {
            "",
        };

        foreach (var newResult in observed.Select(static digit => AdjacentDigits[digit]).Select(adjacent
                     => (from prefix in result from suffix in adjacent select prefix + suffix).ToList<string>()))
        {
            result = newResult;
        }

        return result;
    }
}