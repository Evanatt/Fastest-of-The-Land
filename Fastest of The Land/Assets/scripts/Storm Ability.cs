using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormAbility : MonoBehaviour
{
    public GameObject cloudPrefab;       // Prefab de la nube
    public GameObject lightningPrefab;  // Prefab del rayo
    public GameObject mushroomPrefab;   // Prefab del hongo

    public Transform carTransform;          // Transform del coche
    public Transform lightningSpawnPoint;   // Transform para la posici�n de los rayos

    public float lightningInterval = 0.5f;  // Intervalo entre rayos
    public int lightningCount = 4;          // N�mero de rayos generados

    private GameObject activeCloud;

    public void StartStorm()
    {
        // Crear la nube sobre el coche
        activeCloud = Instantiate(cloudPrefab, carTransform.position, Quaternion.identity);
        activeCloud.transform.SetParent(carTransform); // Nube sigue al coche

        // Iniciar la generaci�n de rayos
        StartCoroutine(GenerateLightning());
    }

    IEnumerator GenerateLightning()
    {
        for (int i = 0; i < lightningCount; i++)
        {
            // Posici�n base para los rayos (desde el punto especificado)
            Vector3 lightningPosition = lightningSpawnPoint.position;

            // Peque�o desplazamiento aleatorio para dispersi�n
            lightningPosition += new Vector3(Random.Range(-1f, 1f), 5f, Random.Range(-1f, 1f)); // Altura inicial ajustada a 10

            // Raycast hacia abajo para detectar el suelo
            if (Physics.Raycast(lightningPosition, Vector3.down, out RaycastHit hitInfo, 50f))
            {
                // Ajustar posici�n del rayo al punto de impacto con el suelo
                lightningPosition = hitInfo.point;

                // Instanciar el rayo
                GameObject lightning = Instantiate(lightningPrefab, lightningPosition, Quaternion.identity);
                Destroy(lightning, 1f); // Destruir rayo despu�s de 1 segundo

                // Generar el hongo en la misma posici�n
                GameObject mushroom = Instantiate(mushroomPrefab, lightningPosition, Quaternion.identity);

                // Ajustar la rotaci�n del hongo para que siempre est� "boca arriba"
                mushroom.transform.up = Vector3.up;

                // Configurar f�sicas del hongo
                Rigidbody rb = mushroom.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = true;
                    rb.isKinematic = false;
                }
            }

            // Esperar antes de generar el siguiente rayo
            yield return new WaitForSeconds(lightningInterval);
        }

        // Destruir la nube al finalizar los rayos
        Destroy(activeCloud);
    }
}