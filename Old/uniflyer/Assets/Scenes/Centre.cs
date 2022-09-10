using UnityEngine;
using UnityEngine.UI;

namespace Uniflyer
{
    public class Centre : MonoBehaviour
    {
        [SerializeField]
        private Main main = null;
        [SerializeField]
        private Text score = null;
        [SerializeField]
        private Flyer flyer = null;

        private Flyer gamer = null;

        void Start()
        {
            if (gamer == null)
            {
                gamer = Instantiate(flyer);
            }
        }

        void Update()
        {
            score.text = main.Archive.Score.ToString();
        }
    }
}