#include "benchmark.hpp"

#include "cat.hpp"
#include "premultiply_alpha.hpp"
#if defined(_MSC_VER)
#define aligned_alloc(align, size) _aligned_malloc(size, align)
#define free(ptr) _aligned_free(ptr)
#endif

static constexpr std::size_t const max_pixel = cat::width * cat::height;

static std::uint32_t* setup() noexcept {
    auto data = aligned_alloc(64, max_pixel * sizeof(std::uint32_t));
    std::memcpy(data, cat::image, max_pixel * sizeof(std::uint32_t));
    return reinterpret_cast<std::uint32_t*>(data);
}

namespace pma {

    void v1_plain(benchmark::State& state) noexcept {
        auto data = setup();
        state.SetLabel("plain");
        for (auto _ : state) {
            v1::premultiply_alpha_plain(data, max_pixel);
        }
        state.counters["itr"] = benchmark::Counter(state.iterations(), benchmark::Counter::kIsRate);
        free(data);
    }

    void v2_plain(benchmark::State& state) noexcept {
        auto data = setup();
        state.SetLabel("Dot and Beached (discord #include)");
        for (auto _ : state) {
            v2::premultiply_alpha_plain(data, max_pixel);
        }
        state.counters["itr"] = benchmark::Counter(state.iterations(), benchmark::Counter::kIsRate);
        free(data);
    }

    void v1_simd(benchmark::State& state) noexcept {
        auto data = setup();
        state.SetLabel("simd");
        for (auto _ : state) {
            v1::premultiply_alpha_simd(data, max_pixel);
        }
        state.counters["itr"] = benchmark::Counter(state.iterations(), benchmark::Counter::kIsRate);
        free(data);
    }

    void v3_simd(benchmark::State& state) noexcept {
        auto data = setup();
        state.SetLabel("Peter Cordes (stackoverflow)");
        for (auto _ : state) {
            v3::premultiply_alpha_simd(data, max_pixel);
        }
        state.counters["itr"] = benchmark::Counter(state.iterations(), benchmark::Counter::kIsRate);
        free(data);
    }

    void v4_simd(benchmark::State& state) noexcept {
        auto data = setup();
        state.SetLabel("chtz (stackoverflow)");
        for (auto _ : state) {
            v4::premultiply_alpha_simd(data, max_pixel);
        }
        state.counters["itr"] = benchmark::Counter(state.iterations(), benchmark::Counter::kIsRate);
        free(data);
    }

    void v5_simd(benchmark::State& state) noexcept {
        auto data = setup();
        state.SetLabel("avx2");
        for (auto _ : state) {
            v5::premultiply_alpha_simd(data, max_pixel);
        }
        state.counters["itr"] = benchmark::Counter(state.iterations(), benchmark::Counter::kIsRate);
        free(data);
    }

    void v6_simd(benchmark::State& state) noexcept {
        auto data = setup();
        state.SetLabel("ext_vector_type");
        for (auto _ : state) {
            v6::premultiply_alpha_simd(data, max_pixel);
        }
        state.counters["itr"] = benchmark::Counter(state.iterations(), benchmark::Counter::kIsRate);
        free(data);
    }

    void cs_plain(benchmark::State& state) noexcept {
        auto data = setup();
        auto handle = libLoad(libPath);
        convFunc dotnetConv = (convFunc)symLoad(handle, "PremultiplyScalar");
        state.SetLabel("C# plain");
        for (auto _ : state) {
            dotnetConv(data, max_pixel);
        }
        state.counters["itr"] = benchmark::Counter(state.iterations(), benchmark::Counter::kIsRate);
        free(data);
    }

    void cs_simd(benchmark::State& state) noexcept {
        auto data = setup();
        auto handle = libLoad(libPath);
        convFunc dotnetConv = (convFunc)symLoad(handle, "PremultiplySimd");
        state.SetLabel("C# simd");
        for (auto _ : state) {
            dotnetConv(data, max_pixel);
        }
        state.counters["itr"] = benchmark::Counter(state.iterations(), benchmark::Counter::kIsRate);
        free(data);
    }
}
