﻿using NetTankServer.Messages;
using Serilog;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetTankServer;

public class GameServer
{
    private GameRoomManager roomManager;
    private GamePlayerManager playerManager;
    private GameClientManager clientManager;
    private ILogger logger;

    public string Host { get; init; }
    public int Port { get; init; }

    public GameServer(
        int port,
        GameRoomManager roomManager,
        GamePlayerManager playerManager,
        GameClientManager clientManager,
        ILogger logger,
        string host="0.0.0.0"
    )
    {
        Port = port;
        Host = host;
        this.roomManager = roomManager;
        this.playerManager = playerManager;
        this.clientManager = clientManager;
        this.logger = logger;
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
            checkRead.AddRange(clientManager.Sockets);

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
        clientManager.NewClient(client);
        logger.Information("accept: {0}", client.RemoteEndPoint);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="socket"></param>
    /// <returns></returns>
    public bool ReadFromClient(Socket socket)
    {
        var state = clientManager.GetClient(socket)!;

        try
        {
            int count = socket.Receive(state.Buffer);
            if (count == 0)
            {
                socket.Close();
                clientManager.DropClient(socket);
                logger.Warning("client {0} read 0 bytes.", socket.RemoteEndPoint);
                return false;
            }

            var m = state.Reader.Read(state.Buffer, 0, count);

            if (m != null)
            {
                var sendBytes = m.Body.Encode();

                clientManager.ForEach((sock, gc) =>
                {
                    gc.Socket?.Send(sendBytes);
                });
            }

            return true;
        }
        catch (Exception e)
        {
            socket.Close();
            clientManager.DropClient(socket);
            logger.Warning("serve loop exception: {0}", e);
            return false;
        }
    }
}
