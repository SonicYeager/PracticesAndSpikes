#include <gtest/gtest.h>
#include <cinttypes>
#include "descending_order/kata.hpp"

TEST(testDescendingOrder, should_work_on_several_examples) {
    EXPECT_EQ(descending_order::descendingOrder(0), 0);
    EXPECT_EQ(descending_order::descendingOrder(1), 1);
    EXPECT_EQ(descending_order::descendingOrder(15), 51);
    EXPECT_EQ(descending_order::descendingOrder(1021), 2110);
    EXPECT_EQ(descending_order::descendingOrder(123456789), 987654321);
}