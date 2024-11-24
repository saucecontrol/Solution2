#! /usr/bin/bash

rid=linux-x64
arch=`uname -p`
if [[ $arch=="aarch64" ]]; then
    rid=linux-arm64
fi

cmake --preset=clang-19
cmake --build clang-19
~/.dotnet/dotnet publish ../dotnet/lib -r $rid -o clang-19

cmake --preset=gcc-14
cmake --build gcc-14
~/.dotnet/dotnet publish ../dotnet/lib -r $rid -o gcc-14
