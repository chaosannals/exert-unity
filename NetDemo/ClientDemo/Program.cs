using ClientDemoCommon;

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
    client.Send(cmd ?? "empty");
}
