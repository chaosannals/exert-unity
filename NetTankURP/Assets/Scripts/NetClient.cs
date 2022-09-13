using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;

public static class NetClient
{
    public delegate void MessageHandler<T>(T message) where T : BaseMessage;


    public static event MessageHandler<PlayerEnterMessage> PlayerEnter;
    public static event MessageHandler<PlayerLeaveMessage> PlayerLeave;
    public static event MessageHandler<PlayerListMessage> PlayerList;

    public static event MessageHandler<TankFireMessage> TankFire;
    public static event MessageHandler<TankHitMessage> TankHit;
    public static event MessageHandler<TankMoveMessage> TankMove;


    public static string Host { get; private set; }
    public static int Port { get; private set; }
    public static bool IsClosing { get; private set; }
    public static Socket Sock { get; private set; }

    private static byte[] buffer;
    private static ConcurrentQueue<NetBuffer> sendQueue;
    private static MessageHead readHead = null;

    static NetClient()
    {
        buffer = new byte[1024];
        sendQueue = new ConcurrentQueue<NetBuffer>();
    }

    public static void Connect(string host = "127.0.0.1", int port = 44444)
    {
        Host = host;
        Port = port;
        Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Sock.NoDelay = true;
        Sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        Sock.BeginConnect(Host, Port, OnConnect, Sock);
    }


    public static void Send<T>(T message) where T : BaseMessage
    {
        if (IsClosing) return;

        //byte[] sendBytes = Encoding.UTF8.GetBytes(text);
        var sendBytes = message.Encode();
        var gdb = new NetBuffer(sendBytes);
        sendQueue.Enqueue(gdb);
        if (sendQueue.Count == 1)
        {
            Sock.BeginSend(gdb.Data, gdb.Start, gdb.Size, 0, OnSend, Sock);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ar"></param>
    public static void OnConnect(IAsyncResult ar)
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ar"></param>
    public static void OnReceive(IAsyncResult ar)
    {
        try
        {
            var sock = ar.AsyncState as Socket;
            var count = sock!.EndReceive(ar);
            //var text = Encoding.UTF8.GetString(buffer, 0, count);
            //Console.WriteLine($"client read: {text}");

            if (readHead == null)
            {
                readHead = BaseMessage.HeadOf(buffer);
            }
            // TODO

            sock!.BeginReceive(buffer, 0, buffer.Length, 0, OnReceive, sock!);
        }
        catch (Exception e)
        {
            Console.WriteLine($"on client receive: {e}");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ar"></param>
    public static void OnSend(IAsyncResult ar)
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
