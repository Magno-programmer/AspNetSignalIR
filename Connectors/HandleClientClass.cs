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
                    Console.WriteLine($"Client {clientId.id} : {clientId.Nome} disconnected");
                    Client.RemoverCliente(clientId);
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
                    _clients.Remove(clientId, out _);
                }
                else
                {
                    // Broadcast message to all connected clients
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    if (receivedMessage.All(char.IsDigit))
                    {
                        option = int.Parse(receivedMessage);
                        Console.WriteLine($"\nReceived a option choose from: \nClient id: {clientId.id} \nName: {clientId.Nome} \nOption: {option}");

                        continue;
                    }
                    Console.WriteLine($"\nReceived a message from: \nClient id: {clientId.id} \nName: {clientId.Nome}");

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
