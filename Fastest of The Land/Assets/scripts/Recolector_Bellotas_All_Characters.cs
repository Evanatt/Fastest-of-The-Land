using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recolector_Bellotas_All_Characters : MonoBehaviour
{
    public bool isCollectingBellota;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bellota") && !isCollectingBellota)
        {
            isCollectingBellota = true; // Marcar que se está recogiendo una bellota
            other.GetComponent<bellota_manager>().CollectBellota(transform); // Llama al método de recolección en el manager
        }
    }
    public void ResetCollectingState() // Método para resetear el estado después de un tiempo
    {
        StartCoroutine(ResetCollectingCoroutine());
    }
    private IEnumerator ResetCollectingCoroutine()
    {
        yield return new WaitForSeconds(5f); // Esperar 3 segundos
        isCollectingBellota = false; // Reiniciar el estado de recolección
    }
}
