using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradordeBellotas_script : MonoBehaviour
{
    public GameObject bellotaPrefab; // Prefab de la bellota
    public Transform[] spawnPoints;  // Array con los puntos de spawn
    private List<GameObject> bellotasActivas = new List<GameObject>(); // Lista de bellotas activas

    void GenerarTodasLasBellotas()
    {
        {
            foreach (var spawnPoint in spawnPoints)
            {
                // Instanciar la bellota
                GameObject bellota = Instantiate(bellotaPrefab, spawnPoint.position, Quaternion.identity);
                bellotasActivas.Add(bellota);

                Debug.Log($"Bellota generada en {spawnPoint.position}");

                // Desactivar la colisión por 0.1 segundos
                var collider = bellota.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.enabled = false;  // Desactiva colisión temporalmente
                    StartCoroutine(EnableColliderAfterDelay(collider, 0.1f));
                }

                // Asignar el spawner
                var bellotaManager = bellota.GetComponent<bellota_manager>();

            }
        }
    }
    private IEnumerator EnableColliderAfterDelay(Collider collider, float delay)
    {
        yield return new WaitForSeconds(delay);
        collider.enabled = true;
    }
}






