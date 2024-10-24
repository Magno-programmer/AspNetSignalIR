using System.Net;

namespace AspNetSignalIR.ClientListen;

internal class Client
{
    public Client(string name)
    {
        this.id = _idIncrement++;
        Nome = name;
    }

    public int id { get; private set; }
    public string Nome { get; set; }

    private static int _idIncrement = 1;
}
