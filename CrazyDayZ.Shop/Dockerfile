FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CrazyDayZ.Shop/CrazyDayZ.Shop.csproj", "CrazyDayZ.Shop/"]
RUN dotnet restore "./CrazyDayZ.Shop/CrazyDayZ.Shop.csproj"
COPY CrazyDayZ.Shop/. CrazyDayZ.Shop/
WORKDIR "/src/CrazyDayZ.Shop"
RUN dotnet build "./CrazyDayZ.Shop.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CrazyDayZ.Shop.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER root
ENTRYPOINT ["dotnet", "CrazyDayZ.Shop.dll"]