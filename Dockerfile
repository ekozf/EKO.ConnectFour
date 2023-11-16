#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV DOTNET_URLS=http://+:5001

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
# COPY ["EKO.ConnectFour/EKO.ConnectFour.csproj", "EKO.ConnectFour/"]
RUN dotnet restore "./EKO.ConnectFour/EKO.ConnectFour.Api.csproj"
COPY . .
WORKDIR "/src/EKO.ConnectFour"
RUN dotnet build "./EKO.ConnectFour.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EKO.ConnectFour.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EKO.ConnectFour.Api.dll"]