using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetTankServer;

var ioc = new ServiceCollection();
ioc.AddSingleton(op =>
{
    return new GameRoomManager();
});
ioc.AddSingleton(op =>
{
    return new GamePlayerManager();
});
ioc.AddSingleton(op =>
{
    var roomManager = op.GetRequiredService<GameRoomManager>();
    var main = new GameMain(roomManager);
    return main;
});
//ioc.AddDbContext<>

var provider = ioc.BuildServiceProvider();
var main = provider.GetRequiredService<GameMain>();
main.Run();
