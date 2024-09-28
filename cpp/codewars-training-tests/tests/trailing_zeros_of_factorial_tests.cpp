#include <gtest/gtest.h>
#include "trailing_zeros_of_factorial/kata.hpp"

using namespace trailing_zeros_of_factorial;

TEST(SampleTests, Tests) {
    EXPECT_EQ(zeros(0), 0);
    EXPECT_EQ(zeros(6), 1);
    EXPECT_EQ(zeros(30), 7);
    EXPECT_EQ(zeros(100), 24);
    EXPECT_EQ(zeros(1000), 249);
    EXPECT_EQ(zeros(100000), 24999);
    EXPECT_EQ(zeros(1000000000), 249999998);
}