#include <gtest/gtest.h>
#include "counting_duplicates/kata.hpp"

using namespace counting_duplicates;

TEST(SampleTestCases, ShouldPassBasicFixedTests) {
    EXPECT_EQ(duplicateCount("asdfghjkl54"), 0) << R"(Incorrect output for input = "asdfghjkl54")";
    EXPECT_EQ(duplicateCount("abcdeaa"), 1) << R"(Incorrect output for input = "abcdeaa")";
    EXPECT_EQ(duplicateCount("93917949902"), 1) << R"(Incorrect output for input = "93917949902")";
    EXPECT_EQ(duplicateCount("hhhhhhHHhhHHHHhhhhhHhH"), 1) << R"(Incorrect output for input = "hhhhhhHHhhHHHHhhhhhHhH")";
    EXPECT_EQ(duplicateCount("asdfghjkl55"), 1) << R"(Incorrect output for input = "asdfghjkl55")";
    EXPECT_EQ(duplicateCount("aabbcde"), 2) << R"(Incorrect output for input = "aabbcde")";
    EXPECT_EQ(duplicateCount("aabBcde"), 2) << R"(Incorrect output for input = "aabBcde")";
    EXPECT_EQ(duplicateCount("abcdeaB"), 2) << R"(Incorrect output for input = "abcdeaB")";
    EXPECT_EQ(duplicateCount("0"), 0) << R"(Incorrect output for input = "0")";
    EXPECT_EQ(duplicateCount("000000000112"), 2) << R"(Incorrect output for input = "000000000112")";
    EXPECT_EQ(duplicateCount("Indivisibility"), 1) << R"(Incorrect output for input = "Indivisibility")";
    EXPECT_EQ(duplicateCount("Indivisibilities"), 2) << R"(Incorrect output for input = "Indivisibilities")";
}