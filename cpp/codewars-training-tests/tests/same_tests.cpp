#include <gtest/gtest.h>
#include "same/kata.hpp"

using namespace same;

TEST(Comp_Tests, Tests_comp)
{
    std::vector a = {121, 144, 19, 161, 19, 144, 19, 11};
    std::vector b = {14641, 20736, 361, 25921, 361, 20736, 361, 121};
    EXPECT_EQ(comp(a, b), true);
    a = {121, 144, 19, 161, 19, 144, 19, 11};
    b = {14641, 20736, 361, 25921, 361, 20736, 362, 121};
    EXPECT_EQ(comp(a, b), false);
}