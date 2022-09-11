using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;


public class NetClient : IDisposable
{
    public string Host { get; private set; }
    public int Port { get; private set; }
    public bool IsClosing { get; private set; }
    public Socket Sock { get; private set; }

    private byte[] buffer;
    private ConcurrentQueue<NetBuffer> sendQueue;

    public NetClient(Socket socket, string host="127.0.0.1", int port = 44444)
    {
        Host = host;
        Port = port;
        Sock = socket;
        buffer = new byte[1024];
        sendQueue = new ConcurrentQueue<NetBuffer>();
    }

    public void Connect()
    {
        Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Sock.NoDelay = true;
        Sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        Sock.BeginConnect(Host, Port, OnConnect, Sock);
    }

    public void Send(string text)
    {
        if (IsClosing) return;

        byte[] sendBytes = Encoding.UTF8.GetBytes(text);
        var gdb = new NetBuffer(sendBytes);
        sendQueue.Enqueue(gdb);
        if (sendQueue.Count == 1)
        {
            Sock.BeginSend(gdb.Data, gdb.Start, gdb.Size, 0, OnSend, Sock);
        }
    }

    public void Dispose()
    {
        
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
            Console.WriteLine($"on connect: {e}");
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
            sock!.BeginReceive(buffer, 0, buffer.Length, 0, OnReceive, sock!);
        }
        catch (Exception e)
        {
            Console.WriteLine($"on client receive: {e}");
        }
    }

    public void OnSend(IAsyncResult ar)
    {
        try
        {
            var sock = ar.AsyncState as Socket;
            int count = sock!.EndSend(ar);

            NetBuffer nb = null;
            if (sendQueue.TryPeek(out nb))
            {
                nb.Skip(count);
                if (nb.Size == 0)
                {
                    sendQueue.TryDequeue(out nb);
                    if (!sendQueue.TryPeek(out nb))
                    {
                        nb = null;
                    }
                }
            }

            if (nb != null)
            {
                sock!.BeginSend(nb.Data, nb.Start, nb.Size, 0, OnSend, sock);
            }
            else if (IsClosing)
            {
                sock!.Close();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"on send: {e}");
        }
    }
}
