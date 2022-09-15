using System;
using System.Collections.Concurrent;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Serilog;

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

        //Task.Run(() =>
        //{
        //    var checkRead = new List<Socket>();

        //    while (true)
        //    {
        //        checkRead.Clear();
        //        foreach (var cs in clients.Values)
        //        {
        //            checkRead.Add(cs.Socket!);
        //        }

        //        if (checkRead.Count > 0)
        //        {
        //            Socket.Select(checkRead, null, null, 1000);
        //            foreach (var cr in checkRead)
        //            {
        //                ReadFromClient(cr);
        //            }
        //        }
        //        else
        //        {
        //            Task.Yield();
        //        }
        //    }
        //}).ConfigureAwait(false);

        try
        {
            socket.BeginAccept(OnAccept, socket);

            var checkRead = new List<Socket>();

            while (true)
            {
                checkRead.Clear();
                foreach (var cs in clients.Values)
                {
                    checkRead.Add(cs.Socket!);
                }

                if (checkRead.Count > 0)
                {
                    Socket.Select(checkRead, null, null, 1000);
                    foreach (var cr in checkRead)
                    {
                        ReadFromClient(cr);
                    }
                }
                else
                {
                    Thread.Yield();
                }
            }
        }
        catch (Exception e)
        {
            Log.Information("{0}", e);
        }

        //while(true)
        //{
        //    ReadFromServer(socket);
        //}
        
        //var checkRead = new List<Socket>();
        
        //while (true)
        //{
        //    checkRead.Clear();
        //    checkRead.Add(socket);
        //    foreach (var cs in clients.Values)
        //    {
        //        checkRead.Add(cs.Socket!);
        //    }

        //    Socket.Select(checkRead, null, null, 1000);
        //    foreach (var cr in checkRead)
        //    {
        //        if (cr == socket)
        //        {
        //            ReadFromServer(cr);
        //        }
        //        else
        //        {
        //            ReadFromClient(cr);
        //        }
        //    }
        //}
    }

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
    public void ReadFromServer(Socket socket)
    {
        var client = socket.Accept();
        var state = new GameClientState();
        state.Socket = client;
        clients.TryAdd(client, state);
        //clients.Add(client, state);
        Log.Information("[{0}] accept: {1}", clients.Count, client.RemoteEndPoint);
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
                //clients.Remove(socket);
                GameClientState? gcs;
                clients.Remove(socket, out gcs);
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
            //clients.Remove(socket);
            GameClientState? gcs;
            clients.Remove(socket, out gcs);
            Log.Warning("serve loop exception: {0}", e);
            return false;
        }
    }
}
