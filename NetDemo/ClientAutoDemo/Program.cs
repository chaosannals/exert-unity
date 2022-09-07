using ClientDemoCommon;

Console.WriteLine("Client Auto !");

// TODO 数量太多等待会很长，要优化，目前 10 。
var clients = Enumerable.Range(1, 10)
    .ToDictionary(i => i, _ => 
    {
        var r = new GameClient();
        r.Connect();
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
            var d = DateTime.Now;
            client.Value.Send($"client {client.Key} say: {d}");
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
