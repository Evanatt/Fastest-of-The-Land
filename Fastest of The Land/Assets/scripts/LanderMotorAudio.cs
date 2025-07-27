using UnityEngine;

public class LanderMotorAudio : MonoBehaviour
{
    public Rigidbody rb; // Referencia al Rigidbody
    public AudioSource motorSource;
    public float minPitch = 0.8f;
    public float maxPitch = 2.0f;
    public float maxSpeed = 50f;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float speed = rb.velocity.magnitude;

        // Normaliza la velocidad para el pitch
        float normalizedSpeed = Mathf.Clamp01(speed / maxSpeed);
        float targetPitch = Mathf.Lerp(minPitch, maxPitch, normalizedSpeed);

        motorSource.pitch = targetPitch;
    }
}

