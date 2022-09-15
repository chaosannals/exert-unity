using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using ServerDemo;

try
{
    var ioc = new ServiceCollection();
    var cb = new ConfigurationBuilder();
    var cnf = cb.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .Add(new JsonConfigurationSource
        {
            Path = "appsettings.json",
            ReloadOnChange = true,
        }).Build();
    var path = cnf.GetRequiredSection("Logger:PathFormat");

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.File(
            path: path?.ToString() ?? "Logs/S-{Date}.log",
            rollingInterval: RollingInterval.Day,
            rollOnFileSizeLimit: true,
            fileSizeLimitBytes: 2000000,
            flushToDiskInterval: TimeSpan.FromSeconds(10),
            restrictedToMinimumLevel: LogEventLevel.Information,
            outputTemplate: "[{Timestamp:yy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        )
        .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
        .CreateLogger();

    ioc.AddSingleton(op => cnf);
    ioc.AddSingleton(op => Log.Logger);
    ioc.AddSingleton(op =>
    {
        return new GameServer();
    });

    Log.Information("start server");
    var provider = ioc.BuildServiceProvider();
    var server = provider.GetRequiredService<GameServer>();
    server.Serve();
}
catch (Exception e)
{
    Log.Error("Exception: {0}", e);
}