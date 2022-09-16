using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;
using Serilog;
using NetPackKit;
using ServerDemo;
using System.Net.Sockets;
using System.Net;

var ioc = new ServiceCollection();
var cb = new ConfigurationBuilder();
var cnf = cb.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .Add(new JsonConfigurationSource
    {
        Path = "appsettings.json",
        ReloadOnChange = true,
    }).Build();
ioc.AddSingleton(op => cnf);
ioc.AddDbContext<DemoDbContext>(opb =>
{
    var connetionString = cnf.GetConnectionString("Main");
    opb.UseMySQL(connetionString);
});
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

ioc.AddSingleton<INetPackDispatcher>(op => new DemoDispatcher());
ioc.AddSingleton(op =>
{
    var dispatcher = op.GetRequiredService<INetPackDispatcher>();
    return new NetPackServer(dispatcher, 44444);
});

var provider = ioc.BuildServiceProvider();
var server = provider.GetRequiredService<NetPackServer>();
var logger = provider.GetRequiredService<ILogger>();
var ipEntry = Dns.GetHostEntry(Dns.GetHostName());
foreach (var ip in ipEntry.AddressList)
{
    if (ip.AddressFamily == AddressFamily.InterNetwork)
    {
        logger.Information("listen: {0}:{1}", ip, server.Port);
    }
}
server.Serve().Wait();