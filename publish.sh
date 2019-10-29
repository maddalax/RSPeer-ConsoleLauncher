#!/usr/bin/env bash
dotnet publish -c Release --runtime linux-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true
dotnet publish -c Release --runtime linux-arm /p:PublishSingleFile=true /p:PublishTrimmed=true
dotnet publish -c Release --runtime osx-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true
dotnet publish -c Release --runtime win-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true
dotnet publish -c Release --runtime win-x86 /p:PublishSingleFile=true /p:PublishTrimmed=true