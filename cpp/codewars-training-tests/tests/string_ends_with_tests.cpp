#include <gtest/gtest.h>
#include "string_ends_with/kata.hpp"

TEST(Tests, Sample_Test_Cases)
{
    EXPECT_EQ(string_ends_with::solution("abcde", "cde"), true);
    EXPECT_EQ(string_ends_with::solution("abcde", "abc"), false);
    EXPECT_EQ(string_ends_with::solution("abc", ""), true);
    EXPECT_EQ(string_ends_with::solution("abc", "abcd"), false);
}
