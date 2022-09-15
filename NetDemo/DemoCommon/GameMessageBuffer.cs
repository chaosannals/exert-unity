using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoCommon;

public class GameMessageBuffer
{
    public byte[] Data { get; private set; }
    public int Size { get; private set; }
    public int Capacity { get { return Data.Length; } }
    public int Remain { get { return Capacity - Size; } }

    public GameMessageBuffer(byte[] data, int length)
    {
        Data = data;
        Size = length;
    }

    public void Write(byte[] data, int start, int length)
    {
        if (length > Remain)
        {
            var ns = (Capacity + length) << 1;
            var nb = new byte[ns];
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
