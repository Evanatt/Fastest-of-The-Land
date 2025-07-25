using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Luz_Lander_Car_Controller : MonoBehaviour
{
    private Rigidbody playerRB;
    public WheelColliders colliders;
    public WheelMeshes wheelMeshes;
    public WheelParticles wheelParticles;
    public float gasInput;
    public float brakeInput;
    public float steeringInput;
    public GameObject smokePrefab;
    public float motorPower;
    public float brakePower;
    public float slipAngle;
    private float speed;
    public AnimationCurve steeringCurve;

    public float jumpForce = 5f;  // Controla la altura del salto
    public bool isGrounded;
    public float groundDistance = 0.5f;

    public float boostAccelerationValue = 1000f;
    public float speedEma = 1.9f;

    // Start is called before the first frame update
    void Start()
    {
        playerRB = gameObject.GetComponent<Rigidbody>();
        InstantiateSmoke();
    }

    void InstantiateSmoke()
    {
        wheelParticles.FRWheel = Instantiate(smokePrefab, colliders.FRWheel.transform.position - Vector3.up * colliders.FRWheel.radius, Quaternion.identity, colliders.FRWheel.transform)
            .GetComponent<ParticleSystem>();
        wheelParticles.FLWheel = Instantiate(smokePrefab, colliders.FLWheel.transform.position - Vector3.up * colliders.FRWheel.radius, Quaternion.identity, colliders.FLWheel.transform)
            .GetComponent<ParticleSystem>();
        wheelParticles.RRWheel = Instantiate(smokePrefab, colliders.RRWheel.transform.position - Vector3.up * colliders.FRWheel.radius, Quaternion.identity, colliders.RRWheel.transform)
            .GetComponent<ParticleSystem>();
        wheelParticles.RLWheel = Instantiate(smokePrefab, colliders.RLWheel.transform.position - Vector3.up * colliders.FRWheel.radius, Quaternion.identity, colliders.RLWheel.transform)
            .GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        speed = playerRB.velocity.magnitude;
        CheckInput();
        ApplyMotor();
        ApplySteering();
        ApplyBrake();
        CheckParticles();
        ApplyWheelPositions();
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("Space pressed");
            Jump();
        }
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.Z))  // Boost temporal
        {
            playerRB.AddForce(transform.forward * boostAccelerationValue, ForceMode.Acceleration);
        }
        aceleracion2();
    }
    public void aceleracion2()
    {
        speed = speed + playerRB.velocity.magnitude * speedEma;
        playerRB.AddForce(transform.forward * speedEma, ForceMode.Acceleration);
    }

    void CheckInput()
    {
        // Using keyboard input for gas, brake, and steering
        gasInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");

        // Calculate the slip angle
        slipAngle = Vector3.Angle(transform.forward, playerRB.velocity - transform.forward);

        // Fixed code to brake even after going in reverse
        float movingDirection = Vector3.Dot(transform.forward, playerRB.velocity);
        if (movingDirection < -0.5f && gasInput > 0)
        {
            brakeInput = Mathf.Abs(gasInput);
        }
        else if (movingDirection > 0.5f && gasInput < 0)
        {
            brakeInput = Mathf.Abs(gasInput);
        }
        else
        {
            brakeInput = 0;
        }
    }

    void ApplyBrake()
    {
        colliders.FRWheel.brakeTorque = brakeInput * brakePower * 0.7f;
        colliders.FLWheel.brakeTorque = brakeInput * brakePower * 0.7f;

        colliders.RRWheel.brakeTorque = brakeInput * brakePower * 0.3f;
        colliders.RLWheel.brakeTorque = brakeInput * brakePower * 0.3f;
    }

    void ApplyMotor()
    {
        colliders.RRWheel.motorTorque = motorPower * gasInput;
        colliders.RLWheel.motorTorque = motorPower * gasInput;
    }

    void ApplySteering()
    {
        float steeringAngle = steeringInput * steeringCurve.Evaluate(speed);
        if (slipAngle < 120f)
        {
            steeringAngle += Vector3.SignedAngle(transform.forward, playerRB.velocity + transform.forward, Vector3.up);
        }
        steeringAngle = Mathf.Clamp(steeringAngle, -90f, 90f);
        colliders.FRWheel.steerAngle = steeringAngle;
        colliders.FLWheel.steerAngle = steeringAngle;
    }

    void ApplyWheelPositions()
    {
        UpdateWheel(colliders.FRWheel, wheelMeshes.FRWheel);
        UpdateWheel(colliders.FLWheel, wheelMeshes.FLWheel);
        UpdateWheel(colliders.RRWheel, wheelMeshes.RRWheel);
        UpdateWheel(colliders.RLWheel, wheelMeshes.RLWheel);
    }

    void CheckParticles()
    {
        WheelHit[] wheelHits = new WheelHit[4];
        colliders.FRWheel.GetGroundHit(out wheelHits[0]);
        colliders.FLWheel.GetGroundHit(out wheelHits[1]);
        colliders.RRWheel.GetGroundHit(out wheelHits[2]);
        colliders.RLWheel.GetGroundHit(out wheelHits[3]);

        float slipAllowance = 0.5f;
        if ((Mathf.Abs(wheelHits[0].sidewaysSlip) + Mathf.Abs(wheelHits[0].forwardSlip) > slipAllowance))
        {
            wheelParticles.FRWheel.Play();
        }
        else
        {
            wheelParticles.FRWheel.Stop();
        }
        if ((Mathf.Abs(wheelHits[1].sidewaysSlip) + Mathf.Abs(wheelHits[1].forwardSlip) > slipAllowance))
        {
            wheelParticles.FLWheel.Play();
        }
        else
        {
            wheelParticles.FLWheel.Stop();
        }
        if ((Mathf.Abs(wheelHits[2].sidewaysSlip) + Mathf.Abs(wheelHits[2].forwardSlip) > slipAllowance))
        {
            wheelParticles.RRWheel.Play();
        }
        else
        {
            wheelParticles.RRWheel.Stop();
        }
        if ((Mathf.Abs(wheelHits[3].sidewaysSlip) + Mathf.Abs(wheelHits[3].forwardSlip) > slipAllowance))
        {
            wheelParticles.RLWheel.Play();
        }
        else
        {
            wheelParticles.RLWheel.Stop();
        }
    }

    void UpdateWheel(WheelCollider coll, MeshRenderer wheelMesh)
    {
        Quaternion quat;
        Vector3 position;
        coll.GetWorldPose(out position, out quat);
        wheelMesh.transform.position = position;
        wheelMesh.transform.rotation = quat;
    }

    void Jump()
    {
        if (IsGrounded())
        { // Asegúrate de que el auto esté tocando el suelo
            playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            Debug.Log("Jumping");
        }
    }
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, groundDistance);
    }
}

[System.Serializable]
public class WheelColliders2
{
    public WheelCollider FRWheel;
    public WheelCollider FLWheel;
    public WheelCollider RRWheel;
    public WheelCollider RLWheel;
}

[System.Serializable]
public class WheelMeshes2
{
    public MeshRenderer FRWheel;
    public MeshRenderer FLWheel;
    public MeshRenderer RRWheel;
    public MeshRenderer RLWheel;
}

[System.Serializable]
public class WheelParticles2
{
    public ParticleSystem FRWheel;
    public ParticleSystem FLWheel;
    public ParticleSystem RRWheel;
    public ParticleSystem RLWheel;
}

