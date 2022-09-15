using NetTankServer.Messages;
using System;
using System.Net.Sockets;

namespace NetTankServer;

public class GameClient
{
    public long Id { get; init; }
    public Socket Socket { get; init; }
    public byte[] Buffer { get; init; }

    public long? PlayerId { get; set; }

    public MessageReader Reader { get; init; }

    public GameClient(long id, Socket socket)
    {
        Id = id;
        Socket = socket;
        Buffer = new byte[1024];
        Reader = new MessageReader();
    }
}
