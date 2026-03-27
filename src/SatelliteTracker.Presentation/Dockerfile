FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY SatelliteTracker.sln ./
COPY src/SatelliteTracker.Domain/SatelliteTracker.Domain.csproj src/SatelliteTracker.Domain/
COPY src/SatelliteTracker.Application/SatelliteTracker.Application.csproj src/SatelliteTracker.Application/
COPY src/SatelliteTracker.Infrastructure/SatelliteTracker.Infrastructure.csproj src/SatelliteTracker.Infrastructure/
COPY src/SatelliteTracker.Presentation/SatelliteTracker.Presentation.csproj src/SatelliteTracker.Presentation/
COPY tests/SatelliteTracker.Domain.Tests/SatelliteTracker.Domain.Tests.csproj tests/SatelliteTracker.Domain.Tests/
COPY tests/SatelliteTracker.Application.Tests/SatelliteTracker.Application.Tests.csproj tests/SatelliteTracker.Application.Tests/
COPY tests/SatelliteTracker.Integration.Tests/SatelliteTracker.Integration.Tests.csproj tests/SatelliteTracker.Integration.Tests/

RUN dotnet restore

COPY src/ src/
COPY tests/ tests/

RUN dotnet publish src/SatelliteTracker.Presentation/SatelliteTracker.Presentation.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "SatelliteTracker.Presentation.dll"]
