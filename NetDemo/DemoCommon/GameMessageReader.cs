using DemoCommon.Messages;

namespace DemoCommon;

public delegate void ReadHeadHandler(GameMessageHead? head);
public delegate void ReadBodyHandler(GameBaseMessage? body);
public delegate void ErrorHandler(GameMessageBuffer? buffer, GameMessageHead? head, Exception e);

public class GameMessageReader
{
    public GameMessageBuffer Buffer { get; init; }
    public GameMessageHead? Head { get; private set; }
    public int? TotalLength { get; private set; }

    public event ReadHeadHandler? ReadHead = null;
    public event ReadBodyHandler? ReadBody = null;
    public event ErrorHandler? Error = null;
    

    public GameMessageReader()
    {
        Buffer = new GameMessageBuffer(new byte[1024], 0);
        Head = null;
        TotalLength = null;
    }

    public GameMessagePack? Read(byte[] data, int start, int length)
    {
        try
        {
            Buffer.Write(data, start, length);

            if (Head is null)
            {
                Head = GameBaseMessage.HeadOf(Buffer.Data);
                ReadHead?.Invoke(Head);
            }

            if (Head != null)
            {
                TotalLength = GameBaseMessage.HeadSize + Head.Value.kindSize + Head.Value.jsonSize;
            }

            if (TotalLength != null && Buffer.Size >= TotalLength)
            {
                var r = GameBaseMessage.BodyOf(Head!.Value, Buffer.Data);
                ReadBody?.Invoke(r as GameBaseMessage);

                if (r != null)
                {
                    Buffer.Drop(TotalLength.Value);
                    var result = new GameMessagePack(Head!.Value, (r as GameBaseMessage)!);
                    Head = null;
                    TotalLength = null;
                    return result;
                }
            }
        }
        catch (Exception e)
        {
            Error?.Invoke(Buffer, Head, e);
        }

        return null;
    }
}
