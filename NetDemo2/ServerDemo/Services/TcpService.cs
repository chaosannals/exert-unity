using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetPackKit;
using System.Net;
using System.Net.Sockets;

namespace ServerDemo.Services;

public class TcpService : BackgroundService
{
    private NetPackServer server;
    private ILogger<TcpService> logger;

    public TcpService(IConfiguration configuration, ILogger<TcpService> logger, INetPackDispatcher dispatcher)
    {
        var port = configuration.GetValue<int>("Tcp:Port");
        var host = configuration.GetValue<string>("Tcp:Host");
        server = new NetPackServer(dispatcher, port, host);
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var ipEntry = Dns.GetHostEntry(Dns.GetHostName());
        var ips = ipEntry.AddressList
            .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        logger.LogInformation("tcp listen {0}:{1}({2})", server.Host, server.Port, ips);
        await server.ServeAsync(stoppingToken);
        logger.LogInformation("tcp server end.");
    }
}
