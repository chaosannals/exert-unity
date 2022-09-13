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

    /// <summary>
    /// TODO Start != 0 有 bug 待处理。
    /// </summary>
    /// <param name="data"></param>
    /// <param name="length"></param>
    public void Write(byte[] data, int length)
    {
        if (length > (Capacity - Size))
        {
            var nb = new byte[Data.Length + length];
            Array.Copy(Data, 0, nb, Start, Size);
            Data = nb;
        }
        Array.Copy(data, 0, Data, End, length);
        End += length;
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