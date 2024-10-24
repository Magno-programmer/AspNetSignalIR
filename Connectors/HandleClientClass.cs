﻿using AspNetSignalIR.ClientListen;
using System.Net.WebSockets;
using System.Text;

namespace AspNetSignalIR.Connectors;

internal class HandleClientClass
{
    public static async Task HandleClientAsync(Client clientId, WebSocket webSocket, Dictionary<Client, WebSocket> _clients)
    {
        byte[] buffer = new byte[1024];

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    // Handle client disconnection
                    Console.WriteLine($"Client {clientId.id} disconnected");
                    _clients.Remove(clientId, out _);
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
                }
                else
                {
                    // Broadcast message to all connected clients
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received a mensage from client id: {clientId.id} name: {clientId.Nome}");

                    string responseMessage = $"{clientId.Nome}: {receivedMessage}";
                    await BroadcastClass.BroadcastMessageAsync(responseMessage, clientId, _clients);
                }
            }
            // Close the WebSocket and remove the client from the list
            if (_clients.Remove(clientId, out _))
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro no HandleClientClass: {ex.Message}");
        }
    }
}
