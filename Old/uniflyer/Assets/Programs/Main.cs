using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Uniflyer
{
    /// <summary>
    /// 主类，游戏的主要控制中心。
    /// </summary>
    public class Main : MonoBehaviour
    {
        public string SavePath { get; private set; }
        public Archive Archive { get; private set; }

        private void Awake()
        {
            SavePath = Application.persistentDataPath + "/Flyer.archive";
        }

        public bool HasSave()
        {
            return Archive.Level > 1;
        }

        public void Save()
        {
            Archive.Save(SavePath);
        }

        public void Load()
        {
            Archive = Archive.Load(SavePath);
        }
    }
}
