using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fighting.Archives;

namespace Fighting
{
    /// <summary>
    /// 角色控制器，游戏角色控制。
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Animator))]
    public class RoleController : MonoBehaviour
    {
        public Role role = new Role();
        public Vector4 bottom;

        private Rigidbody body;
        private Animator animator;

        public bool grounded { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        public void Move(Vector3 directory)
        {
            directory *= role.speed;
            directory.y = body.velocity.y;
            body.velocity = directory;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Jump()
        {
            Vector3 velocity = body.velocity;
            velocity.y += role.jump;
            body.velocity = velocity;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// 
        /// </summary>
        private void FixedUpdate()
        {
            Vector3 position = transform.position;
            position.y += bottom.w;
            grounded = Physics.CheckBox(position, bottom * 0.49f);
            animator.SetBool("Grounded", grounded);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Vector3 position = transform.position;
            position.y += bottom.w;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(position, bottom);
        }
    }
}

