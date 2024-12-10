# syntax=docker/dockerfile:1

# Create a stage for building the application.
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

# Set the working directory
WORKDIR /source

# Copy the project file and restore dependencies
COPY ["CrazyDayZ.Shop/CrazyDayZ.Shop.csproj", "CrazyDayZ.Shop/"]
RUN dotnet restore "CrazyDayZ.Shop/CrazyDayZ.Shop.csproj"

# Copy the rest of the source code
COPY . .

# Set the working directory to the project folder
WORKDIR /source/CrazyDayZ.Shop

# Build the application
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish -c Release -o /app --self-contained false

# Final stage to create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Copy everything needed to run the app from the "build" stage.
COPY --from=build /app .

# Set the user and entry point
USER $APP_UID
ENTRYPOINT ["dotnet", "CrazyDayZ.Shop.dll"]
