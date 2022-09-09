using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTankServer;

public class GamePlayerManager
{
    public ConcurrentDictionary<long, GamePlayer> players;

    public GamePlayerManager()
    {
        players = new ConcurrentDictionary<long, GamePlayer>();
    }

}
