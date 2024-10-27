using AspNetSignalIR.ClientListen;
using static AspNetSignalIR.OpenAITest.APIOpenAI;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace AspNetSignalIR.Connectors;

internal class HandleClientClass
{
    public static async Task HandleClientAsync(Client clientId, WebSocket webSocket, Dictionary<Client, WebSocket> _clients, int option)
    {
        byte[] buffer = new byte[1024];
        byte[] bufferOption = new byte[256];

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

                    if (receivedMessage.All(char.IsDigit))
                    {
                        option = int.Parse(receivedMessage);

                        string send = $"Opção escolhida {receivedMessage}";
                        byte[] opcaoClient = Encoding.UTF8.GetBytes(send);
                        await webSocket.SendAsync(new ArraySegment<byte>(opcaoClient), WebSocketMessageType.Text, true, CancellationToken.None);

                    }

                    if (receivedMessage == "back")
                    {
                        continue;
                    }

                    switch (option)
                    {
                        case 1:

                            string responseMessage = $"{clientId.Nome}: {receivedMessage}";
                            await BroadcastClass.BroadcastMessageAsync(responseMessage, clientId, _clients);
                            break;

                        case 2:

                            string chatBox = await Conectar(receivedMessage);
                            string resposta = $"ChatBox: {chatBox}";
                            byte[] responseMessageChat = Encoding.UTF8.GetBytes(resposta);
                            await webSocket.SendAsync(new ArraySegment<byte>(responseMessageChat), WebSocketMessageType.Text, true, CancellationToken.None);
                            break;

                        default:
                            throw new Exception("Invalid Option");
                    }
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
