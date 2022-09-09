using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTankServer;

public class GameRoom
{
    public long Id { get; init; }

    public ConcurrentDictionary<long, GamePlayer> players;

    public GameRoom(long id)
    {
        Id = id;
        players = new ConcurrentDictionary<long, GamePlayer>();
    }
}
