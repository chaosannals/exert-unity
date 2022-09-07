using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace ClientDemo;

class GameClient
{
    public string Host { get; init; }
    public int Port { get; init; }
    private Socket? socket;
    private byte[] buffer;


    public GameClient(string host="127.0.0.1", int port = 44444)
    {
        Host = host;
        Port = port;
        socket = null;
        buffer = new byte[1024];
    }

    public void Connect()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.BeginConnect(Host, Port, OnConnect, socket);
    }

    public void Send(string text)
    {
        byte[] sendBytes = Encoding.UTF8.GetBytes(text);
        socket?.Send(sendBytes);
    }

    public void OnConnect(IAsyncResult ar)
    {
        try
        {
            var sock = ar.AsyncState as Socket;
            sock!.EndConnect(ar);

            sock!.BeginReceive(buffer, 0, buffer.Length, 0, OnReceive, sock!);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void OnReceive(IAsyncResult ar)
    {
        try
        {
            var sock = ar.AsyncState as Socket;
            var count = sock!.EndReceive(ar);
            var text = Encoding.UTF8.GetString(buffer, 0, count);
            Console.WriteLine($"client read: {text}");
            sock.BeginReceive(buffer, 0, buffer.Length, 0, OnReceive, sock!);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
