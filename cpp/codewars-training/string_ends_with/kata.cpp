#include "string_ends_with/kata.hpp"

namespace string_ends_with
{
    auto solution(std::string const& str, std::string const& ending) -> bool
    {
        if (ending.size() > str.size()) return false;
        return std::equal(ending.rbegin(), ending.rend(), str.rbegin());
    }
}
