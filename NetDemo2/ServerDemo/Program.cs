using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetPackKit;
using ServerDemo;
using ServerDemo.Services;

var host = Host.CreateDefaultBuilder(args)
    .UseConsoleLifetime()
    .ConfigureHostConfiguration(cd =>
    {
        cd.SetBasePath(Directory.GetCurrentDirectory());
        cd.AddEnvironmentVariables(prefix: "NET_DEMO_2_");
    })
    .ConfigureLogging((hc, cl) =>
    {
        cl.AddFile(hc.Configuration.GetSection("LoggingFile"));
        cl.AddConsole();
        cl.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
    })
    .ConfigureServices((hc, services) =>
    {
        services.AddDbContext<DemoDbContext>(opb =>
        {
            var connetionString = hc.Configuration.GetConnectionString("Main");
            opb.UseMySQL(connetionString);
        });
        services.AddSingleton<INetPackDispatcher>(op => new DemoDispatcher());
        services.AddHostedService<TcpService>();
    })
    .Build();

host.Run();
