using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tank
{
    public class TankController : MonoBehaviour
    {
        public float speed;
        public float rotateSpeed;
        public Transform muzzle;
        public TankBullet bullet;

        void Start()
        {

        }

        void Update()
        {

        }

        void Fire()
        {
            TankBullet one = Instantiate(bullet, muzzle.position,Quaternion.identity);
            one.direction = muzzle.forward;
            one.ownerid = GetInstanceID();
        }
    }
}