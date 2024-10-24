using AspNetSignalIR.ClientListen;
using System.Net.WebSockets;
using System.Text;

namespace AspNetSignalIR.Connectors;

internal class BroadcastClass
{
    public static async Task BroadcastMessageAsync(string message, Client clientid, Dictionary<Client, WebSocket> _clients)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);

        foreach (var client in _clients)
        {
            if (client.Value.State == WebSocketState.Open)
            {
                if (!client.Key.Equals(clientid))
                {
                    try
                    {
                       await client.Value.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ocorreu um erro no BroadcastClass: {ex.Message}");
                    }
                }
            }
        }
    }
}
