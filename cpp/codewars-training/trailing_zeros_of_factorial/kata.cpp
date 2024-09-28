#include "trailing_zeros_of_factorial/kata.hpp"

namespace trailing_zeros_of_factorial
{
    auto zeros(const long n) -> long
    {
        long result = 0;
        for (long i = 5; n / i >= 1; i *= 5)
        {
            result += n / i;
        }
        return result;
    }
}