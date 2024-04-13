#include "digital_root/kata.hpp"
#include <gtest/gtest.h>
#include <ctime>

using namespace digital_root;

TEST(FixedTests, DigitalRoot)
{
    EXPECT_EQ(get_digital_root(16), 7);
    EXPECT_EQ(get_digital_root(195), 6);
    EXPECT_EQ(get_digital_root(992), 2);
    EXPECT_EQ(get_digital_root(167346), 9);
    EXPECT_EQ(get_digital_root(0), 0);
    EXPECT_EQ(get_digital_root(10), 1);
}

TEST(RandomTests, DigitalRoot)
{
    srand(time(NULL));
    for(int i = 3; i < 103; i++)
    {
        int C = std::rand();
        EXPECT_EQ(get_digital_root(C), (C-1) % 9 + 1);
    }
}