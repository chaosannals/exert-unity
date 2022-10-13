using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetTankServer;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(cd =>
    {
        cd.SetBasePath(Directory.GetCurrentDirectory());
        cd.AddEnvironmentVariables(prefix: "NET_TANK_");
    })
    .ConfigureLogging((hc, cl) =>
    {
        cl.AddFile(hc.Configuration.GetSection("Logger"));
        cl.AddConsole();
        cl.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
    })
    .ConfigureServices((hc, services) =>
    {
        services.AddDbContext<GameDbContext>(oa =>
        {
            var cs = hc.Configuration.GetConnectionString("Main");
            oa.UseMySQL(cs);
        });
        services.AddSingleton<GameRoomManager>();
        services.AddSingleton<GamePlayerManager>();
        services.AddSingleton(op =>
        {
            var id = hc.Configuration.GetValue<int>("Id");
            return new GameClientManager(id);
        });
    })
    .UseConsoleLifetime()
    .Build();

host.Run();
