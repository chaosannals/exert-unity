using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetPackKit;

public interface INetPackCoder
{
    public object Decode(byte[] bytes, string kind);
    public byte[] Encode(object value, string kind);
}
