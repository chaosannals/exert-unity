﻿using NetPackKit;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace ServerDemo;

public class DemoDispatcher : INetPackDispatcher
{
    private ConcurrentDictionary<Socket, DemoClient> clients;

    public IEnumerable<Socket> Sockets { get { return clients.Values.Select(i => i.Sock); } }

    public DemoDispatcher()
    {
        clients = new ConcurrentDictionary<Socket, DemoClient>();
    }

    public void Dispatch(INetPackClient client, NetPack m)
    {
        switch (m.Kind)
        {
            case nameof(DemoClient):
                break;
        }
    }

    public INetPackClient NewClient(Socket socket)
    {
        var client = new DemoClient(socket);
        clients.AddOrUpdate(socket, client, (s, old) => old);
        return client;
    }

    public INetPackClient? DropClient(Socket socket)
    {
        DemoClient? client;
        clients.Remove(socket, out client);
        return client;
    }

    public INetPackClient? GetClient(Socket socket)
    {
        if (clients.ContainsKey(socket))
        {
            return clients[socket];
        }
        return null;
    }

    public INetPackDispatcher ForEach(Action<Socket, INetPackClient> action)
    {
        foreach (var client in clients)
        {
            action.Invoke(client.Key, client.Value);
        }
        return this;
    }
}
