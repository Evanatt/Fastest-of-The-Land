using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bellota_manager : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public GameObject vfxPrefab;
    private Transform bellotaPoint;
    private bool isCollected = false;
    private Animator animator;
    private static readonly int consumoHash = Animator.StringToHash("consumo");
    private static readonly int brokeHash = Animator.StringToHash("broke");
    private AudioSource bellotaAudioSource;
    public AudioClip bellotaClip; // Sonido de recolección
    public AudioClip bellotapop_end_clip; // Sonido de explosión


    public delegate void BellotaCollected();
    public static event BellotaCollected OnBellotaCollected;

    private bool hasRecoleccionSoundPlayed = false; // Control de sonido de recolección

    void Start()
    {
        bellotaAudioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        if (isCollected && bellotaPoint != null)
        {
            transform.position = bellotaPoint.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            CollectBellota(other.transform);
            StartCoroutine(InvokeBellotaCollectedEvent()); // Llamar al evento con retraso
        }
    }

    public void CollectBellota(Transform playerTransform)
    {
        if (isCollected) return;

        bellotaPoint = FindBellotaPoint(playerTransform);
        if (bellotaPoint == null) return;

        isCollected = true;
        transform.SetParent(bellotaPoint);
        transform.localPosition = Vector3.zero;

        if (vfxPrefab != null)
        {
            Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        }

        StartCoroutine(PlayDestructionAnimation());
    }

    private IEnumerator PlayDestructionAnimation()
    {
        animator.SetBool(consumoHash, true);
        animator.SetTrigger(brokeHash);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }

    // Nueva corrutina para retrasar la invocación del evento
    private IEnumerator InvokeBellotaCollectedEvent()
    {
        yield return new WaitForSeconds(0.2f); // Esperar 0.2 segundos para evitar solapamiento de sonidos
        OnBellotaCollected?.Invoke();
    }

    // Este método será llamado desde el evento de la animación al principio
    public void sonidoRecoleccion()
    {
        if (!hasRecoleccionSoundPlayed) // Solo se reproduce si no se ha reproducido antes
        {
            bellotaAudioSource.PlayOneShot(bellotaClip); // Reproduce el sonido de recolección
            hasRecoleccionSoundPlayed = true; // Marcar como reproducido
        }
    }

    // Este método será llamado desde el evento de la animación cuando explote
    public void sonidoexplotaburbuja()
    {
        bellotaAudioSource.PlayOneShot(bellotapop_end_clip); // Reproduce el sonido de explosión
        hasRecoleccionSoundPlayed = false; // Restablecer el estado cuando la bellota explota
    }

    private Transform FindBellotaPoint(Transform root)
    {
        foreach (Transform child in root.GetComponentsInChildren<Transform>())
        {
            if (child.name == "BellotaPoint")
                return child;
        }
        return null;
    }
}



















