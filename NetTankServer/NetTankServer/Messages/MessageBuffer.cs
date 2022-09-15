using System;

namespace NetTankServer.Messages;

public class MessageBuffer
{
    public int Size { get; private set; }
    public int Capacity { get { return Data.Length; } }
    public byte[] Data { get; private set; }

    public MessageBuffer(byte[] data)
    {
        Data = data;
        Size = 0;
    }

    public MessageBuffer(byte[]data, int start, int length)
    {
        Data = new byte[length];
        Size = length;
        Array.Copy(data, start, Data, 0, length);
    }

    public void Write(byte[] data, int start, int length)
    {
        if (length > (Capacity - Size))
        {
            var nb = new byte[Data.Length + length];
            Array.Copy(Data, 0, nb, 0, Size);
            Data = nb;
        }
        Array.Copy(data, start, Data, Size, length);
        Size += length;
    }

    public void Drop(int size)
    {
        Size -= size;
        Array.Copy(Data, size, Data, 0, Size);
    }

    public override string ToString()
    {
        return $"[{Size} / {Capacity}]";
    }

    public string ToDebugString()
    {
        return $"{ToString()} D: {BitConverter.ToString(Data, 0, Size)}";
    }
}
