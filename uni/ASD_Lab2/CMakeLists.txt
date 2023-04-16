cmake_minimum_required(VERSION 3.0.0)
project(asd_lab_2 VERSION 0.1.0)

include(CTest)
enable_testing()

add_executable(asd_lab_2 main.cpp algorithms.cpp linked_list.cpp
    hash_table.cpp test_hash_table.cpp s_hash_table.cpp s_linked_list.cpp
    o_hash_table.cpp test_o_hash_table.cpp)

set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PROJECT_VERSION ${PROJECT_VERSION})
include(CPack)
