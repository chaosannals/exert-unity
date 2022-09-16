using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetPackKit;

public interface INetPackDispatcher
{
    public IEnumerable<Socket> Sockets { get; }
    public INetPackClient NewClient(Socket socket);
    public INetPackClient? DropClient(Socket socket);
    public INetPackClient? GetClient(Socket socket);
    public INetPackDispatcher ForEach(Action<Socket, INetPackClient> action);

    public void Dispatch(INetPackClient client, NetPack m);
}
