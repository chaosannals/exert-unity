﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTankServer.Messages;

public class MessageHead
{
    public int kindSize;
    public int jsonSize;
    public string kindName = null!;
}
