using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using NetTankServer;
using Serilog.Events;
using Serilog;

var ioc = new ServiceCollection();
var cb = new ConfigurationBuilder();
var cnf = cb.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .Add(new JsonConfigurationSource
    {
        Path = "appsettings.json",
        ReloadOnChange = true,
    }).Build();
ioc.AddSingleton(op => cnf);
ioc.AddSingleton<ILogger>(op =>
{
    var path = cnf.GetRequiredSection("Logger:PathFormat");
    return new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.File(
                path: path?.ToString() ?? "Logs/S-{Date}.log",
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: int.Parse(cnf.GetSection("Logger:FileSizeLimitBytes")?.Value ?? "2000000"),
                retainedFileCountLimit: int.Parse(cnf.GetSection("Logger:RetainedFileCountLimit")?.Value ?? "31"),
                flushToDiskInterval: TimeSpan.FromSeconds(10),
                outputTemplate: cnf.GetSection("Logger:OutputTemplate")?.Value ?? "[{Timestamp:yy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
            )
            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
            .CreateLogger();
});
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
    return new GameClientManager(44);
});
ioc.AddDbContext<GameDbContext>(opb =>
{
    var connetionString = cnf.GetConnectionString("Main");
    opb.UseMySQL(connetionString);
});

ioc.AddSingleton(op =>
{
    var rm = op.GetRequiredService<GameRoomManager>();
    var pm = op.GetRequiredService<GamePlayerManager>();
    var cm = op.GetRequiredService<GameClientManager>();
    var lgr = op.GetRequiredService<ILogger>();
    return new GameServer(44444, rm, pm, cm, lgr);
});

ioc.AddSingleton(op =>
{
    var srv = op.GetRequiredService<GameServer>();
    var lgr = op.GetRequiredService<ILogger>();
    var main = new GameMain(srv, lgr!);
    return main;
});

var provider = ioc.BuildServiceProvider();
var main = provider.GetRequiredService<GameMain>();
main.Run();
