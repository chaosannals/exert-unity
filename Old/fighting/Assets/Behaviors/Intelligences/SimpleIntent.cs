using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fighting.Intelligences
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(RoleController))]
    public class SimpleIntent : MonoBehaviour
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
            
        }
    }
}
