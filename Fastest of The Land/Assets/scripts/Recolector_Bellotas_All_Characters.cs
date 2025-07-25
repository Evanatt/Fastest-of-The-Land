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
            isCollectingBellota = true; // Marcar que se est� recogiendo una bellota
            other.GetComponent<bellota_manager>().CollectBellota(transform); // Llama al m�todo de recolecci�n en el manager
        }
    }
    public void ResetCollectingState() // M�todo para resetear el estado despu�s de un tiempo
    {
        StartCoroutine(ResetCollectingCoroutine());
    }
    private IEnumerator ResetCollectingCoroutine()
    {
        yield return new WaitForSeconds(5f); // Esperar 3 segundos
        isCollectingBellota = false; // Reiniciar el estado de recolecci�n
    }
}
