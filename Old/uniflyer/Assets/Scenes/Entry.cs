using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Uniflyer
{
    /// <summary>
    /// 入口界面场景控制类，提供该场景的调度方法。
    /// </summary>
    public class Entry : MonoBehaviour
    {
        [SerializeField]
        private Main main = null;
        [SerializeField]
        private Button loadButton = null;

        void Start()
        {
            loadButton.interactable = main.HasSave();
        }

        public void Play()
        {
            SceneManager.LoadScene("Centre");
        }

        public void Load()
        {
            main.Load();
            SceneManager.LoadScene("Centre");
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}