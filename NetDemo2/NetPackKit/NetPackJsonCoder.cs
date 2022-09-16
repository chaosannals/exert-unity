using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NetPackKit;

public class NetPackJsonCoder : INetPackCoder
{
    public object Decode(byte[] bytes, string kind)
    {
        var type = Type.GetType(kind);
        var text = Encoding.UTF8.GetString(bytes);
        return JsonSerializer.Deserialize(text, type!)!;
    }

    public byte[] Encode(object value, string kind)
    {
        var type = Type.GetType(kind);
        var text = JsonSerializer.Serialize(value, type!);
        return Encoding.UTF8.GetBytes(text);
    }
}
