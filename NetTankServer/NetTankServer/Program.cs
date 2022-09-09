using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using NetTankServer;

var ioc = new ServiceCollection();
var cb = new ConfigurationBuilder();
var cnf = cb.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .Add(new JsonConfigurationSource
    {
        Path = "appsettings.json",
        ReloadOnChange = true,
    }).Build();
ioc.AddSingleton(op => cnf);
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
ioc.AddDbContext<GameDbContext>(opb =>
{
    var connetionString = cnf.GetConnectionString("Main");
    opb.UseMySQL(connetionString);
    
    });

var provider = ioc.BuildServiceProvider();
var main = provider.GetRequiredService<GameMain>();
main.Run();
