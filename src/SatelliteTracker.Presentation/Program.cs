using SatelliteTracker.Infrastructure.Messaging;
using SatelliteTracker.Presentation.BackgroundJobs;
using SatelliteTracker.Presentation.DependencyInjection;
using SatelliteTracker.Presentation.Middleware;
using SatelliteTracker.Presentation.WebSockets;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Database=satellite_tracker;Username=postgres;Password=postgres";

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

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
