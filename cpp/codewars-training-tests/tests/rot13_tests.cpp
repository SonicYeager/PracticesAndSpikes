#include <gtest/gtest.h>
#include "rot13/kata.hpp"

TEST(Rot13Test, SampleTests) {
    EXPECT_EQ(rot13::rot13("test"), "grfg");
    EXPECT_EQ(rot13::rot13("Test"), "Grfg");
    EXPECT_EQ(rot13::rot13("AbCd"), "NoPq");
    EXPECT_EQ(rot13::rot13("Ab/Cd"), "No/Pq");
    EXPECT_EQ(rot13::rot13("Ab4Cd"), "No4Pq");
}
