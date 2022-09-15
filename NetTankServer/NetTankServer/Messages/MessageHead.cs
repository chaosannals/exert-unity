using System;

namespace NetTankServer.Messages;

public class MessageHead
{
    public int kindSize;
    public int jsonSize;
    public string kindName = null!;
}
