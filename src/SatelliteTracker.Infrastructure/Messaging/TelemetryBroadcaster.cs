using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using SatelliteTracker.Application.DTOs;
using SatelliteTracker.Application.Interfaces;

namespace SatelliteTracker.Infrastructure.Messaging;

public class TelemetryBroadcaster : ITelemetryBroadcaster
{
    private readonly ConcurrentDictionary<string, WebSocket> _clients = new();

    public void AddClient(string clientId, WebSocket webSocket)
    {
        _clients.TryAdd(clientId, webSocket);
    }

    public void RemoveClient(string clientId)
    {
        _clients.TryRemove(clientId, out _);
    }

    public async Task BroadcastPositionUpdateAsync(Guid satelliteId, double latitude, double longitude, double altitude, CancellationToken cancellationToken = default)
    {
        var message = new
        {
            @event = "satellite.position.update",
            data = new { satelliteId, latitude, longitude, altitude, timestamp = DateTime.UtcNow }
        };

        await BroadcastAsync(message, cancellationToken);
    }

    public async Task BroadcastTelemetryUpdateAsync(Guid satelliteId, TelemetryDto telemetry, CancellationToken cancellationToken = default)
    {
        var message = new
        {
            @event = "satellite.telemetry.update",
            data = new { satelliteId, telemetry }
        };

        await BroadcastAsync(message, cancellationToken);
    }

    private async Task BroadcastAsync(object message, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(message, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var bytes = Encoding.UTF8.GetBytes(json);
        var segment = new ArraySegment<byte>(bytes);

        var deadClients = new List<string>();

        foreach (var (clientId, socket) in _clients)
        {
            if (socket.State == WebSocketState.Open)
            {
                try
                {
                    await socket.SendAsync(segment, WebSocketMessageType.Text, true, cancellationToken);
                }
                catch
                {
                    deadClients.Add(clientId);
                }
            }
            else
            {
                deadClients.Add(clientId);
            }
        }

        foreach (var clientId in deadClients)
        {
            _clients.TryRemove(clientId, out _);
        }
    }
}
