cmake_minimum_required(VERSION 3.28)
project(codewars_training_tests)

set(CMAKE_CXX_STANDARD 17)

find_package(GTest)

# Add the directory containing the static library's CMakeLists.txt
add_subdirectory(../codewars-training ${CMAKE_BINARY_DIR}/codewars-training)

add_executable(codewars_training_tests
        main.cpp
        tests/digital_root_tests.cpp
        tests/high_and_low_tests.cpp
        tests/string_ends_with_tests.cpp
        tests/descending_order_tests.cpp
        tests/same_tests.cpp
)

# Add the include directory of the static library
target_include_directories(codewars_training_tests PRIVATE ../codewars-training/include)

target_link_libraries(codewars_training_tests gtest::gtest codewars_training)