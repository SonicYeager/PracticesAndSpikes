#include <gtest/gtest.h>
#include "high_and_low/kata.hpp"

using namespace high_and_low;

TEST(ExampleTests, Test1)
{
    EXPECT_EQ(get_high_and_low("8 3 -5 42 -1 0 0 -9 4 7 4 -4"), "42 -9");
}

TEST(ExampleTests, Test2)
{
    EXPECT_EQ(get_high_and_low("1 2 3"), "3 1");
}