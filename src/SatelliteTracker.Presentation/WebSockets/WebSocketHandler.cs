using System.Net.WebSockets;
using SatelliteTracker.Infrastructure.Messaging;

namespace SatelliteTracker.Presentation.WebSockets;

public static class WebSocketHandler
{
    public static async Task HandleWebSocketAsync(HttpContext context, TelemetryBroadcaster broadcaster)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = 400;
            return;
        }

        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var clientId = Guid.NewGuid().ToString();

        broadcaster.AddClient(clientId, webSocket);

        try
        {
            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
            }
        }
        finally
        {
            broadcaster.RemoveClient(clientId);
        }
    }
}
