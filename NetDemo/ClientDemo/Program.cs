using ClientDemoCommon;

Console.WriteLine("Game Client Start");

var client = new GameClient();
client.Connect();

while (true)
{
    var cmd = Console.ReadLine();
    if (cmd != null && cmd.StartsWith("quit"))
    {
        break;
    }
    client.Send(cmd ?? "empty");
}
