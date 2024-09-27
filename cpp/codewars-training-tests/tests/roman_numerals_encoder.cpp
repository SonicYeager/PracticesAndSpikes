#include <gtest/gtest.h>
#include "roman_numerals_encoder//kata.hpp"

using namespace roman_numerals_encoder;

TEST(Kata, Fixed_Test) {
    EXPECT_EQ(solution(182), "CLXXXII");
    EXPECT_EQ(solution(1990), "MCMXC");
    EXPECT_EQ(solution(1875), "MDCCCLXXV");
}