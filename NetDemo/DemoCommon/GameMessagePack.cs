using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoCommon.Messages;

namespace DemoCommon;

public class GameMessagePack
{
    public GameMessageHead Head { get; init; }
    public GameBaseMessage Body { get; init; }

    public GameMessagePack(GameMessageHead head, GameBaseMessage body)
    {
        Head = head;
        Body = body;
    }
}
