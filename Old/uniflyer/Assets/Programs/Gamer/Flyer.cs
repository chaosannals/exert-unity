using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Uniflyer
{
    public class Flyer : MonoBehaviour
    {
        [SerializeField]
        private Cabin cabin = null;
        [SerializeField]
        private float speed = 1.0f;

        public bool visible = false;
        public float updateTime = 0.0f;
        public float renderTime = 0.0f;

        void Start()
        {

        }

        void Update()
        {
            visible = renderTime != updateTime;
            updateTime = renderTime;
            if (visible)
            {
                transform.Translate(cabin.Arrow * Time.deltaTime * speed);
            }
        }

        void OnWillRenderObject()
        {
            renderTime = Time.time;
        }
    }
}