using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rubemori_Car : MonoBehaviour
{
    public TrailRenderer[] trailRenderers;
    private Rigidbody playerRB;
    public WheelColliders3 colliders;
    public WheelMeshes3 wheelMeshes;
    public WheelParticles3 wheelParticles;
    public float gasInput;
    public float brakeInput;
    public float steeringInput;
    public GameObject smokePrefab;
    public float motorPower = 800f;
    public float brakePower = 400f;
    public float slipAngle;
    private float speed;
    public AnimationCurve steeringCurve;

    public float jumpForce = 5f;  // Controla la altura del salto
    public bool isGrounded;
    public float groundDistance = 0.5f;

    public float brakeSmoothness = 2f;      // Controla qu� tan suave es la transici�n al frenar
    public float maxSpeed = 250f;            // Velocidad m�xima del veh�culo
    public AnimationCurve accelerationCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float boostAccelerationValue = 1000f;

    public float staticFrictionValue = 1.2f;   // Valor alto para fricci�n est�tica
    public float dynamicFrictionValue = 0.6f;  // Valor bajo para fricci�n din�mica
    public float speedThreshold = 0.1f;        // Umbral para determinar si el coche est� en movimiento

    private string currentSurface = "Normal";
    // Start is called before the first frame update

    void Start()
    {
        playerRB = gameObject.GetComponent<Rigidbody>();
        InstantiateSmoke();
        foreach (TrailRenderer trail in trailRenderers)
        {
            trail.emitting = false;
        }
    }
    void FixedUpdate()
    {
        ApplyDriftControl();
    }
    void InstantiateSmoke()
    {
        // Part�culas de humo para las ruedas
        wheelParticles.FRWheel = Instantiate(smokePrefab, colliders.FRWheel.transform.position - Vector3.up * colliders.FRWheel.radius, Quaternion.identity, colliders.FRWheel.transform)
            .GetComponent<ParticleSystem>();
        wheelParticles.FLWheel = Instantiate(smokePrefab, colliders.FLWheel.transform.position - Vector3.up * colliders.FRWheel.radius, Quaternion.identity, colliders.FLWheel.transform)
            .GetComponent<ParticleSystem>();
        wheelParticles.RRWheel = Instantiate(smokePrefab, colliders.RRWheel.transform.position - Vector3.up * colliders.FRWheel.radius, Quaternion.identity, colliders.RRWheel.transform)
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
        AdjustWheelFrictionBasedOnSpeed(); // Ajusta la fricci�n basado en la velocidad actual
        CheckDrifting();
        isGrounded = Physics.Raycast(transform.position, -Vector3.up, colliders.FRWheel.radius + 0.1f);
        DetectSurface();
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("Space pressed");
            Jump();
        }
        if (Input.GetKey(KeyCode.LeftShift))  // Boost temporal
        {
            playerRB.AddForce(transform.forward * boostAccelerationValue, ForceMode.Acceleration);
        }
    }
    void DetectSurface()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1f))
        {
            // Aqu� puedes agregar m�s l�gica para diferentes tipos de superficies
            if (hit.collider.CompareTag("Dirt"))
            {
                currentSurface = "Dirt";
                AdjustFrictionForSurface(0.8f, 0.4f); // Valores de fricci�n para tierra
            }
            else if (hit.collider.CompareTag("Wood"))
            {
                currentSurface = "Wood";
                AdjustFrictionForSurface(0.5f, 0.3f); // Valores de fricci�n para madera
            }
            else
            {
                currentSurface = "Normal";
                AdjustFrictionForSurface(staticFrictionValue, dynamicFrictionValue); // Valores por defecto
            }
        }
    }

    void AdjustFrictionForSurface(float staticFriction, float dynamicFriction)
    {
        this.staticFrictionValue = staticFriction;
        this.dynamicFrictionValue = dynamicFriction;
    }
    void CheckInput()
    {
        gasInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");

        // Calcula el �ngulo de deslizamiento (slip angle)
        slipAngle = Vector3.Angle(transform.forward, playerRB.velocity - transform.forward);

        // Frena si el auto est� en reversa y el jugador presiona gas, o viceversa
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
    void CheckDrifting()
    {
        // Realizar un raycast hacia abajo desde el centro del coche
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1f))
        {
            // Verificamos si estamos tocando el suelo
            if (hit.collider != null && hit.distance < 1f) // Ajusta la distancia seg�n sea necesario
            {
                // Supongamos que un deslizamiento ocurre si el �ngulo de deslizamiento es mayor a un umbral
                if (slipAngle > 10f) // Ajusta este umbral seg�n lo que consideres como "derrape"
                {
                    StartEmitting(); // Comienza a emitir marcas
                }
                else
                {
                    StopEmitting(); // Detiene la emisi�n de marcas
                }
            }
            else
            {
                StopEmitting(); // Si no estamos en el suelo, detener la emisi�n
            }
        }
        else
        {
            StopEmitting(); // Si el raycast no golpea nada, detener la emisi�n
        }
    }
    void StartEmitting()
    {
        foreach (TrailRenderer trail in trailRenderers)
        {
            trail.emitting = true; // Activar el emisor
        }
    }

    void StopEmitting()
    {
        foreach (TrailRenderer trail in trailRenderers)
        {
            trail.emitting = false; // Desactivar el emisor
        }
    }
    void ApplyDriftControl()
    {
        // Detectamos la velocidad lateral del auto
        Vector3 lateralVelocity = transform.InverseTransformDirection(playerRB.velocity).x * Vector3.right;

        // Aplicamos una fuerza estabilizadora para reducir el derrape lateral
        playerRB.AddForce(-lateralVelocity * 1.0f, ForceMode.Acceleration); // Ajusta este valor seg�n la estabilidad deseada

        // Aplica una resistencia adicional cuando no se acelera
        if (gasInput == 0)
        {
            playerRB.AddForce(-playerRB.velocity * 0.05f, ForceMode.Acceleration); // Ajusta 0.1f seg�n sea necesario
        }
    }
    void ApplyBrake()
    {
        float speedFactor = Mathf.Clamp01(speed / maxSpeed);
        float dynamicBrakePower = brakePower * (1 + speedFactor); // Aumenta el poder de frenado con la velocidad

        // Freno en las ruedas delanteras y trasera
        colliders.FRWheel.brakeTorque = brakeInput * dynamicBrakePower * 0.5f;
        colliders.FLWheel.brakeTorque = brakeInput * dynamicBrakePower * 0.5f;
        colliders.RRWheel.brakeTorque = brakeInput * dynamicBrakePower;
    }

    void ApplyMotor()
    {
        float speed = playerRB.velocity.magnitude;
        float accelerationFactor = accelerationCurve.Evaluate(speed / maxSpeed);
        float motorTorque = motorPower * gasInput * accelerationFactor;

        if (gasInput > 0)
        {
            colliders.RRWheel.motorTorque = motorTorque * 1.5f;
        }
        else if (gasInput < 0 && speed > 1f)
        {
            colliders.RRWheel.brakeTorque = brakePower * brakeSmoothness;
        }
        else
        {
            colliders.RRWheel.motorTorque = 0;
            colliders.RRWheel.brakeTorque = brakePower * 1.5f; // Aumenta el brakePower para frenar m�s efectivamente
        }

        // Aplica menos torque a las ruedas delanteras para evitar perder control
        colliders.FRWheel.motorTorque = motorTorque * 0.7f;
        colliders.FLWheel.motorTorque = motorTorque * 0.7f;
        if (Vector3.Angle(Vector3.up, transform.up) > 15f)  // Detecta si el auto est� en una colina
        {
            motorTorque += 3000f;  // Aplica un boost de fuerza en pendientes
        }

    }

    void ApplySteering()
    {
        // Direcci�n en las ruedas delanteras
        float speedFactor = Mathf.Clamp01(speed / maxSpeed);
        float steeringAngle = steeringInput * steeringCurve.Evaluate(speed) * (1 - speedFactor);
        if (slipAngle < 90f)
        {
            steeringAngle += Vector3.SignedAngle(transform.forward, playerRB.velocity + transform.forward, Vector3.up);
        }

        steeringAngle = Mathf.Clamp(steeringAngle, -45f, 45f);  // Limitar el �ngulo de direcci�n
        colliders.FRWheel.steerAngle = steeringAngle;
        colliders.FLWheel.steerAngle = steeringAngle;
    }

    void ApplyWheelPositions()
    {
        // Ajusta la fricci�n lateral en las ruedas traseras basado en la velocidad
        AdjustRearWheelFriction();

        // Actualiza la posici�n y rotaci�n de las ruedas seg�n la f�sica del WheelCollider
        UpdateWheel(colliders.FRWheel, wheelMeshes.FRWheel);
        UpdateWheel(colliders.FLWheel, wheelMeshes.FLWheel);
        UpdateWheel(colliders.RRWheel, wheelMeshes.RRWheel);
    }
    void AdjustWheelFrictionBasedOnSpeed()
    {
        float speed = playerRB.velocity.magnitude; // Obtiene la magnitud de la velocidad actual

        // Determina si el coche est� en reposo o en movimiento
        if (speed < speedThreshold)
        {
            // Si est� en reposo, aplicar fricci�n est�tica
            SetWheelFriction(staticFrictionValue);
        }
        else
        {
            // Si est� en movimiento, aplicar fricci�n din�mica
            SetWheelFriction(dynamicFrictionValue);
        }
    }
    void SetWheelFriction(float frictionValue)
    {
        WheelFrictionCurve forwardFriction = new WheelFrictionCurve();
        WheelFrictionCurve sidewaysFriction = new WheelFrictionCurve();

        WheelFrictionCurve forwardFriction2 = new WheelFrictionCurve();
        WheelFrictionCurve sidewaysFriction2 = new WheelFrictionCurve();

        // Ajustar la fricci�n hacia adelante y lateral de las ruedas
        forwardFriction.extremumSlip = 0.4f;
        forwardFriction.extremumValue = 1f;
        forwardFriction.asymptoteSlip = 0.8f;
        forwardFriction.asymptoteValue = 0.6f;
        forwardFriction.stiffness = Mathf.Lerp(1.2f, 1.5f, speed / maxSpeed * 1.25f);

        sidewaysFriction.extremumSlip = 0.20f;
        sidewaysFriction.extremumValue = 1.1f;
        sidewaysFriction.asymptoteSlip = 0.5f;
        sidewaysFriction.asymptoteValue = 0.8f;
        sidewaysFriction.stiffness = 1.55f;

        //ajuste de rueda trasera 
        forwardFriction2.extremumSlip = 0.5f;
        forwardFriction2.extremumValue = 0.9f;
        forwardFriction2.asymptoteSlip = 1.0f;
        forwardFriction2.asymptoteValue = 0.4f;
        forwardFriction2.stiffness = Mathf.Lerp(1.0f, 1.2f, speed / maxSpeed);

        sidewaysFriction2.extremumSlip = 0.25f;
        sidewaysFriction2.extremumValue = 1;
        sidewaysFriction2.asymptoteSlip = 0.6f;
        sidewaysFriction2.asymptoteValue = 0.7f;
        sidewaysFriction2.stiffness = 1.4f;
        // Aplica las curvas de fricci�n a las ruedas delanteras y traseras
        colliders.FRWheel.forwardFriction = forwardFriction;
        colliders.FRWheel.sidewaysFriction = sidewaysFriction;
        colliders.FLWheel.forwardFriction = forwardFriction;
        colliders.FLWheel.sidewaysFriction = sidewaysFriction;
        colliders.RRWheel.forwardFriction = forwardFriction2;
        colliders.RRWheel.sidewaysFriction = sidewaysFriction2;
    }
    void AdjustWheelFriction()
    {
        Debug.Log("AdjustWheelFriction called!");
        WheelFrictionCurve lateralFriction = colliders.FRWheel.sidewaysFriction;
        WheelFrictionCurve forwardFriction = colliders.FRWheel.forwardFriction;

        // Ajusta la fricci�n lateral para mejorar el control en las curvas
        lateralFriction.stiffness = Mathf.Lerp(1.0f, 0.6f, speed / maxSpeed); // Ajusta el rango seg�n tus necesidades
        colliders.FRWheel.sidewaysFriction = lateralFriction;
        colliders.FLWheel.sidewaysFriction = lateralFriction;

        // Ajusta la fricci�n hacia adelante para mejorar la tracci�n
        forwardFriction.stiffness = Mathf.Lerp(1.0f, 0.8f, speed / maxSpeed); // Ajusta el rango seg�n tus necesidades
        colliders.FRWheel.forwardFriction = forwardFriction;
        colliders.FLWheel.forwardFriction = forwardFriction;
    }
    void AdjustRearWheelFriction()
    {
        WheelFrictionCurve lateralFriction = colliders.RRWheel.sidewaysFriction;
        lateralFriction.stiffness = Mathf.Lerp(1.5f, 0.5f, speed / maxSpeed);  // Ajusta los valores seg�n el comportamiento deseado
        colliders.RRWheel.sidewaysFriction = lateralFriction;
    }

    void CheckParticles()
    {
        WheelHit[] wheelHits = new WheelHit[3];
        colliders.FRWheel.GetGroundHit(out wheelHits[0]);
        colliders.FLWheel.GetGroundHit(out wheelHits[1]);
        colliders.RRWheel.GetGroundHit(out wheelHits[2]);

        float slipAllowance = 0.7f;  // Aumentar para que las part�culas se activen menos f�cilmente
        // Revisi�n de las part�culas de derrape
        if ((Mathf.Abs(wheelHits[0].sidewaysSlip) + Mathf.Abs(wheelHits[0].forwardSlip) > slipAllowance))
        {
            Debug.Log("Derrape en la rueda delantera derecha");
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
        { // Aseg�rate de que el auto est� tocando el suelo
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
public class WheelColliders3
{
    public WheelCollider FRWheel;
    public WheelCollider FLWheel;
    public WheelCollider RRWheel;
}

[System.Serializable]
public class WheelMeshes3
{
    public MeshRenderer FRWheel;
    public MeshRenderer FLWheel;
    public MeshRenderer RRWheel;
}

[System.Serializable]
public class WheelParticles3
{
    public ParticleSystem FRWheel;
    public ParticleSystem FLWheel;
    public ParticleSystem RRWheel;
}
