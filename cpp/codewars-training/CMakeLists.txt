cmake_minimum_required(VERSION 3.28)
project(codewars_training)

set(CMAKE_CXX_STANDARD 23)

add_library(codewars_training STATIC
        digital_root/kata.cpp
        include/digital_root/kata.hpp
        high_and_low/kata.cpp
        include/high_and_low/kata.hpp
        string_ends_with/kata.cpp
        include/string_ends_with/kata.hpp
        include/descending_order/kata.hpp
        descending_order/kata.cpp
        same/kata.cpp
        include/same/kata.hpp
        include/roman_numerals_encoder/kata.hpp
        roman_numerals_encoder/kata.cpp
        include/create_phone_number/kata.hpp
        create_phone_number/kata.cpp
)

# Add the include directory of the static library
target_include_directories(codewars_training PUBLIC include)