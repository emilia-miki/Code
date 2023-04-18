cmake_minimum_required(VERSION 3.0.0)
project(kursach1 VERSION 0.1.0)

include(CTest)
enable_testing()

include_directories("include")
add_executable(kursach1 main.cpp treesort.cpp search.cpp heapsort.cpp huffman_coding.cpp
      priority_queue.cpp)
target_compile_features(kursach1 PRIVATE cxx_std_17)


set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PROJECT_VERSION ${PROJECT_VERSION})
include(CPack)
