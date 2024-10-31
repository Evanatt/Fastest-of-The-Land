using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotatez_Molino : MonoBehaviour
{
    public float speedRotz = 11f;
    void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, 1f * speedRotz) * Time.deltaTime);
    }
}
