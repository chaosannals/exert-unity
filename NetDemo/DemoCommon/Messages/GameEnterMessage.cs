using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoCommon.Messages;

public class GameEnterMessage : GameBaseMessage
{
    public long playerId { get; set; }
}
