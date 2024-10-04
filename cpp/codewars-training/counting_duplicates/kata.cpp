#include "counting_duplicates/kata.hpp"

#include <ranges>
#include <unordered_map>

namespace counting_duplicates
{
    auto duplicateCount(const std::string& in) -> std::size_t
    {
        std::unordered_map<char, int> charCount;
        for (const char c : in)
        {
            char lowerChar = std::tolower(c);
            charCount[lowerChar]++;
        }

        int count = 0;
        for (const auto& value : std::ranges::views::values(charCount))
        {
            if (value > 1)
            {
                count++;
            }
        }

        return count;
    }
}
