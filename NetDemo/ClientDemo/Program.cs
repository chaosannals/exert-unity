using ClientDemoCommon;
using DemoCommon.Messages;

Thread.Sleep(2000);

Console.WriteLine("Game Client Start");

var client = new GameClient(44444);
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
