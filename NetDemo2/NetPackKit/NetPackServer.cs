using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetPackKit;

public class NetPackServer<T>
{
    public string Host { get; init; }
    public int Port { get; init; }
    private ConcurrentDictionary<Socket, T> clients;

    public NetPackServer(string host, int port)
    {
        Host = host;
        Port = port;
        clients = new ConcurrentDictionary<Socket, T>();
    }


}
