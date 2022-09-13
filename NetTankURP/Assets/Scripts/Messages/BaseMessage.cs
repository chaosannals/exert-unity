using System;
using System.Text;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class BaseMessage
{
    public const string MAGIC = "TANK";
    public static readonly byte[] MagicBytes = Encoding.ASCII.GetBytes(MAGIC);

    public virtual byte[] Encode()
    {
        var kind = Encoding.UTF8.GetBytes(GetType().Name);
        var json = Encoding.UTF8.GetBytes(JsonUtility.ToJson(this));
        byte[] ksize = ToBigEndian(kind.Length);
        byte[] jsize = ToBigEndian(json.Length);
        var result = new byte[MagicBytes.Length + ksize.Length + jsize.Length + kind.Length + json.Length];
        Array.Copy(MagicBytes, 0, result, 0, MagicBytes.Length);
        Array.Copy(ksize, 0, result, MagicBytes.Length, ksize.Length);
        Array.Copy(jsize, 0, result, MagicBytes.Length + ksize.Length, jsize.Length);
        Array.Copy(kind, 0, result, MagicBytes.Length + ksize.Length + jsize.Length, kind.Length);
        Array.Copy(json, 0, result, MagicBytes.Length + ksize.Length + jsize.Length + kind.Length, json.Length);
        return result;
    }

    public static T Decode<T>(byte[] data) where T : BaseMessage
    {
        var magic = data.Take(MagicBytes.Length);
        if (!magic.SequenceEqual(MagicBytes))
        {
            return null;
        }
        var ksize = FromBigEndianInt32(data.Skip(MagicBytes.Length).Take(4).ToArray());
        var jsize = FromBigEndianInt32(data.Skip(MagicBytes.Length + 4).Take(4).ToArray());
        var kind = data.Skip(MagicBytes.Length + 8).Take(ksize).ToArray();
        var json = data.Skip(MagicBytes.Length + 8 + ksize).Take(jsize).ToArray();
        var text = Encoding.UTF8.GetString(json);
        return JsonUtility.FromJson<T>(text);
    }

    public static MessageHead HeadOf(byte[] data)
    {
        var magic = data.Take(MagicBytes.Length);
        if (!magic.SequenceEqual(MagicBytes))
        {
            return null;
        }
        var ksize = FromBigEndianInt32(data.Skip(MagicBytes.Length).Take(4).ToArray());
        var jsize = FromBigEndianInt32(data.Skip(MagicBytes.Length + 4).Take(4).ToArray());
        var kind = Encoding.UTF8.GetString(data.Skip(MagicBytes.Length + 8).Take(ksize).ToArray());
        return new MessageHead
        {
            kindSize = ksize,
            jsonSize = jsize,
            kindName = kind,
        };
    }

    public static object BodyOf(byte[] data, MessageHead head)
    {
        var type = Type.GetType(head.kindName);
        var json = Encoding.UTF8.GetString(data.Skip(MagicBytes.Length + 8 + head.kindSize).Take(head.jsonSize).ToArray());
        return JsonUtility.FromJson(json, type);
    }

    public static int FromBigEndianInt32(byte[] v)
    {
        if (BitConverter.IsLittleEndian)
        {
            v = v.Reverse().ToArray();
        }
        return BitConverter.ToInt32(v);
    }

    public static byte[] ToBigEndian(int v)
    {
        byte[] result = BitConverter.GetBytes(v);
        if (BitConverter.IsLittleEndian)
        {
            result = result.Reverse().ToArray();
        }
        return result;
    }
}
