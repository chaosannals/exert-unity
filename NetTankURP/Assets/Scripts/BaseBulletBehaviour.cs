using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseBulletBehaviour : MonoBehaviour
{
    private Rigidbody body;
    public long playerId = 0;
    public float power = 30f;
    public float speed = 10f;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    void Start()
    {
        body.velocity = transform.forward * speed;
    }

    void Update()
    {
        body.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
