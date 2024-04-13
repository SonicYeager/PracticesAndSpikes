cmake_minimum_required(VERSION 3.15)
project(codewars-training CXX)

# GoogleTest requires at least C++14
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Automatically add all source files and headers
file(GLOB SOURCES 
"src/*.cpp" 
"tests/*.cpp" 
"src/digital_root/*.cpp"
"src/high_and_low/*.cpp"
)
#file(GLOB HEADERS "include/*.hpp")

include_directories(include/)

find_package(GTest REQUIRED)

add_executable(${PROJECT_NAME} ${SOURCES}) # ${HEADERS}
target_link_libraries(${PROJECT_NAME} GTest::GTest)