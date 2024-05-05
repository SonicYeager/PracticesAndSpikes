using System;
using System.Collections.Generic;
using System.Linq;

namespace Codewars.Training.RomanNums;

public static class Kata
{
    private static readonly Dictionary<int, string> RomanDictionary = new Dictionary<int, string>
    {
        {
            1000, "M"
        },
        {
            900, "CM"
        },
        {
            500, "D"
        },
        {
            400, "CD"
        },
        {
            100, "C"
        },
        {
            90, "XC"
        },
        {
            50, "L"
        },
        {
            40, "XL"
        },
        {
            10, "X"
        },
        {
            9, "IX"
        },
        {
            5, "V"
        },
        {
            4, "IV"
        },
        {
            1, "I"
        }
    };

    public static string ToRoman(int n)
    {
        return RomanDictionary
            .Where(dict => n >= dict.Key)
            .Select(dict => dict.Value + ToRoman(n - dict.Key))
            .FirstOrDefault();
    }

    public static int FromRoman(string romanNumeral)
    {
        return
            romanNumeral.Length == 0
                ? 0
                : RomanDictionary
                    .Where(dict => romanNumeral.StartsWith(dict.Value))
                    .Select(dict => dict.Key + FromRoman(romanNumeral[dict.Value.Length..]))
                    .FirstOrDefault();
    }
}