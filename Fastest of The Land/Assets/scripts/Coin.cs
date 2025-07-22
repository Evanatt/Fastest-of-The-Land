using FIMSpace.Basics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1; // Valor de la moneda
    private AudioSource audioSource;
    private Transform coinTarget;
    private Animator animator;

    private void Start()
    {
        coinTarget = GameObject.Find("CoinTarget")?.transform;
        if (coinTarget == null)
        {
            Debug.LogError("No se encontró el objeto 'CoinTarget' en la escena. Asegúrate de que exista un objeto con ese nombre.");
        }

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); // Obtiene el componente AudioSource
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Asegúrate de que el jugador tenga el tag "Player"
        {
            CoinManager.Instance.AddCoins(value); // Llama al gestor de monedas

            transform.parent.SetParent(coinTarget);
            transform.parent.localPosition = Vector3.zero;
            transform.parent.localRotation = Quaternion.identity;

            // Forzar la escala del padre
            transform.parent.localScale = Vector3.one;


            animator.SetTrigger("CollectTrigger");


            if (audioSource != null)
            {
                audioSource.Play(); // Reproduce el sonido de recolección
            }

            

        }

    }
    public void DestroySelf()
    {
        Destroy(gameObject); // Destruye la moneda
    }
}