using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace ServerDemo;

class GameClientState
{
    public Socket? Socket { get; set; }
    public byte[] Buffer { get; set; } = new byte[1024];
}
