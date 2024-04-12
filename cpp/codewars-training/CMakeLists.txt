cmake_minimum_required(VERSION 3.15)
project(codewars-training CXX)

# GoogleTest requires at least C++14
set(CMAKE_CXX_STANDARD 14)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

find_package(GTest REQUIRED)

add_executable(${PROJECT_NAME} src/main.cpp tests/test_dummy.cpp)
target_link_libraries(${PROJECT_NAME} GTest::GTest)