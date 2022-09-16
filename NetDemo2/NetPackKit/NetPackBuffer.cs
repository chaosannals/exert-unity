using System;
using System.Text;

namespace NetPackKit;

public class NetPackBuffer
{
    public byte[] Data { get; private set; }
    public int Size { get; private set; }
    public int Capacity { get { return Data.Length; } }
    public int Remain { get { return Capacity - Size; } }

    public INetPackCoder Coder { get; set; }

    private int? nowKindSize;
    private int? nowBodySize;
    private int? nowSize;

    public NetPackBuffer(byte[] data, int length)
    {
        Data = data;
        Size = length;
        nowKindSize = null;
        nowBodySize = null;
        nowSize = null;
        Coder = new NetPackJsonCoder();
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

    public NetPack? Read()
    {
        if (NetPack.HeadSize >= Size)
        {
            return null;
        }

        if (nowKindSize is null)
        {
            nowKindSize = FromBigEndianInt32(Data.Skip(NetPack.Magic.Length).Take(4).ToArray());
        }

        if (nowBodySize is null)
        {
            nowBodySize = FromBigEndianInt32(Data.Skip(NetPack.Magic.Length + 4).Take(4).ToArray());
        }

        if (nowSize is null)
        {
            nowSize = NetPack.HeadSize + nowKindSize + nowBodySize;
        }

        if (nowSize >= Size)
        {
            return null;
        }

        var nowKind = Encoding.UTF8.GetString(Data.Skip(NetPack.HeadSize).Take(nowKindSize!.Value).ToArray());
        var nowBody = Data.Skip(NetPack.HeadSize + nowKindSize!.Value).Take(nowBodySize!.Value).ToArray();

        var body = Coder.Decode(nowBody, nowKind);

        // 读出的数据清除
        Size -= nowSize!.Value;
        Array.Copy(Data, nowSize!.Value, Data, 0, Size);
        nowKindSize = null;
        nowBodySize = null;
        nowSize = null;

        return new NetPack(
            nowKindSize!.Value,
            nowBodySize!.Value,
            nowKind,
            body
        );
    }

    public override string ToString()
    {
        return $"[{Size} / {Capacity}]";
    }

    public string ToDebugString()
    {
        return $"{ToString()} D: {BitConverter.ToString(Data, 0, Size)}";
    }

    public static int FromBigEndianInt32(byte[] data)
    {
        if (BitConverter.IsLittleEndian)
        {
            data = data.Reverse().ToArray();
        }
        return BitConverter.ToInt32(data);
    }

    public static byte[] ToBigEndian(int v)
    {
        var result = BitConverter.GetBytes(v);
        if (BitConverter.IsLittleEndian)
        {
            result = result.Reverse().ToArray();
        }
        return result;
    }
}
