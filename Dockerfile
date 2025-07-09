# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore as distinct layers
COPY ["SEP_MMB_API/SEP_MMB_API.csproj", "SEP_MMB_API/"]
COPY ["Services/Services.csproj", "Services/"]
COPY ["DataAccessLayers/DataAccessLayers.csproj", "DataAccessLayers/"]
COPY ["BusinessObjects/BusinessObjects.csproj", "BusinessObjects/"]

RUN dotnet restore "SEP_MMB_API/SEP_MMB_API.csproj"

# Copy the rest of the code
COPY . .

# Build project
WORKDIR "/src/SEP_MMB_API"
RUN dotnet build "SEP_MMB_API.csproj" -c Release -o /app/build

# Publish project
FROM build AS publish
RUN dotnet publish "SEP_MMB_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "SEP_MMB_API.dll"]