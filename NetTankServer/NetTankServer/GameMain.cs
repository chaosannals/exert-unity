using Serilog;

namespace NetTankServer;

public class GameMain
{
    private GameServer server;
    private ILogger logger;

    public GameMain(GameServer server, ILogger logger)
    {
        this.server = server;
        this.logger = logger;
    }

    public void Run()
    {
        logger.Information("run");
        server.Serve().Wait();
    }
}
