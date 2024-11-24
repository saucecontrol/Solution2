@echo off
if "%PROCESSOR_ARCHITECTURE%"=="ARM64" (set rid=win-arm64) else (set rid=win-x64)

cmake --preset=default
cmake --build msvc
dotnet publish ..\dotnet\lib -r %rid% -o msvc
