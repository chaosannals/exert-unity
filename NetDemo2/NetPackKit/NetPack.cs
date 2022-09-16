using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetPackKit;

public class NetPack
{
    public static readonly byte[] Magic = Encoding.UTF8.GetBytes("NPK\0");
    public static readonly int HeadSize = Magic.Length + 8;

    public int KindSize { get; init; }
    public int BodySize { get; init; }
    public string Kind { get; init; } = null!;
    public object Body { get; init; } = null!;

    public NetPack(int kindSize, int bodySize, string kind, object body)
    {
        KindSize = kindSize;
        BodySize = bodySize;
        Kind = kind;
        Body = body;
    }
}
