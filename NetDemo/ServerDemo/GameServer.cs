using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ServerDemo;

class GameServer
{
    public string Host { get; init; }
    public int Port { get; init; }
    //private Socket? socket;
    private Dictionary<Socket, GameClientState> clients;

    public GameServer(int port=44444, string host="0.0.0.0")
    {
        Host = host;
        Port = port;
        //socket = null;
        clients = new Dictionary<Socket, GameClientState>();
    }


    public void Serve()
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var ipAddress = IPAddress.Parse(Host);
        var ipEndPoint = new IPEndPoint(ipAddress, Port);
        socket.Bind(ipEndPoint);
        socket.Listen(0);

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

    public void ReadFromServer(Socket socket)
    {
        var client = socket.Accept();
        var state = new GameClientState();
        state.Socket = client;
        clients.Add(client, state);
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
                clients.Remove(socket);
                Console.WriteLine("client read 0 bytes.");
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
            Console.WriteLine(e);
            return false;
        }
    }
}
