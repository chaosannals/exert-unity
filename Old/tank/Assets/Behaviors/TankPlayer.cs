using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tank
{
    [ExecuteInEditMode]
    public class TankPlayer : MonoBehaviour
    {
        public Camera shot;
        public TankController controller;

        void Start()
        {
            if (controller)
            {
                Camera.SetupCurrent(shot);
                transform.position = controller.transform.position;
            }
        }
        
        void Update()
        {
            if (controller)
            {
                transform.position = controller.transform.position;
                Vector3 direction = new Vector3();
                if (Input.GetKey(KeyCode.W)) direction.z = 1.0f;
                else if (Input.GetKey(KeyCode.S)) direction.z = -1.0f;
                if (Input.GetKey(KeyCode.A)) direction.x = -1.0f;
                else if (Input.GetKey(KeyCode.D)) direction.x = 1.0f;
                direction = direction.normalized;
                controller.transform.forward = Vector3.Lerp(controller.transform.forward, direction, Time.deltaTime * controller.rotateSpeed).normalized;
            }
        }
    }
}