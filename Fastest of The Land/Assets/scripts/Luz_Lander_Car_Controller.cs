using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Luz_Lander_Car_Controller : MonoBehaviour
{
    public TrailRenderer[] trailRenderers;
    private Rigidbody playerRB;
    public WheelColliders colliders;
    public WheelMeshes wheelMeshes;
    public WheelParticles wheelParticles;
    public float gasInput;
    public float brakeInput;
    public float steeringInput;
    public GameObject smokePrefab;
    public float motorPower = 800f;
    public float brakePower = 400f;
    public float slipAngle;
    private float speed;
    public AnimationCurve steeringCurve;

    public float jumpForce = 5f;
    public bool isGrounded;
    public float groundDistance = 0.5f;

    // Nuevas variables para mejorar el comportamiento
    public float brakeSmoothness = 2f;
    public float maxSpeed = 250f;
    public AnimationCurve accelerationCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float boostAccelerationValue = 1000f;

    public float staticFrictionValue = 1.2f;
    public float dynamicFrictionValue = 0.6f;
    public float speedThreshold = 0.1f;

    private string currentSurface = "Normal";
    public float speedEma = 1.9f;

    // Variables para el sistema de rebote y reversa mejorada
    public float wallBounceForce = 10f;
    public float wallRedirectForce = 5f;
    public float reverseMultiplier = 0.8f;
    private bool isStuckAgainstWall = false;
    private float stuckTimer = 0f;
    private Vector3 lastValidDirection;
    private float lastSpeedCheck = 0f;
    public LayerMask wallLayerMask = -1; // Por defecto detecta todo

    void Start()
    {
        playerRB = gameObject.GetComponent<Rigidbody>();
        InstantiateSmoke();
        lastValidDirection = transform.forward;

        // Inicializar trail renderers si existen
        if (trailRenderers != null)
        {
            foreach (TrailRenderer trail in trailRenderers)
            {
                trail.emitting = false;
            }
        }
    }

    void FixedUpdate()
    {
        ApplyDriftControl();
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

    void Update()
    {
        // Usar la misma conversión de velocidad que Rubemori
        speed = playerRB.velocity.magnitude * 5.919f;

        CheckInput();
        CheckWallCollision(); // Nuevo sistema de detección de paredes
        ApplyMotor();
        ApplySteering();
        ApplyBrake();
        CheckParticles();
        ApplyWheelPositions();

        // Agregar sistemas de fricción dinámicos
        AdjustWheelFrictionBasedOnSpeed();
        CheckDrifting();

        // Mejorar detección de suelo
        isGrounded = Physics.Raycast(transform.position, -Vector3.up, colliders.FRWheel.radius + 0.1f);
        DetectSurface();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("Space pressed");
            Jump();
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.Z))
        {
            playerRB.AddForce(transform.forward * boostAccelerationValue, ForceMode.Acceleration);
        }

        aceleracion2();

        // Actualizar dirección válida cuando nos movemos bien
        if (playerRB.velocity.magnitude > 1f && !isStuckAgainstWall)
        {
            lastValidDirection = playerRB.velocity.normalized;
        }
    }

    public void aceleracion2()
    {
        speed = speed + playerRB.velocity.magnitude * speedEma;
        playerRB.AddForce(transform.forward * speedEma, ForceMode.Acceleration);
    }

    void DetectSurface()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1f))
        {
            if (hit.collider.CompareTag("Dirt"))
            {
                currentSurface = "Dirt";
                AdjustFrictionForSurface(0.8f, 0.4f);
            }
            else if (hit.collider.CompareTag("Wood"))
            {
                currentSurface = "Wood";
                AdjustFrictionForSurface(0.5f, 0.3f);
            }
            else
            {
                currentSurface = "Normal";
                AdjustFrictionForSurface(staticFrictionValue, dynamicFrictionValue);
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

        // Calcular el ángulo de deslizamiento igual que Rubemori
        slipAngle = Vector3.Angle(transform.forward, playerRB.velocity - transform.forward);

        // Mejorar la lógica de frenado - REMOVIDA para permitir reversa
        float movingDirection = Vector3.Dot(transform.forward, playerRB.velocity);

        // Solo frenar si estamos atascados contra una pared
        if (isStuckAgainstWall && Mathf.Abs(gasInput) > 0)
        {
            // Si estamos atascados, permitir cambio de dirección más fácil
            if ((movingDirection > 0.1f && gasInput < 0) || (movingDirection < -0.1f && gasInput > 0))
            {
                brakeInput = Mathf.Abs(gasInput) * 0.5f; // Freno más suave cuando estamos atascados
            }
            else
            {
                brakeInput = 0;
            }
        }
        else
        {
            // Comportamiento normal - permitir reversa libremente
            brakeInput = 0;
        }
    }

    void CheckDrifting()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1f))
        {
            if (hit.collider != null && hit.distance < 1f)
            {
                if (slipAngle > 10f)
                {
                    StartEmitting();
                }
                else
                {
                    StopEmitting();
                }
            }
            else
            {
                StopEmitting();
            }
        }
        else
        {
            StopEmitting();
        }
    }

    void StartEmitting()
    {
        if (trailRenderers != null)
        {
            foreach (TrailRenderer trail in trailRenderers)
            {
                trail.emitting = true;
            }
        }
    }

    void StopEmitting()
    {
        if (trailRenderers != null)
        {
            foreach (TrailRenderer trail in trailRenderers)
            {
                trail.emitting = false;
            }
        }
    }

    void ApplyDriftControl()
    {
        // Sistema de control de derrape mejorado como Rubemori
        Vector3 lateralVelocity = transform.InverseTransformDirection(playerRB.velocity).x * Vector3.right;
        playerRB.AddForce(-lateralVelocity * 1.0f, ForceMode.Acceleration);

        // Resistencia cuando no se acelera
        if (gasInput == 0)
        {
            playerRB.AddForce(-playerRB.velocity * 0.05f, ForceMode.Acceleration);
        }
    }

    void ApplyBrake()
    {
        // Sistema de frenado dinámico como Rubemori
        float speedFactor = Mathf.Clamp01(speed / maxSpeed);
        float dynamicBrakePower = brakePower * (1 + speedFactor);

        // Distribución de frenado más equilibrada
        colliders.FRWheel.brakeTorque = brakeInput * dynamicBrakePower * 0.5f;
        colliders.FLWheel.brakeTorque = brakeInput * dynamicBrakePower * 0.5f;
        colliders.RRWheel.brakeTorque = brakeInput * dynamicBrakePower;
        colliders.RLWheel.brakeTorque = brakeInput * dynamicBrakePower;
    }

    void ApplyMotor()
    {
        float currentSpeed = playerRB.velocity.magnitude;
        Vector3 vehicleVelocity = playerRB.velocity;
        float movingDirection = Vector3.Dot(transform.forward, vehicleVelocity);

        // Factor de aceleración basado en curva
        float accelerationFactor = accelerationCurve.Evaluate(currentSpeed / maxSpeed);
        float motorTorque = motorPower * gasInput * accelerationFactor;

        // SISTEMA MEJORADO DE MOTOR - PERMITE REVERSA REAL
        if (gasInput > 0) // Intentando ir hacia adelante
        {
            // Movimiento normal hacia adelante - tracción trasera principal
            colliders.RRWheel.motorTorque = motorTorque * 1.2f;
            colliders.RLWheel.motorTorque = motorTorque * 1.2f;
            colliders.FRWheel.motorTorque = motorTorque * 0.3f;
            colliders.FLWheel.motorTorque = motorTorque * 0.3f;

            // Liberar frenos cuando aceleramos hacia adelante
            if (!isStuckAgainstWall)
            {
                colliders.RRWheel.brakeTorque = 0;
                colliders.RLWheel.brakeTorque = 0;
                colliders.FRWheel.brakeTorque = 0;
                colliders.FLWheel.brakeTorque = 0;
            }
        }
        else if (gasInput < 0) // Intentando ir hacia atrás - REVERSA REAL
        {
            float reverseTorque = motorTorque * reverseMultiplier; // Reversa un poco más lenta

            colliders.RRWheel.motorTorque = reverseTorque * 1.2f;
            colliders.RLWheel.motorTorque = reverseTorque * 1.2f;
            colliders.FRWheel.motorTorque = reverseTorque * 0.3f;
            colliders.FLWheel.motorTorque = reverseTorque * 0.3f;

            // Liberar frenos cuando vamos en reversa
            if (!isStuckAgainstWall)
            {
                colliders.RRWheel.brakeTorque = 0;
                colliders.RLWheel.brakeTorque = 0;
                colliders.FRWheel.brakeTorque = 0;
                colliders.FLWheel.brakeTorque = 0;
            }
        }
        else // Sin input
        {
            colliders.RRWheel.motorTorque = 0;
            colliders.RLWheel.motorTorque = 0;
            colliders.FRWheel.motorTorque = 0;
            colliders.FLWheel.motorTorque = 0;

            // Freno de motor suave cuando no hay input
            colliders.RRWheel.brakeTorque = brakePower * 0.3f;
            colliders.RLWheel.brakeTorque = brakePower * 0.3f;
        }

        // Boost en pendientes
        if (Vector3.Angle(Vector3.up, transform.up) > 15f)
        {
            float hillBoost = 3000f * Mathf.Sign(gasInput);
            colliders.RRWheel.motorTorque += hillBoost * 0.6f;
            colliders.RLWheel.motorTorque += hillBoost * 0.6f;
        }
    }

    void ApplySteering()
    {
        // Sistema de dirección mejorado
        float speedFactor = Mathf.Clamp01(speed / maxSpeed);
        float steeringAngle = steeringInput * steeringCurve.Evaluate(speed) * (1 - speedFactor * 0.7f);

        // Corrección de dirección basada en velocidad
        if (slipAngle < 90f)
        {
            steeringAngle += Vector3.SignedAngle(transform.forward, playerRB.velocity + transform.forward, Vector3.up) * 0.5f;
        }

        steeringAngle = Mathf.Clamp(steeringAngle, -45f, 45f);
        colliders.FRWheel.steerAngle = steeringAngle;
        colliders.FLWheel.steerAngle = steeringAngle;
    }

    void ApplyWheelPositions()
    {
        AdjustRearWheelFriction();

        UpdateWheel(colliders.FRWheel, wheelMeshes.FRWheel);
        UpdateWheel(colliders.FLWheel, wheelMeshes.FLWheel);
        UpdateWheel(colliders.RRWheel, wheelMeshes.RRWheel);
        UpdateWheel(colliders.RLWheel, wheelMeshes.RLWheel);
    }

    void AdjustWheelFrictionBasedOnSpeed()
    {
        float speed = playerRB.velocity.magnitude;

        if (speed < speedThreshold)
        {
            SetWheelFriction(staticFrictionValue);
        }
        else
        {
            SetWheelFriction(dynamicFrictionValue);
        }
    }

    void SetWheelFriction(float frictionValue)
    {
        // Configuración de fricción para ruedas delanteras
        WheelFrictionCurve forwardFriction = new WheelFrictionCurve();
        WheelFrictionCurve sidewaysFriction = new WheelFrictionCurve();

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

        // Configuración de fricción para ruedas traseras
        WheelFrictionCurve forwardFriction2 = new WheelFrictionCurve();
        WheelFrictionCurve sidewaysFriction2 = new WheelFrictionCurve();

        forwardFriction2.extremumSlip = 0.5f;
        forwardFriction2.extremumValue = 0.9f;
        forwardFriction2.asymptoteSlip = 1.0f;
        forwardFriction2.asymptoteValue = 0.4f;
        forwardFriction2.stiffness = Mathf.Lerp(1.0f, 1.2f, speed / maxSpeed);

        sidewaysFriction2.extremumSlip = 0.25f;
        sidewaysFriction2.extremumValue = 1f;
        sidewaysFriction2.asymptoteSlip = 0.6f;
        sidewaysFriction2.asymptoteValue = 0.7f;
        sidewaysFriction2.stiffness = 1.4f;

        // Aplicar fricción
        colliders.FRWheel.forwardFriction = forwardFriction;
        colliders.FRWheel.sidewaysFriction = sidewaysFriction;
        colliders.FLWheel.forwardFriction = forwardFriction;
        colliders.FLWheel.sidewaysFriction = sidewaysFriction;
        colliders.RRWheel.forwardFriction = forwardFriction2;
        colliders.RRWheel.sidewaysFriction = sidewaysFriction2;
        colliders.RLWheel.forwardFriction = forwardFriction2;
        colliders.RLWheel.sidewaysFriction = sidewaysFriction2;
    }

    void AdjustRearWheelFriction()
    {
        WheelFrictionCurve lateralFriction = colliders.RRWheel.sidewaysFriction;
        lateralFriction.stiffness = Mathf.Lerp(1.5f, 0.5f, speed / maxSpeed);
        colliders.RRWheel.sidewaysFriction = lateralFriction;
        colliders.RLWheel.sidewaysFriction = lateralFriction;
    }

    void CheckParticles()
    {
        WheelHit[] wheelHits = new WheelHit[4];
        colliders.FRWheel.GetGroundHit(out wheelHits[0]);
        colliders.FLWheel.GetGroundHit(out wheelHits[1]);
        colliders.RRWheel.GetGroundHit(out wheelHits[2]);
        colliders.RLWheel.GetGroundHit(out wheelHits[3]);

        float slipAllowance = 0.7f; // Usar el mismo valor que Rubemori

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
        {
            playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            Debug.Log("Jumping");
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, groundDistance);
    }

    // NUEVO SISTEMA DE DETECCIÓN Y REBOTE CONTRA PAREDES
    void CheckWallCollision()
    {
        // Raycast hacia adelante para detectar paredes
        RaycastHit frontHit;
        bool hitFront = Physics.Raycast(transform.position, transform.forward, out frontHit, 2f, wallLayerMask);

        // Raycast hacia atrás para detectar paredes
        RaycastHit backHit;
        bool hitBack = Physics.Raycast(transform.position, -transform.forward, out backHit, 2f, wallLayerMask);

        // Detectar si estamos atascados
        float currentSpeedMagnitude = playerRB.velocity.magnitude;
        bool isMovingSlowly = currentSpeedMagnitude < 2f;
        bool isTryingToMove = Mathf.Abs(gasInput) > 0.1f;

        if ((hitFront || hitBack) && isMovingSlowly && isTryingToMove)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer > 0.5f) // Después de 0.5 segundos atascado
            {
                isStuckAgainstWall = true;

                if (hitFront && gasInput > 0) // Chocando hacia adelante
                {
                    ApplyWallBounce(frontHit);
                }
                else if (hitBack && gasInput < 0) // Chocando hacia atrás
                {
                    ApplyWallBounce(backHit);
                }
            }
        }
        else
        {
            stuckTimer = 0f;
            isStuckAgainstWall = false;
        }

        // Detectar colisiones laterales para rebote lateral
        CheckLateralWallBounce();
    }

    void ApplyWallBounce(RaycastHit hit)
    {
        // Calcular dirección de rebote
        Vector3 bounceDirection = Vector3.Reflect(transform.forward, hit.normal);
        bounceDirection.y = 0; // Mantener en el plano horizontal
        bounceDirection.Normalize();

        // Aplicar fuerza de rebote
        playerRB.AddForce(bounceDirection * wallBounceForce, ForceMode.Impulse);

        // Redireccionar el auto hacia una dirección libre
        Vector3 redirectDirection = FindBestEscapeDirection(hit.point);
        if (redirectDirection != Vector3.zero)
        {
            playerRB.AddForce(redirectDirection * wallRedirectForce, ForceMode.Acceleration);
            // Rotar suavemente hacia la dirección de escape
            Quaternion targetRotation = Quaternion.LookRotation(redirectDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
        }

        Debug.Log("Wall bounce applied! Direction: " + bounceDirection);
    }

    void CheckLateralWallBounce()
    {
        // Raycast a los lados para detectar colisiones laterales
        RaycastHit leftHit, rightHit;
        bool hitLeft = Physics.Raycast(transform.position, -transform.right, out leftHit, 1.5f, wallLayerMask);
        bool hitRight = Physics.Raycast(transform.position, transform.right, out rightHit, 1.5f, wallLayerMask);

        if (hitLeft && playerRB.velocity.magnitude > 3f)
        {
            Vector3 bounceForce = transform.right * wallBounceForce * 0.5f;
            playerRB.AddForce(bounceForce, ForceMode.Impulse);
        }

        if (hitRight && playerRB.velocity.magnitude > 3f)
        {
            Vector3 bounceForce = -transform.right * wallBounceForce * 0.5f;
            playerRB.AddForce(bounceForce, ForceMode.Impulse);
        }
    }

    Vector3 FindBestEscapeDirection(Vector3 collisionPoint)
    {
        // Probar varias direcciones para encontrar una ruta de escape
        Vector3[] testDirections = {
            transform.right,
            -transform.right,
            -transform.forward,
            (transform.right + -transform.forward).normalized,
            (-transform.right + -transform.forward).normalized,
            lastValidDirection
        };

        foreach (Vector3 direction in testDirections)
        {
            if (!Physics.Raycast(transform.position, direction, 3f, wallLayerMask))
            {
                return direction;
            }
        }

        // Si no hay escape claro, usar la última dirección válida
        return lastValidDirection;
    }

    // Método para ser llamado desde OnCollisionEnter para rebotes inmediatos
    void OnCollisionEnter(Collision collision)
    {
        // Solo rebotar si estamos moviéndonos rápido
        if (playerRB.velocity.magnitude > 5f)
        {
            Vector3 bounceDirection = Vector3.Reflect(playerRB.velocity.normalized, collision.contacts[0].normal);
            bounceDirection.y = 0;
            bounceDirection.Normalize();

            // Rebote más suave para colisiones a alta velocidad
            playerRB.AddForce(bounceDirection * wallBounceForce * 0.7f, ForceMode.Impulse);
        }
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

