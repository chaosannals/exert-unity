using System;
using UnityEngine;

public class NetMessageBuffer
{
    public int Size { get; private set; }
    public int Capacity { get { return Data.Length; } }

    public byte[] Data { get; private set; }

    public NetMessageBuffer(byte[] data, int size)
    {
        Data = data;
        Size = size;
    }

    public NetMessageBuffer() : this(new byte[1024], 0)
    {
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
            Array.Copy(Data, 0, nb, 0, Size);
            Data = nb;
        }
        Array.Copy(data, 0, Data, Size, length);
        Size += length;
    }

    public override string ToString()
    {
        return $"[{Size} / {Capacity}]";
    }

    public string ToDebugString()
    {
        return $"{ToString()} D: {BitConverter.ToString(Data)}";
    }

}
