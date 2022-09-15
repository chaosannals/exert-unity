using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net.Sockets;
using DemoCommon;
using DemoCommon.Messages;

namespace ClientDemoCommon;

public delegate void ReceiveDispatchHandler(GameClient client, GameMessagePack message);

public class GameClient : IDisposable
{
    public long Id { get; init; }
    public string Host { get; init; }
    public int Port { get; init; }

    public bool IsClosing { get; private set; }

    private Socket? socket;
    private byte[] buffer;
    private ConcurrentQueue<GameDataBuffer> sendQueue;
    private GameMessageReader reader;

    public event ReceiveDispatchHandler? ReceiveDispatch;

    public GameClient(long id, string host="127.0.0.1", int port = 44444)
    {
        Id = id;
        Host = host;
        Port = port;
        IsClosing = false;
        socket = null;
        buffer = new byte[1024];
        sendQueue = new ConcurrentQueue<GameDataBuffer>();
        reader = new GameMessageReader();
    }

    public void Connect()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.NoDelay = true;
        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        socket.BeginConnect(Host, Port, OnConnect, socket);
    }

    public void Send<T>(T message) where T : GameBaseMessage
    {
        if (IsClosing) return;

        byte[] sendBytes = message.Encode();
        //Console.WriteLine(Encoding.UTF8.GetString(sendBytes));
        var gdb = new GameDataBuffer(sendBytes);
        sendQueue.Enqueue(gdb);
        if (sendQueue.Count == 1)
        {
            socket?.BeginSend(gdb.Data, gdb.Head, gdb.Size, 0, OnSend, socket);
        }
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

            var m = reader.Read(buffer, 0, count);

            if (m != null)
            {
                ReceiveDispatch?.Invoke(this, m);
            }

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
      
            var gdb = sendQueue.First();
            gdb.Head += count;
            if (gdb.Size == 0)
            {
                sendQueue.TryDequeue(out gdb);
                gdb = sendQueue.IsEmpty ? null : sendQueue.First();
            }

            if (gdb != null)
            {
                sock!.BeginSend(gdb.Data, gdb.Head, gdb.Size, 0, OnSend, sock);
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

    public void Close()
    {
        if (sendQueue.IsEmpty)
        {
            socket?.Close();
        }
        else
        {
            IsClosing = true;
        }
    }

    public void Dispose()
    {
        if (!IsClosing)
        {
            Close();
        }
    }
}
