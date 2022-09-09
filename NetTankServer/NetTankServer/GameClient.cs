using System;
using System.Net.Sockets;

namespace NetTankServer;

public class GameClient
{
    public Socket Socket { get; init; }
    public byte[] Buffer { get; init; }

    public long? PlayerId { get; set; }

    public GameClient(Socket socket)
    {
        Socket = socket;
        Buffer = new byte[1024];
    }
}
