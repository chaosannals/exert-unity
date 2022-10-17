using System;
using System.Net;
using System.Net.Sockets;

namespace NetPackKit;

public class NetPackServer
{
    public delegate void AcceptedHandler(INetPackClient client);
    public delegate void LiftloopHandler();

    public event AcceptedHandler? Accepted;
    public event LiftloopHandler? AfterListen;

    public string Host { get; init; }
    public int Port { get; init; }
    public INetPackDispatcher Dispatcher { get; init; }

    public NetPackServer(INetPackDispatcher dispatcher, int port, string host="0.0.0.0")
    {
        Host = host;
        Port = port;
        Dispatcher = dispatcher;
    }

    public async Task ServeAsync(CancellationToken cancellationToken)
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var ipAddress = IPAddress.Parse(Host);
        var ipEndPoint = new IPEndPoint(ipAddress, Port);
        socket.NoDelay = true;
        socket.Bind(ipEndPoint);
        socket.Listen(0);
        AfterListen?.Invoke();

        socket.BeginAccept(OnAccept, socket);

        var checkRead = new List<Socket>();

        while (true)
        {
            checkRead.Clear();
            checkRead.AddRange(Dispatcher.Sockets);

            if (checkRead.Count > 0)
            {
                Socket.Select(checkRead, null, null, 1000);
                await Parallel.ForEachAsync(checkRead, cancellationToken, ReadFromClient);
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
        var server = ar.AsyncState as Socket;
        var socket = server!.EndAccept(ar);
        server.BeginAccept(OnAccept, server);
        var client = Dispatcher.NewClient(socket);
        Accepted?.Invoke(client);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="socket"></param>
    /// <returns></returns>
    public async ValueTask ReadFromClient(Socket socket, CancellationToken cancellationToken)
    {
        var client = Dispatcher.GetClient(socket);

        try
        {

            var m = await client!.ReceiveAsync(async (buffer) =>
            {
                int count = await socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken);
                if (count == 0)
                {
                    socket.Close();
                    Dispatcher.DropClient(socket);
                    // Log.Warning("client {0} read 0 bytes.", socket.RemoteEndPoint);
                }
                return count;
            });

            if (m != null)
            {
                Dispatcher.Dispatch(client, m);
            }
        }
        catch (Exception e)
        {
            socket.Close();
            Dispatcher.DropClient(socket);
            //Log.Warning("serve loop exception: {0}", e);
        }
    }
}
