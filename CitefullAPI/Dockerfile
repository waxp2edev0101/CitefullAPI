#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 22

ARG DEBIAN_FRONTEND=noninteractive
RUN apt-get update -qq && \
    apt-get install openssh-server -y && \
    systemctl enable ssh && \
    apt-get install software-properties-common apt-transport-https -y && \
    apt-get install gpg -y && \
    mkdir -p /usr/share/man/man1

RUN add-apt-repository ppa:openjdk-r/ppa -y && \
    apt-get install openjdk-11-jdk -y && \
    useradd -m -s /bin/bash jenkins && \
    echo jenkins:Jenkins | chpasswd

RUN apt-get git -y

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["CitefullAPI.csproj", ""]
RUN dotnet restore "./CitefullAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CitefullAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CitefullAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CitefullAPI.dll"]