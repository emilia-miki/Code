cmake_minimum_required(VERSION 3.0.0)
project(asd_lab_4 VERSION 0.1.0)

include(CTest)
enable_testing()

add_executable(asd_lab_4 main.cpp BST.cpp testMergeSplit.cpp
    testAutocompletion.cpp testBinarySearchTree.cpp algorithms.cpp)

set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PROJECT_VERSION ${PROJECT_VERSION})
include(CPack)
