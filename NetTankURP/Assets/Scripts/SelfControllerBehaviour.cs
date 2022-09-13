using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfControllerBehaviour : MonoBehaviour
{
    public BaseTankBehaviour tank;
    public Camera shot;

    private void Start()
    {
        NetClient.PlayerEnter += (m) =>
        {

        };

        if (shot != null && tank != null)
        {
            Camera.SetupCurrent(shot);
            transform.position = tank.transform.position;
        }
    }

    private void Update()
    {
        if (tank)
        {
            transform.position = tank.transform.position;

            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            Vector3 direction = new Vector3(x, 0.0f, y).normalized;
            tank.Move(direction);

            if (Input.GetKey(KeyCode.Q))
            {
                tank.RotateLeft();
            }
            else if (Input.GetKey(KeyCode.E))
            {
                tank.RotateRight();
            }

            if (Input.GetKey(KeyCode.Space))
            {
                tank.Fire();
            }
        }
    }
}
