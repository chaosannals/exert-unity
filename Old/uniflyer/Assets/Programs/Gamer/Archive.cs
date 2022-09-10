using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Uniflyer
{
    [Serializable]
    public class Archive
    {
        public uint Level = 1;
        public uint Score = 0;

        public void Save(string path)
        {
            using (FileStream writer = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(writer, this);
            }
        }

        public static Archive Load(string path)
        {
            if (!File.Exists(path)) return new Archive();
            using (FileStream reader = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                IFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(reader) as Archive;
            }
        }
    }
}