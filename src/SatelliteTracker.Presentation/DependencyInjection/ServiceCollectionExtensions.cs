using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SatelliteTracker.Application.Interfaces;
using SatelliteTracker.Application.UseCases;
using SatelliteTracker.Domain.Interfaces;
using SatelliteTracker.Infrastructure.External;
using SatelliteTracker.Infrastructure.Messaging;
using SatelliteTracker.Infrastructure.Persistence;
using SatelliteTracker.Infrastructure.Repositories;

namespace SatelliteTracker.Presentation.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<CreateSatelliteUseCase>();
        services.AddTransient<GetSatelliteUseCase>();
        services.AddTransient<ListSatellitesUseCase>();
        services.AddTransient<UpdateSatelliteUseCase>();
        services.AddTransient<GetOrbitUseCase>();
        services.AddTransient<UpdateOrbitFromTleUseCase>();
        services.AddTransient<IngestTelemetryUseCase>();
        services.AddTransient<GetTelemetryHistoryUseCase>();

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<SatelliteTrackerDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ISatelliteRepository, SatelliteRepository>();
        services.AddScoped<IOrbitRepository, OrbitRepository>();
        services.AddScoped<ITelemetryRepository, TelemetryRepository>();
        services.AddScoped<IGroundStationRepository, GroundStationRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SatelliteTrackerDbContext>());

        services.AddSingleton<TelemetryBroadcaster>();
        services.AddSingleton<ITelemetryBroadcaster>(sp => sp.GetRequiredService<TelemetryBroadcaster>());

        services.AddHttpClient<CelestrakClient>();
        services.AddSingleton<ITleProvider>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            return new CelestrakClient(factory.CreateClient(nameof(CelestrakClient)));
        });

        return services;
    }
}
