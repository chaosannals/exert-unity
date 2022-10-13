using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTankServer.Services;

public class GameServices : BackgroundService
{
    private ILogger<GameServices> logger;
    private GameServer server;

    public GameServices(
        IConfiguration configuration,
        ILogger<GameServices> logger,
        GameRoomManager roomManager,
        GamePlayerManager playerManager,
        GameClientManager clientManager
    ) {
        var port = configuration.GetValue<int>("Port");
        this.logger = logger;
        server = new GameServer(port, roomManager, playerManager, clientManager);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}
