using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdGen;

namespace NetTankServer;

public  class GameRoomManager
{
    private IdGenerator idGenerator;
    private ConcurrentDictionary<long, GameRoom> rooms;

    public GameRoomManager(int id=1000)
    {
        idGenerator = new IdGenerator(id);
        rooms = new ConcurrentDictionary<long, GameRoom>();
    }

    public  GameRoom NewRoom()
    {
        long nid = idGenerator.CreateId();
        return rooms.AddOrUpdate(nid, (id) =>
        {
            return new GameRoom(id);
        }, (id, old) =>
        {
            // TODO 如果进入此分支，ID生成重复。
            return old;
        });
    }

    public  GameRoom? DropRoom(long id)
    {
        GameRoom? room;
        if (rooms.Remove(id, out room))
        {
            // TODO
            return room;
        }
        return null;
    }

    public  GameRoom? GetRoom(long id)
    {
        if (rooms.ContainsKey(id))
        {
            return rooms[id];
        }
        return null;
    }
}
