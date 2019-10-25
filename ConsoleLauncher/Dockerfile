FROM centos:7
WORKDIR /app
COPY ./ .
RUN ./bin/Release/netcoreapp3.0/linux-x64/publish/ConsoleLauncher test

FROM centos:8
WORKDIR /app
COPY ./ .
RUN ./bin/Release/netcoreapp3.0/linux-x64/publish/ConsoleLauncher test

FROM ubuntu:18.04
WORKDIR /app
COPY ./ .
RUN ./bin/Release/netcoreapp3.0/linux-x64/publish/ConsoleLauncher test

FROM ubuntu:16.04
WORKDIR /app
COPY ./ .
RUN ./bin/Release/netcoreapp3.0/linux-x64/publish/ConsoleLauncher test

