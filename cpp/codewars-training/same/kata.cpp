#include "same/kata.hpp"

#include <algorithm>

namespace same
{
    auto comp(std::vector<int>& a, std::vector<int>& b) -> bool
    {
        std::vector<int> a_squares;
        a_squares.reserve(a.size());
        for (const auto& i : a)
        {
            a_squares.push_back(i * i);
        }

        std::ranges::sort(a_squares);
        std::ranges::sort(b);
        return a_squares == b;
    }
}
