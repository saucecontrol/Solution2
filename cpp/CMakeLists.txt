cmake_minimum_required(VERSION 3.20)

project(alpha-bench)

if(CMAKE_SYSTEM_PROCESSOR STREQUAL "aarch64" OR CMAKE_SYSTEM_PROCESSOR STREQUAL "ARM64")
  set(compile_options $<IF:$<CXX_COMPILER_ID:MSVC>,/arch:armv8.2,-march=armv8.2-a+simd -mtune=cortex-x1c>)
else()
  set(compile_options $<IF:$<CXX_COMPILER_ID:MSVC>,/arch:AVX512,-march=native>)
endif()

set(CMAKE_BUILD_TYPE Release)
set(CMAKE_EXPORT_COMPILE_COMMANDS ON)
set(CMAKE_BUILD_RPATH ${CMAKE_BINARY_DIR})

add_executable(alpha-bench main.cpp test.cpp benchmark.cpp)

target_compile_options(alpha-bench PRIVATE ${compile_options})

find_package(benchmark CONFIG REQUIRED)

target_link_libraries(alpha-bench PRIVATE benchmark::benchmark)
