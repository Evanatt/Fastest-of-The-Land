using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rubemori_Car_Controller_Final : MonoBehaviour
{
    private Rigidbody playerRB;
    public WheelColliders3Rube colliders;
    public WheelMeshes3Rube wheelMeshes;
    public float gasInput;
    public float brakeInput;
    public float steeringInput;
    public float motorPower = 800f;
    public float brakePower = 400f;
    public float maxSpeed = 250f;
    public AnimationCurve accelerationCurve = AnimationCurve.Linear(0, 0, 1, 1);

    // Fricción
    public float staticFrictionValue = 1.2f;
    public float dynamicFrictionValue = 0.6f;
    public float dirtFrictionValue = 0.4f; // Fricción para la tierra
    public float woodFrictionValue = 0.8f; // Fricción para la madera
    public float currentSurfaceFriction;

    // Salto
    public float jumpForce = 5f;
    public bool isGrounded;

    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        isGrounded = CheckIfGrounded();
        CheckInput();
        ApplyMotor();
        ApplySteering();
        ApplyBrake();
        AdjustWheelFriction();
        ApplyWheelPositions();
        Debug.Log(playerRB.velocity);
    }

    void CheckInput()
    {
        gasInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");
        brakeInput = (gasInput < 0) ? Mathf.Abs(gasInput) : 0;
    }

    void ApplyMotor()
    {
        float speed = playerRB.velocity.magnitude;
        float accelerationFactor = accelerationCurve.Evaluate(speed / maxSpeed);

        float motorTorque = motorPower * gasInput * accelerationFactor;

        // Aplicar torque a las ruedas traseras
        colliders.RRWheel.motorTorque = motorTorque;
        colliders.FRWheel.motorTorque = motorTorque * 0.2f; // Menos torque en las delanteras

        // Limitar velocidad máxima
        if (speed > maxSpeed / 3.6f) // Convertir km/h a m/s
        {
            playerRB.velocity = playerRB.velocity.normalized * (maxSpeed / 3.6f);
        }

        Debug.Log("Motor Torque: " + motorTorque);
    }

    void ApplySteering()
    {
        float speedFactor = Mathf.Clamp01(playerRB.velocity.magnitude / maxSpeed);
        float steeringAngle = steeringInput * (1 - speedFactor) * 30f; // Máximo 30 grados
        colliders.FRWheel.steerAngle = steeringAngle;
        colliders.FLWheel.steerAngle = steeringAngle;
    }

    void ApplyBrake()
    {
        colliders.FRWheel.brakeTorque = brakeInput * brakePower;
        colliders.FLWheel.brakeTorque = brakeInput * brakePower;
        colliders.RRWheel.brakeTorque = brakeInput * brakePower;
    }

    void AdjustWheelFriction()
    {
        WheelFrictionCurve forwardFriction = new WheelFrictionCurve
        {
            extremumSlip = 0.4f,
            extremumValue = currentSurfaceFriction,
            asymptoteSlip = 0.8f,
            asymptoteValue = currentSurfaceFriction * 0.75f
        };

        WheelFrictionCurve sidewaysFriction = new WheelFrictionCurve
        {
            extremumSlip = 0.2f,
            extremumValue = currentSurfaceFriction,
            asymptoteSlip = 0.5f,
            asymptoteValue = currentSurfaceFriction * 0.75f
        };

        // Aplicar la fricción a las ruedas
        colliders.FRWheel.forwardFriction = forwardFriction;
        colliders.FRWheel.sidewaysFriction = sidewaysFriction;
        colliders.FLWheel.forwardFriction = forwardFriction;
        colliders.FLWheel.sidewaysFriction = sidewaysFriction;
        colliders.RRWheel.forwardFriction = forwardFriction;
        colliders.RRWheel.sidewaysFriction = sidewaysFriction;
    }

    bool CheckIfGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, colliders.FRWheel.radius + 0.2f); // Ajustar la distancia
    }

    public void Jump()
    {
        if (isGrounded)
        {
            playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dirt"))
        {
            SetSurfaceFriction("dirt");
        }
        else if (other.CompareTag("Wood"))
        {
            SetSurfaceFriction("wood");
        }
    }

    public void SetSurfaceFriction(string surfaceType)
    {
        switch (surfaceType)
        {
            case "dirt":
                currentSurfaceFriction = dirtFrictionValue;
                break;
            case "wood":
                currentSurfaceFriction = woodFrictionValue;
                break;
            default:
                currentSurfaceFriction = staticFrictionValue; // Por defecto
                break;
        }
    }

    void ApplyWheelPositions()
    {
        UpdateWheel(colliders.FRWheel, wheelMeshes.FRWheel);
        UpdateWheel(colliders.FLWheel, wheelMeshes.FLWheel);
        UpdateWheel(colliders.RRWheel, wheelMeshes.RRWheel);
    }

    void UpdateWheel(WheelCollider coll, MeshRenderer wheelMesh)
    {
        coll.GetWorldPose(out Vector3 position, out Quaternion rotation);
        wheelMesh.transform.position = position;
        wheelMesh.transform.rotation = rotation;
    }
}

[System.Serializable]
public class WheelColliders3Rube
{
    public WheelCollider FRWheel;
    public WheelCollider FLWheel;
    public WheelCollider RRWheel;
}

[System.Serializable]
public class WheelMeshes3Rube
{
    public MeshRenderer FRWheel;
    public MeshRenderer FLWheel;
    public MeshRenderer RRWheel;
}



