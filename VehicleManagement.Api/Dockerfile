FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

RUN chgrp -R 0 /app && \
    chmod -R g=u /app

RUN apt-get update && \
    apt-get install -y --no-install-recommends curl && \
    rm -rf /var/lib/apt/lists/*

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD curl --fail --silent http://localhost:8080/health || exit 1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["VehicleManagement.Api/VehicleManagement.Api.csproj", "VehicleManagement.Api/"]
COPY ["VehicleManagement.Application/VehicleManagement.Application.csproj", "VehicleManagement.Application/"]
COPY ["VehicleManagement.Domain/VehicleManagement.Domain.csproj", "VehicleManagement.Domain/"]
COPY ["VehicleManagement.Infrastructure/VehicleManagement.Infrastructure.csproj", "VehicleManagement.Infrastructure/"]
RUN dotnet restore "./VehicleManagement.Api/VehicleManagement.Api.csproj"
COPY . .
WORKDIR "/src/VehicleManagement.Api"
RUN dotnet build "./VehicleManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./VehicleManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN chgrp -R 0 /app && \
    chmod -R g=u /app
USER app
ENTRYPOINT ["dotnet", "VehicleManagement.Api.dll"]
