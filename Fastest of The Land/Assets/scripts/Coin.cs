using FIMSpace.Basics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1; // Valor de la moneda
    private AudioSource audioSource;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); // Componente de sonido
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Aseg�rate de que los jugadores tengan el tag "Player"
        {
            // Buscar CoinTarget dentro del jugador que toc� la moneda
            Transform coinTarget = other.transform.Find("CoinTarget");
            if (coinTarget == null)
            {
                Debug.LogWarning($"No se encontr� 'CoinTarget' en {other.name}. Aseg�rate de que cada jugador tenga un hijo llamado 'CoinTarget'.");
                return;
            }

            // Sumar monedas
            CoinManager.Instance.AddCoins(value);

            // Reubicar la moneda hacia el CoinTarget del jugador
            transform.parent.SetParent(coinTarget);
            transform.parent.localPosition = Vector3.zero;
            transform.parent.localRotation = Quaternion.identity;
            transform.parent.localScale = Vector3.one;

            // Activar animaci�n y sonido
            if (animator != null) animator.SetTrigger("CollectTrigger");
            if (audioSource != null) audioSource.Play();
        }
}
    public void DestroySelf()
    {
        Destroy(gameObject); // Destruye la moneda
    }
}