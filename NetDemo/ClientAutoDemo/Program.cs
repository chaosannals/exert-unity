using System;
using ClientDemoCommon;
using DemoCommon.Messages;

Console.WriteLine("Client Auto !");

// 因为 demo 是和服务器一起启动，所以先等待服务器初始化完成。
Thread.Sleep(4000);

var clients = Enumerable.Range(1, 444)
    //.AsParallel()
    .ToDictionary(i => i, i => 
    {
        var r = new GameClient(i);
        r.ReceiveDispatch += (c, m) =>
        {
            switch (m.Head.kindName)
            {
                case nameof(GameEnterMessage):
                    var em = m.Body as GameEnterMessage;
                    Console.WriteLine($"pid; {em?.playerId}");
                    break;
                case nameof(GamePongMessage):
                    Console.WriteLine($"pong");
                    break;
                default:
                    break;
            }
        };
        r.Connect();
        Thread.Sleep(10);
        return r;
    });

// TODO 需要连接完成才能发送，这个等待要换成事件。
Thread.Sleep(2000);

while (true)
{
    try
    {
        foreach (var client in clients)
        {
            Console.WriteLine($"send pid: {client.Value.Id}");
            client.Value.Send(new GameEnterMessage { playerId = client.Value.Id });
        }

        foreach (var client in clients)
        {
            client.Value.Send(new GamePingMessage());
        }
        var n = DateTime.Now;
        Console.WriteLine($"[{n}] clients say final.");
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
    Thread.Sleep(1000);
}
