using ClientDemoCommon;
using DemoCommon.Messages;
using System.Diagnostics;

Thread.Sleep(2000);

Console.WriteLine("Game Client Start");

object countLock = new { };
int count = 0;
var stopWatch = new Stopwatch();
stopWatch.Start();
var client = new GameClient(44444);
client.ReceiveDispatch += (c, m) =>
{
    lock(countLock)
    {
        if ((count & 0x7F) == 0)
        {
            Console.WriteLine($"m count: {count} ms: {stopWatch.ElapsedMilliseconds}");
            stopWatch.Restart();
        }
        count++;
    }
};
client.Connect();
while (true)
{
    var cmd = Console.ReadLine();
    if (cmd != null && cmd.StartsWith("quit"))
    {
        break;
    }

    switch (cmd)
    {
        case nameof(GameEnterMessage):
            client?.Send(new GameEnterMessage() { playerId = 123 });
            break;
        case nameof(GamePingMessage):
            client?.Send(new GamePingMessage());
            break;
        default:
            break;
    }
}
