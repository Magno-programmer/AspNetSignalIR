using AspNetSignalIR.ClientListen;
using System.Net;
using System.Net.WebSockets;
using static AspNetSignalIR.Connectors.ListenClientClass;
/*
 OBS: Esse tipo de conexão não é necessário ficar se reconectando, ela se mantem
      ligada
 */

Dictionary<Client, WebSocket> _clients = new();
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseWebSockets();

try
{
    app.Map(pattern: "/", requestDelegate: async context =>
    {
        try
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.CompleteAsync(); // Safely complete the response
                return;
            }
            else
            {
                Task teste = ListenForClients(context, _clients);
                await teste;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro no app.Map: {ex.Message}");
        }
    });
    await app.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Ocorreu um erro no Program: {ex.Message}");
}