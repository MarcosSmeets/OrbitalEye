using DotNetEnv;
using HealthChecks.NpgSql;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using SatelliteTracker.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using SatelliteTracker.Infrastructure.Persistence;
using SatelliteTracker.Presentation.BackgroundJobs;
using SatelliteTracker.Presentation.DependencyInjection;
using SatelliteTracker.Presentation.Middleware;
using SatelliteTracker.Presentation.WebSockets;

// Load .env files: .env.development in Development, .env in Production
var envFile = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
    ? ".env.development"
    : ".env";

// Search for .env file in current dir and up to 3 parent directories
var searchDir = Directory.GetCurrentDirectory();
for (var i = 0; i < 4; i++)
{
    var candidate = Path.Combine(searchDir, envFile);
    if (File.Exists(candidate))
    {
        Env.Load(candidate);
        break;
    }
    var parent = Directory.GetParent(searchDir);
    if (parent is null) break;
    searchDir = parent.FullName;
}

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole(options =>
{
    options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
    options.UseUtcTimestamp = true;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Build connection string: DATABASE_URL (Railway) > individual env vars > appsettings
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

if (!string.IsNullOrEmpty(databaseUrl))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
}
else
{
    var dbHost = Environment.GetEnvironmentVariable("DATABASE_HOST") ?? "localhost";
    var dbPort = Environment.GetEnvironmentVariable("DATABASE_PORT") ?? "5432";
    var dbName = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? "satellite_tracker";
    var dbUser = Environment.GetEnvironmentVariable("DATABASE_USER") ?? "postgres";
    var dbPassword = Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "postgres";

    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";
}

// Respect PORT env var for Railway dynamic port assignment
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://+:{port}");
}

// Override SpaceTrack config from env vars
var spaceTrackIdentity = Environment.GetEnvironmentVariable("SPACETRACK_IDENTITY");
var spaceTrackPassword = Environment.GetEnvironmentVariable("SPACETRACK_PASSWORD");
if (!string.IsNullOrEmpty(spaceTrackIdentity))
{
    builder.Configuration["SpaceTrack:Identity"] = spaceTrackIdentity;
    builder.Configuration["SpaceTrack:Password"] = spaceTrackPassword ?? "";
}

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString, builder.Configuration);

var corsOrigins = Environment.GetEnvironmentVariable("CORS_ORIGINS");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (!string.IsNullOrEmpty(corsOrigins))
        {
            policy.WithOrigins(corsOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
        else
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    });
});

builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "postgresql", tags: new[] { "db", "ready" });

builder.Services.AddHostedService<TleUpdateJob>();
builder.Services.AddHostedService<TelemetryCleanupJob>();
builder.Services.AddHostedService<OrbitPropagationJob>();
builder.Services.AddHostedService<SatelliteDiscoveryJob>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SatelliteTrackerDbContext>();
    await dbContext.Database.MigrateAsync();
    await DatabaseSeeder.SeedAsync(dbContext);
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseWebSockets();
app.UseCors();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapHealthChecks("/health");
app.Map("/ws/satellites", async (HttpContext context, TelemetryBroadcaster broadcaster) =>
    await WebSocketHandler.HandleWebSocketAsync(context, broadcaster));

app.Run();

public partial class Program { }
