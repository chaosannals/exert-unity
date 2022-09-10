using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Fighting.Archives;

namespace Fighting
{
    /// <summary>
    /// 主角控制器。
    /// 选取被控制的角色，通过捕获玩家输入，控制角色的行为。
    /// </summary>
    [RequireComponent(typeof(RoleController))]
    public class Protagonist : MonoBehaviour
    {
        private RoleController controller;

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            controller = GetComponent<RoleController>();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            if (controller.grounded)
            {
                Vector3 direction = new Vector3(x, 0.0f, y).normalized;
                controller.Move(direction);
                if (Input.GetButtonDown("Jump"))
                {
                    controller.Jump();
                }
            }
        }
    }
}

