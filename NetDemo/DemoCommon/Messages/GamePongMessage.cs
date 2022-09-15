using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoCommon.Messages;

public class GamePongMessage : GameBaseMessage
{
    public DateTime createAt = DateTime.Now;
}
