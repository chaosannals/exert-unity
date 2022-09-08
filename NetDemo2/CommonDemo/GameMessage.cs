using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CommonDemo;

public class GameMessage
{
    public GameMessageKind Kind { get; set; }
    public long PlayerId { get; set; }

    public string ToJsonString()
    {
        return JsonSerializer.Serialize(this);
    }
}
