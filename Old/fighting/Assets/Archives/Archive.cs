using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace Fighting.Archives
{
    /// <summary>
    /// 存档，保存和读取存档的功能。
    /// </summary>
    [XmlType]
    public class Archive
    {
        [XmlAttribute]
        public DateTime CreateDate { get; set; }
        [XmlElement]
        public DateTime ModifyDate { get; set; }
        [XmlElement]
        public Role Protagonist { get; set; }

        /// <summary>
        /// 保存存档。
        /// </summary>
        /// <param name="name">存档名</param>
        public void Save(string name)
        {
            string directory = Application.persistentDataPath + "/save";
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            string path = directory + "/" + name + ".sav";
            using (FileStream stream = File.OpenWrite(path))
            {
                XmlSerializer serializer = new XmlSerializer(GetType());
                serializer.Serialize(stream, this);
            }
        }

        /// <summary>
        /// 读取存档。
        /// </summary>
        /// <param name="name">存档名</param>
        /// <returns></returns>
        public static Archive Load(string name)
        {
            string path = Application.persistentDataPath + "/save/" + name + ".sav";
            using (FileStream stream = File.OpenRead(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Archive));
                return serializer.Deserialize(stream) as Archive;
            }
        }
    }
}
