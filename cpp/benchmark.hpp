#pragma once

#include <benchmark/benchmark.h>

namespace pma {
    void v1_plain(benchmark::State& state) noexcept;
    void v2_plain(benchmark::State& state) noexcept;
    void cs_plain(benchmark::State& state) noexcept;

    void v1_simd(benchmark::State& state) noexcept;
    void v3_simd(benchmark::State& state) noexcept;
    void v4_simd(benchmark::State& state) noexcept;
    void v5_simd(benchmark::State& state) noexcept;
    void v6_simd(benchmark::State& state) noexcept;
    void cs_simd(benchmark::State& state) noexcept;
}
