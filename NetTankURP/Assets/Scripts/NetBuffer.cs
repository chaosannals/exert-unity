using System;

public class NetBuffer
{
    public int Start { get; private set; }
    public int End { get; private set; }

    public int Size { get { return End - Start; } }
    public int Capacity { get { return Data.Length; } }

    public byte[] Data { get; private set; }

    public NetBuffer(byte[] data)
    {
        Data = data;
        Start = 0;
        End = data.Length;
    }

    public NetBuffer() : this(new byte[1024])
    {
    }

    public void Skip(int v)
    {
        Start += v;
    }

    public override string ToString()
    {
        return $"[{Start} - {End}]({Size} / {Capacity})";
    }

    public string ToDebugString()
    {
        return $"{ToString()} D: {BitConverter.ToString(Data, Start, Size)}";
    }

}