namespace AspNetSignalIR.ClientListen;

internal class Client
{
    public int id { get; private set; }
    public string Nome { get; set; }
    private static int _idIncrement = 1;
    private static Stack<int> availableIds = new();

    public Client(string name)
    {
        if (availableIds.Count > 0)
        {
            this.id = availableIds.Pop(); // Reutiliza ID disponível
        }
        else
        {
            this.id = _idIncrement++;
        }

        Nome = name;
    }

    public static void RemoverCliente(Client client)
    {
        // Adiciona o ID do cliente removido para reutilização
        availableIds.Push(client.id);
    }
}
