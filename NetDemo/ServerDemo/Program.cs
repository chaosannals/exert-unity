using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using ServerDemo;

try
{
    var ioc = new ServiceCollection();
    ioc.AddSingleton(op =>
    {
        return new GameServer();
    });

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.File
        (
            path: "logs/gameserver-.log",
            rollingInterval: RollingInterval.Day,
            rollOnFileSizeLimit: true,
            fileSizeLimitBytes: 2000000,
            flushToDiskInterval: TimeSpan.FromSeconds(10),
            outputTemplate: "[{Timestamp:yy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
            )
        .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
        .CreateLogger();

    Log.Information("start server");
    var provider = ioc.BuildServiceProvider();
    var server = provider.GetRequiredService<GameServer>();
    server.Serve();
}
catch (Exception e)
{
    Log.Error("Exception: {0}", e);
}