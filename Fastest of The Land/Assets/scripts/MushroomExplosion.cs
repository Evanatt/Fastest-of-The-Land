using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomExplosion : MonoBehaviour
{
    public GameObject explosionEffectPrefab; // Prefab de las part�culas de explosi�n
    public AudioClip explosionSound; // Clip de sonido de explosi�n
    public float destroyDelay = 2f; // Tiempo para destruir el prefab despu�s de la explosi�n
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // M�todo para manejar colisiones
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Aseg�rate de que los jugadores tengan la etiqueta "Player"
        {
            TriggerExplosion();
        }
    }

    void TriggerExplosion()
    {
        // Reproducir efecto de part�culas
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Reproducir sonido de explosi�n
        if (explosionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        // Desactivar el hongo visualmente (opcional mientras espera para destruirse)
        GetComponent<MeshRenderer>().enabled = false; // Esconde el modelo del hongo
        GetComponent<Collider>().enabled = false; // Desactiva colisiones

        // Destruir el objeto despu�s del tiempo indicado
        Destroy(gameObject, destroyDelay);
    }
}

