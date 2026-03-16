using DotNetEnv;
using SatelliteTracker.Infrastructure.Messaging;
using SatelliteTracker.Presentation.BackgroundJobs;
using SatelliteTracker.Presentation.DependencyInjection;
using SatelliteTracker.Presentation.Middleware;
using SatelliteTracker.Presentation.WebSockets;

// Load .env files: .env.development in Development, .env in Production
var envFile = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
    ? ".env.development"
    : ".env";

var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", envFile);
if (File.Exists(envPath))
    Env.Load(envPath);
else if (File.Exists(envFile))
    Env.Load(envFile);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Build connection string from env vars, fallback to appsettings
var dbHost = Environment.GetEnvironmentVariable("DATABASE_HOST") ?? "localhost";
var dbPort = Environment.GetEnvironmentVariable("DATABASE_PORT") ?? "5432";
var dbName = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? "satellite_tracker";
var dbUser = Environment.GetEnvironmentVariable("DATABASE_USER") ?? "postgres";
var dbPassword = Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "postgres";

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddHealthChecks();

builder.Services.AddHostedService<TleUpdateJob>();
builder.Services.AddHostedService<TelemetryCleanupJob>();
builder.Services.AddHostedService<OrbitPropagationJob>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWebSockets();
app.UseCors();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();
app.MapHealthChecks("/health");
app.Map("/ws/satellites", async (HttpContext context, TelemetryBroadcaster broadcaster) =>
    await WebSocketHandler.HandleWebSocketAsync(context, broadcaster));

app.Run();

public partial class Program { }
