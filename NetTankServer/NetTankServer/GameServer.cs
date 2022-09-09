using Serilog;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetTankServer;

public class GameServer
{
    private GameRoomManager roomManager;
    private GamePlayerManager playerManager;
    private ILogger logger;
    private ConcurrentDictionary<Socket, GameClient> clients;

    public string Host { get; init; }
    public int Port { get; init; }

    public GameServer(
        int port,
        GameRoomManager roomManager,
        GamePlayerManager playerManager,
        ILogger logger,
        string host="0.0.0.0"
    )
    {
        Port = port;
        Host = host;
        this.roomManager = roomManager;
        this.playerManager = playerManager;
        this.logger = logger;
        clients = new ConcurrentDictionary<Socket, GameClient>();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Serve()
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var ipAddress = IPAddress.Parse(Host);
        var ipEndPoint = new IPEndPoint(ipAddress, Port);
        socket.NoDelay = true;
        socket.Bind(ipEndPoint);
        socket.Listen(0);

        logger.Information("listen: {0}", socket.LocalEndPoint);
        var ipEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in ipEntry.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                logger.Information("listen: {0}:{1}", ip, Port);
            }
        }

        var checkRead = new List<Socket>();

        while (true)
        {
            checkRead.Clear();
            checkRead.Add(socket);
            foreach (var cs in clients.Values)
            {
                checkRead.Add(cs.Socket);
            }

            Socket.Select(checkRead, null, null, 1000);
            foreach (var cr in checkRead)
            {
                if (cr == socket)
                {
                    ReadFromServer(cr);
                }
                else
                {
                    ReadFromClient(cr);
                }
            }
        }
    }

    public void ReadFromServer(Socket socket)
    {
        var client = socket.Accept();
        var state = new GameClient(client);
        clients.AddOrUpdate(client, state, (s, old) => old);
        logger.Information("accept: {0}", client.RemoteEndPoint);
    }

    public bool ReadFromClient(Socket socket)
    {
        var state = clients[socket];

        try
        {
            int count = socket.Receive(state.Buffer);
            if (count == 0)
            {
                socket.Close();
                GameClient? client;
                clients.Remove(socket, out client);
                logger.Warning("client {0} read 0 bytes.", socket.RemoteEndPoint);
                return false;
            }

            // TODO
            var endPoint = socket.RemoteEndPoint?.ToString() ?? "unknown";
            byte[] sendBytes = Encoding.UTF8.GetBytes(endPoint);

            foreach (var cs in clients.Values)
            {
                cs.Socket?.Send(sendBytes);
            }

            return true;
        }
        catch (Exception e)
        {
            socket.Close();
            GameClient? client;
            clients.Remove(socket, out client);
            logger.Warning("serve loop exception: {0}", e);
            return false;
        }
    }
}
