using NetTankServer.Messages;
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

    public string Host { get; init; }
    public int Port { get; init; }

    public GameServer(
        int port,
        GameRoomManager roomManager,
        GamePlayerManager playerManager,
        GameClientManager clientManager,
        //ILogger logger,
        string host="0.0.0.0"
    )
    {
        Port = port;
        Host = host;
        this.roomManager = roomManager;
        this.playerManager = playerManager;
        this.clientManager = clientManager;
    }

    /// <summary>
    /// 
    /// </summary>
    public async Task Serve()
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var ipAddress = IPAddress.Parse(Host);
        var ipEndPoint = new IPEndPoint(ipAddress, Port);
        socket.NoDelay = true;
        socket.Bind(ipEndPoint);
        socket.Listen(0);

        //logger.Information("listen: {0}", socket.LocalEndPoint);
        var ipEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in ipEntry.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                //logger.Information("listen: {0}:{1}", ip, Port);
            }
        }

        socket.BeginAccept(OnAccept, socket);

        var checkRead = new List<Socket>();

        while (true)
        {
            checkRead.Clear();
            checkRead.AddRange(clientManager.Sockets);

            if (checkRead.Count > 0)
            {
                Socket.Select(checkRead, null, null, 1000);
                await Parallel.ForEachAsync(checkRead, ReadFromClient);
            }
        }
    }

    public void OnAccept(IAsyncResult ar)
    {
        var server = ar.AsyncState as Socket;
        var socket = server!.EndAccept(ar);
        server.BeginAccept(OnAccept, server);
        var client = clientManager.NewClient(socket);
        Log.Information("[{0}] accept: {1}", client.Id, socket.RemoteEndPoint);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="socket"></param>
    /// <returns></returns>
    public async ValueTask ReadFromClient(Socket socket, CancellationToken cancellationToken)
    {
        var state = clientManager.GetClient(socket)!;

        try
        {
            int count = await socket.ReceiveAsync(state.Buffer, SocketFlags.None);
            if (count == 0)
            {
                socket.Close();
                clientManager.DropClient(socket);
                //logger.Warning("client {0} read 0 bytes.", socket.RemoteEndPoint);
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
        }
        catch (Exception e)
        {
            socket.Close();
            clientManager.DropClient(socket);
            //logger.Warning("serve loop exception: {0}", e);
        }
    }
}
