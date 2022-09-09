using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Serilog;

namespace ServerDemo;

class GameServer
{
    public string Host { get; init; }
    public int Port { get; init; }
    private Dictionary<Socket, GameClientState> clients;

    public GameServer(int port=44444, string host="0.0.0.0")
    {
        Host = host;
        Port = port;
        clients = new Dictionary<Socket, GameClientState>();
    }


    public void Serve()
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
        
        var checkRead = new List<Socket>();
        
        while (true)
        {
            checkRead.Clear();
            checkRead.Add(socket);
            foreach (var cs in clients.Values)
            {
                checkRead.Add(cs.Socket!);
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="socket"></param>
    public void ReadFromServer(Socket socket)
    {
        var client = socket.Accept();
        var state = new GameClientState();
        state.Socket = client;
        clients.Add(client, state);
        Log.Information("accept: {0}", client.RemoteEndPoint);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="socket"></param>
    /// <returns></returns>
    public bool ReadFromClient(Socket socket)
    {
        var state = clients[socket];
        
        try
        {
            int count = socket.Receive(state.Buffer);
            if (count == 0)
            {
                socket.Close();
                clients.Remove(socket);
                Log.Warning("client {0} read 0 bytes.", socket.RemoteEndPoint);
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
            clients.Remove(socket);
            Log.Warning("serve loop exception: {0}", e);
            return false;
        }
    }
}
