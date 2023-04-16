cmake_minimum_required(VERSION 3.0.0)
project(prog_lab_14 VERSION 0.1.0)

include(CTest)
enable_testing()

add_executable(prog_lab_14 main.cpp algorithms.cpp data.cpp queries.cpp file_io.cpp)

set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PROJECT_VERSION ${PROJECT_VERSION})
include(CPack)
