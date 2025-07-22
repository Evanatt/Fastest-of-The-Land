using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomExplosion : MonoBehaviour
{
    public GameObject explosionEffectPrefab; // Prefab de las partículas de explosión
    public AudioClip explosionSound; // Clip de sonido de explosión
    public float destroyDelay = 2f; // Tiempo para destruir el prefab después de la explosión
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Método para manejar colisiones
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Asegúrate de que los jugadores tengan la etiqueta "Player"
        {
            TriggerExplosion();
        }
    }

    void TriggerExplosion()
    {
        // Reproducir efecto de partículas
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Reproducir sonido de explosión
        if (explosionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        // Desactivar el hongo visualmente (opcional mientras espera para destruirse)
        GetComponent<MeshRenderer>().enabled = false; // Esconde el modelo del hongo
        GetComponent<Collider>().enabled = false; // Desactiva colisiones

        // Destruir el objeto después del tiempo indicado
        Destroy(gameObject, destroyDelay);
    }
}

