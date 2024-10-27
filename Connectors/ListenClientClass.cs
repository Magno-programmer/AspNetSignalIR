using AspNetSignalIR.ClientListen;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace AspNetSignalIR.Connectors;

internal class ListenClientClass
{
    public static async Task ListenForClients(HttpContext context, Dictionary<Client, WebSocket> _clients)
    {
        byte[] buffer = new byte[256];

        try
        {

            if (context.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                string receivedName = Encoding.UTF8.GetString(buffer, 0, result.Count);

                Client newCliente = new(receivedName); // Unique ID for each client
                _clients.TryAdd(newCliente, webSocket);

                Console.WriteLine($"Client \nId: {newCliente.id} \nName: {newCliente.Nome} \nStatus: connected");

                int option = 0;

                // Handle communication with this client
                await Task.Run(() => HandleClientClass.HandleClientAsync(newCliente, webSocket, _clients, option));
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.CompleteAsync(); // Safely complete the response
                return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro no ListenClientClass: {ex.Message}");

        }
    }
}
