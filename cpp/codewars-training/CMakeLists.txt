cmake_minimum_required(VERSION 3.15)
project(codewars-training CXX)

find_package(ZLIB REQUIRED)
find_package(GTest REQUIRED)

add_executable(${PROJECT_NAME} src/main.cpp)
target_link_libraries(${PROJECT_NAME} ZLIB::ZLIB)
target_link_libraries(${PROJECT_NAME} gtest::gtest)