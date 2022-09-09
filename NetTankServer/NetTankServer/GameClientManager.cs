using IdGen;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetTankServer;

public class GameClientManager
{
    private IdGenerator idGenerator;
    private ConcurrentDictionary<Socket, GameClient> clients;

    public IEnumerable<Socket> Sockets { get { return clients.Values.Select(i => i.Socket);  } }

    public GameClientManager(int id)
    {
        idGenerator = new IdGenerator(id);
        clients = new ConcurrentDictionary<Socket, GameClient>();
    }

    public GameClient NewClient(Socket socket)
    {
        var id = idGenerator.CreateId();
        var client = new GameClient(id, socket);
        clients.AddOrUpdate(socket, client, (s, old) => old);
        return client;
    }

    public GameClient? DropClient(Socket socket)
    {
        GameClient? client;
        clients.Remove(socket, out client);
        return client;
    }

    public GameClient? GetClient(Socket socket)
    {
        if (clients.ContainsKey(socket))
        {
            return clients[socket];
        }
        return null;
    }

    public GameClientManager ForEach(Action<Socket, GameClient> action)
    {
        foreach (var client in clients)
        {
            action.Invoke(client.Key, client.Value);
        }
        return this;
    }
}
