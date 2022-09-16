using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NetPackKit;

public interface INetPackClient
{
    public Socket Sock { get; }

    public Task<NetPack?> ReceiveAsync(Func<Memory<byte>, ValueTask<int>> write);
}
