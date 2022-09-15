using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTankServer.Messages;

public class MessagePack
{
    public MessageHead Head { get; private set; }
    public BaseMessage Body { get; private set; }

    public MessagePack(MessageHead head, BaseMessage body)
    {
        Head = head;
        Body = body;
    }
}
