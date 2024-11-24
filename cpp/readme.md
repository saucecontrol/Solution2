This project was originally copied from https://github.com/Wizermil/premultiply_alpha

It was modified to support more compilers (MSVC and GCC in addition to Clang) and to add benchmarks for calls to a .NET library.

Build setup requires [vcpkg](https://github.com/microsoft/vcpkg) for the [Google Benchmark](https://github.com/google/benchmark) dependency.  Set the path to your local clone in CMakeUserPresets.json
