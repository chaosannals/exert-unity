using System;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace Fighting.Archives
{
    /// <summary>
    /// 角色信息，所有角色的通用信息都在这里。
    /// </summary>
    [XmlType]
    [Serializable]
    public class Role
    {
        [XmlAttribute]
        public float health;
        [XmlAttribute]
        public float jump;
        [XmlAttribute]
        public float speed;
    }
}

