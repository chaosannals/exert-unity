using System;
using System.Net.Sockets;
using DemoCommon;
using DemoCommon.Messages;
using Serilog;

namespace ServerDemo;

public class GameClientState
{
    public Socket? Socket { get; set; }
    public byte[] Buffer { get; set; }
    public GameMessageReader Reader { get; private set; }

    public GameClientState()
    {
        Buffer = new byte[1024];
        Reader = new GameMessageReader();
        //Reader.ReadHead += (h) =>
        //{
        //    Log.Information("hhhh: {0}", h?.kindName);
        //};
        //Reader.ReadBody += (b) =>
        //{
        //    Log.Information("bbbb: {0}", b?.GetType().Name);
        //};
        Reader.Error += (b, h, e) =>
        {
            Log.Warning("error: {0}, {1}, {2}", b?.Size, h?.kindName, e);
        };
    }
}
