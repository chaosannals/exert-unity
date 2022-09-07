using ServerDemo;

try
{
    var server = new GameServer();
    server.Serve();
}
catch (Exception e)
{
    Console.WriteLine(e);
}