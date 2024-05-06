#include "descending_order/kata.hpp"

#include <algorithm>
#include <string>

namespace descending_order
{
    auto descendingOrder(const uint64_t a) -> uint64_t
    {
        auto digits = std::to_string(a);
        std::sort(digits.begin(), digits.end(), std::greater<>());
        return std::stoull(digits);
    }
}
