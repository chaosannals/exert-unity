using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDemoCommon;

public class GameDataBuffer
{
    public int Head { get; set; }
    public int Tail { get; set; }

    public int Size { get { return Tail - Head; } }
    public int Capacity { get { return Data.Length; } }

    public byte[] Data { get; private set; }

    public GameDataBuffer(byte[] data)
    {
        Data = data;
        Head = 0;
        Tail = data.Length;
    }

    public GameDataBuffer() :this(new byte[1024])
    {
    }

    public override string ToString()
    {
        return $"H: {Head} T:{Tail} S:{Size}";
    }

    public string Debug()
    {
        return $"{ToString()} D: {BitConverter.ToString(Data, Head, Size)}";
    }

}
