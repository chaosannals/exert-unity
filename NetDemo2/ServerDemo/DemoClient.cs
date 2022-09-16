using NetPackKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerDemo;

public class DemoClient : INetPackClient
{
    private Socket socket;
    private NetPackBuffer buffer;

    public Socket Sock => socket;

    public DemoClient(Socket socket)
    {
        this.socket = socket;
        this.buffer = new NetPackBuffer();
    }

    public async Task<NetPack?> ReceiveAsync(Func<Memory<byte>, ValueTask<int>> write)
    {
        await buffer.WriteAsync(write);
        return buffer.Read();
    }
}
