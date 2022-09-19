using System;
using System.Collections.Concurrent;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Serilog;
using DemoCommon;
using DemoCommon.Messages;

namespace ServerDemo;

class GameServer
{
    public string Host { get; init; }
    public int Port { get; init; }
    private ConcurrentDictionary<Socket, GameClientState> clients;

    public GameServer(int port=44444, string host="0.0.0.0")
    {
        Host = host;
        Port = port;
        clients = new ConcurrentDictionary<Socket, GameClientState>();
    }


    public async Task Serve()
    //public void Serve()
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var ipAddress = IPAddress.Parse(Host);
        var ipEndPoint = new IPEndPoint(ipAddress, Port);
        socket.NoDelay = true;
        socket.Bind(ipEndPoint);
        socket.Listen(0);

        Log.Information("listen: {0}", socket.LocalEndPoint);
        var ipEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in ipEntry.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                Log.Information("listen: {0}:{1}", ip, Port);
            }
        }

        try
        {
            //socket.BeginAccept(OnAccept, socket);

            await Parallel.ForEachAsync(new List<Task>
            {
                AcceptAsync(socket),
                ReceiveAsync(),
            }, async (t, c) => await t);
        }
        catch (Exception e)
        {
            Log.Information("{0}", e);
        }
    }

    public async Task AcceptAsync(Socket socket)
    {
        while(true)
        {
            var client = await socket.AcceptAsync();
            var state = new GameClientState();
            state.Socket = client;
            clients.TryAdd(client, state);
            Log.Information("[{0}] accept: {1}", clients.Count, client.RemoteEndPoint);
        }
    }

    public async Task ReceiveAsync()
    {
        while (true)
        {
            var checkRead = clients.Values.Select(cs => cs.Socket!).ToList();

            if (checkRead.Count > 0)
            {
                Socket.Select(checkRead, null, null, 1000);
                await Parallel.ForEachAsync(checkRead, ReadFromClient);
                //Parallel.ForEach(checkRead, ReadFromClient);
            }
            else
            {
                await Task.Yield();
            }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="ar"></param>
    public void OnAccept(IAsyncResult ar)
    {
        var socket = ar.AsyncState as Socket;
        var client = socket!.EndAccept(ar);
        socket.BeginAccept(OnAccept, socket);
        var state = new GameClientState();
        state.Socket = client;
        clients.TryAdd(client, state);
        Log.Information("[{0}] accept: {1}", clients.Count, client.RemoteEndPoint);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="socket"></param>
    /// <returns></returns>
    public async ValueTask ReadFromClient(Socket socket, CancellationToken cancellationToken)
    //public void ReadFromClient(Socket socket)
    {
        var state = clients[socket];
        
        try
        {
            //int count = await socket.ReceiveAsync(state.Buffer, SocketFlags.None);
            ////int count = socket.Receive(state.Buffer);
            //if (count == 0)
            //{
            //    socket.Close();
            //    GameClientState? gcs;
            //    clients.Remove(socket, out gcs);
            //    Log.Warning("client {0} read 0 bytes.", socket.RemoteEndPoint);
            //}

            //var m = state.Reader.Read(state.Buffer, 0, count);

            var m = await state.Reader.ReadAsync(async (buffer) =>
            {
                int count = await socket.ReceiveAsync(buffer, SocketFlags.None);
                if (count == 0)
                {
                    socket.Close();
                    GameClientState? gcs;
                    clients.Remove(socket, out gcs);
                    Log.Warning("client {0} read 0 bytes.", socket.RemoteEndPoint);
                }
                return count;
            });

            if (m != null)
            {
                Dispatch(state, m);
            }
        }
        catch (Exception e)
        {
            socket.Close();
            GameClientState? gcs;
            clients.Remove(socket, out gcs);
            Log.Warning("serve loop exception: {0}", e);
        }
    }

    public void Dispatch(GameClientState client, GameMessagePack m)
    {
        var socket = client.Socket!;
        switch (m.Head.kindName)
        {
            case nameof(GamePingMessage):
                //Log.Information("from: {0} msg: {1}", socket.RemoteEndPoint, m.Head.kindName);
                socket.Send(new GamePongMessage().Encode());
                break;
            case nameof(GameEnterMessage):
                var em = m.Body as GameEnterMessage;
                Log.Information("enter: {0} pid: {1}", socket.RemoteEndPoint, em?.playerId);
                foreach (var cs in clients)
                {
                    if (cs.Value.Socket! != socket)
                    {
                        cs.Value.Socket?.Send(m.Body.Encode());
                    }
                }
                break;
            default:
                break;
        }
    }
}
