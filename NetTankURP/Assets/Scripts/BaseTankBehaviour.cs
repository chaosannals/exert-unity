using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseTankBehaviour : MonoBehaviour
{
    private Rigidbody body;
    private DateTime fireLastAt;
    public float fireSpan = 2.0f;
    public float speed = 1.0f;
    public float rotateSpeed = 1.0f;
    public GameObject battery;
    public float batteryAngle = 0f;
    public float batteryRotateSpeed = 20.0f;
    public GameObject mark;
    public int markNumber;
    public Transform muzzle;
    public GameObject bullet;


    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    [ExecuteInEditMode]
    void Start()
    {
        Mark();
    }

    void Update()
    {
        
    }

    public void RotateRight()
    {
        if (battery)
        {
            batteryAngle += batteryRotateSpeed * Time.deltaTime;
            var f = Quaternion.AngleAxis(batteryAngle, Vector3.up) * transform.forward;
            battery.transform.forward = f.normalized;
        }
    }

    public void RotateLeft()
    {
        if (battery)
        {
            batteryAngle -= batteryRotateSpeed * Time.deltaTime;
            var f = Quaternion.AngleAxis(batteryAngle, Vector3.up) * transform.forward;
            battery.transform.forward = f.normalized;
        }
    }

    public void Mark()
    {
        if (mark)
        {
            var mr = mark.GetComponent<MeshRenderer>();
            mr.material.color = markNumber switch
            {
                1 => Color.red,
                2 => Color.green,
                3 => Color.blue,
                _ => Color.black,
            };
        }
    }

    public void Fire()
    {
        var now = DateTime.Now;
        var d = (now - fireLastAt).TotalMilliseconds / 1000.0f;
        if (battery && d >= fireSpan)
        {
            var b = Instantiate(bullet,muzzle.position, muzzle.rotation);
            var bb = b.GetComponent<BaseBulletBehaviour>();
            bb.playerId = markNumber;
            fireLastAt = now;
        }
    }

    public void Move(Vector3 direction)
    {
        var v = direction * speed;
        v.y = body.velocity.y;
        body.velocity = v;
        transform.forward = Vector3.Lerp(
            transform.forward,
            direction,
            Time.deltaTime * rotateSpeed
        ).normalized;
    }
}
