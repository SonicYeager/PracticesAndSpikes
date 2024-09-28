#include <gtest/gtest.h>
#include <array>
#include "create_phone_number/kata.hpp"

using namespace create_phone_number;

TEST(CreatePhoneNumber, BasicTests) {
    EXPECT_EQ(createPhoneNumber(std::array{1, 2, 3, 4, 5, 6, 7, 8, 9, 0}.data()), "(123) 456-7890");
    EXPECT_EQ(createPhoneNumber(std::array{1, 1, 1, 1, 1, 1, 1, 1, 1, 1}.data()), "(111) 111-1111");
    EXPECT_EQ(createPhoneNumber(std::array{1, 2, 3, 4, 5, 6, 8, 8, 0, 0}.data()), "(123) 456-8800");
    EXPECT_EQ(createPhoneNumber(std::array{0, 2, 3, 0, 5, 6, 0, 8, 9, 0}.data()), "(023) 056-0890");
    EXPECT_EQ(createPhoneNumber(std::array{0, 0, 0, 0, 0, 0, 0, 0, 0, 0}.data()), "(000) 000-0000");
}