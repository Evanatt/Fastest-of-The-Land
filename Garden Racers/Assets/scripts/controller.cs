using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class controller : MonoBehaviour
{

    public WheelCollider[] whells = new WheelCollider[4];
    public GameObject[] whellsMesh = new GameObject[4];
    public float motorTorque = 200;
    public float steeringMax = 4;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void FixedUpdate()
    {
        animatewheel();
        if (Input.GetKeyUp(KeyCode.W))
        {
            for (int i = 0; i < whells.Length; i++)
            {
                whells[i].motorTorque = motorTorque;
            }
        }
        /*else
        {
            for (int i = 0; i < whells.Length; i++)
            {
                whells[i].motorTorque = 0;
            }
        }*/
        if (Input.GetAxis("Horizontal")!=0) {
            for (int i = 0; i < whells.Length -2; i++)
            {
                whells[i].steerAngle = Input.GetAxis("Horizontal")*steeringMax;
            }
        }
        else
        {
            for (int i = 0; i < whells.Length - 2; i++)
            {
                whells[i].steerAngle = 0;
            }
        }

    }
    void animatewheel()
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;
        for (int i = 0; i < 4; i++) {
            whells[i].GetWorldPose(out wheelPosition, out wheelRotation);
            whellsMesh[i].transform.position = wheelPosition;
            whellsMesh[i].transform.rotation = wheelRotation;

        }

    }
}


