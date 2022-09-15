using System;

namespace NetTankServer.Messages;

public class MessageReader
{
    private MessageBuffer buffer;
    private MessageHead? head;
    private int? totalLength;

    public MessageReader()
    {
        buffer = new MessageBuffer(new byte[1024]);
    }

    public MessagePack? Read(byte[] data, int start, int length)
    {
        buffer.Write(data, start, length);

        if (head == null)
        {
            head = BaseMessage.HeadOf(buffer.Data);
        }

        if (head != null)
        {
            totalLength = BaseMessage.HeadSize + head.kindSize + head.jsonSize;
        }

        if (totalLength != null && buffer.Size > totalLength)
        {
            var r = BaseMessage.BodyOf(head!, buffer.Data);

            if (r != null)
            {
                buffer.Drop(totalLength.Value);
                head = null;
                totalLength = null;
                return new MessagePack(head!, (r as BaseMessage)!);
            }
        }

        return null;
    }
}
