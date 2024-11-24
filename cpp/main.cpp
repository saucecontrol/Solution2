#include <benchmark/benchmark.h>

#include "benchmark.hpp"
#include "test.hpp"
#include "premultiply_alpha.hpp"

namespace pma {
    BENCHMARK(v1_plain);
    BENCHMARK(v2_plain);
    BENCHMARK(cs_plain);

#if !defined(NSIMD)
#if defined(__i386__) || defined(__x86_64__)
    BENCHMARK(v1_simd);
    BENCHMARK(v3_simd);
    BENCHMARK(v4_simd);
    BENCHMARK(v5_simd);
#endif
#if defined(__clang__)
    BENCHMARK(v6_simd);
#endif
#endif
    BENCHMARK(cs_simd);
}

int main(int argc, char** argv) {
    ::benchmark::Initialize(&argc, argv);
    if (::benchmark::ReportUnrecognizedArguments(argc, argv))
        return 1;

    auto handle = libLoad(libPath);
    if (!handle)
    {
        std::cout << "dotnet library load failed: " << errLoad() << std::endl;
        return 1;
    }

    CHECK(&v1::premultiply_alpha_plain)
    CHECK(&v2::premultiply_alpha_plain)
    CHECK((convFunc)symLoad(handle, "PremultiplyScalar"))

#if !defined(NSIMD)
    CHECK(&v1::premultiply_alpha_simd)
    CHECK(&v3::premultiply_alpha_simd)
    CHECK(&v4::premultiply_alpha_simd)
    CHECK(&v5::premultiply_alpha_simd)
    CHECK(&v6::premultiply_alpha_simd)
    CHECK((convFunc)symLoad(handle, "PremultiplySimd"))
#endif

    ::benchmark::RunSpecifiedBenchmarks();
}
