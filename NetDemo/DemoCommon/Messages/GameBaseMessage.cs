using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DemoCommon.Messages;

public abstract class GameBaseMessage
{
    public const string MAGIC = "TANK";
    public static readonly byte[] MagicBytes = Encoding.UTF8.GetBytes(MAGIC);
    public static readonly int HeadSize = MagicBytes.Length + 8;

    public byte[] Encode()
    {
        var kind = Encoding.UTF8.GetBytes(GetType().Name);
        var ksize = ToBigEndian(kind.Length);
        var json = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(this, GetType()));
        var jsize = ToBigEndian(json.Length);
        var result = new byte[MagicBytes.Length + ksize.Length + jsize.Length + kind.Length + json.Length];
        Array.Copy(MagicBytes, 0, result, 0, MagicBytes.Length);
        Array.Copy(ksize, 0, result, MagicBytes.Length, ksize.Length);
        Array.Copy(jsize, 0, result, MagicBytes.Length + ksize.Length, jsize.Length);
        Array.Copy(kind, 0, result, MagicBytes.Length + ksize.Length + jsize.Length, kind.Length);
        Array.Copy(json, 0, result, MagicBytes.Length + ksize.Length + jsize.Length + kind.Length, json.Length);
        return result;
    }

    public static GameMessageHead? HeadOf(byte[] data)
    {
        var magic = data.Take(MagicBytes.Length);
        if (!magic.SequenceEqual(MagicBytes))
        {
            return null;
        }

        var ksize = FromBigEndianInt32(data.Skip(MagicBytes.Length).Take(4).ToArray());
        var jsize = FromBigEndianInt32(data.Skip(MagicBytes.Length + 4).Take(4).ToArray());
        var kind = Encoding.UTF8.GetString(data.Skip(MagicBytes.Length + 8).Take(ksize).ToArray());
        return new GameMessageHead
        {
            kindSize = ksize,
            jsonSize = jsize,
            kindName = kind,
        };
    }

    public static object? BodyOf(GameMessageHead head, byte[] data)
    {
        var json = Encoding.UTF8.GetString(data.Skip(HeadSize + head.kindSize).Take(head.jsonSize).ToArray());
        var type = Type.GetType($"DemoCommon.Messages.{head.kindName}");
        if (type is null)
        {
            return null;
        }
        return JsonSerializer.Deserialize(json, type);
    }

    public static T? BodyOf<T>(GameMessageHead head, byte[] data) where T : GameBaseMessage
    {
        return BodyOf(head, data) as T;
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
