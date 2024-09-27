#include "roman_numerals_encoder/kata.hpp"

#include <map>

namespace roman_numerals_encoder
{
    struct descending_comparator {
        bool operator()(const int lhs, const int rhs) const {
            return lhs > rhs;
        }
    };

    const std::map<int, std::string, descending_comparator> RomanDictionary = {
        {1000, "M"},
        {900, "CM"},
        {500, "D"},
        {400, "CD"},
        {100, "C"},
        {90, "XC"},
        {50, "L"},
        {40, "XL"},
        {10, "X"},
        {9, "IX"},
        {5, "V"},
        {4, "IV"},
        {1, "I"}
    };

    auto solution(const int number) -> std::string
    {
        for (const auto& [key, value] : RomanDictionary)
        {
            if (number >= key)
            {
                return value + solution(number - key);
            }
        }
        return "";
    }
}
