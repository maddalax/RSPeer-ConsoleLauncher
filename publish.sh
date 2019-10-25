#!/usr/bin/env bash
dotnet publish -c Release --runtime linux-x64 --self-contained
dotnet publish -c Release --runtime linux-arm --self-contained
dotnet publish -c Release --runtime osx-x64 --self-contained
dotnet publish -c Release --runtime win-x64 --self-contained
dotnet publish -c Release --runtime win-x86 --self-contained
