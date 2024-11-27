using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;  // Necesario para la entrada del rat�n
using System.Collections;

public class PowerUpUIManager : MonoBehaviour
{
    public Image powerUpFillImage;  // La barra de progreso
    public Button powerUpButton;    // El bot�n de la habilidad
    public AudioClip readySound;    // Sonido cuando la barra est� llena
    public AudioClip abilitySound;  // Sonido cuando se activa la habilidad
    public AudioClip disabledSound; // Sonido cuando el bot�n est� deshabilitado

    private AudioSource audioSource;  // El audio source para reproducir sonidos

    public float FILL_INCREMENT = 0.25f; // Incremento por bellota (0.25 para 4 bellotas)
    public float MAX_FILL_AMOUNT = 1f;   // M�ximo de llenado (100%)
    [Tooltip("Tiempo en segundos para vaciar la barra completamente.")]
    public float emptyTime = 3.0f; // Ajustable en Unity

    public float FILL_SPEED = 2f;   // Velocidad de suavizado para el llenado

    private float currentFillAmount = 0f; // Valor actual de la barra de progreso
    private float targetFillAmount = 0f;  // Valor objetivo para la interpolaci�n
    private bool isEmptying = false;      // Indica si la barra se est� vaciando
    private bool isReadySoundPlayed = false; // Indica si ya se reprodujo el sonido de "ready"
    private float emptyLerpTime = 0f;     // Controlador interno para el tiempo de vaciado

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        powerUpButton.interactable = false;
        powerUpFillImage.fillAmount = 0f;

        // Suscribirse al evento OnBellotaCollected para aumentar la barra
        bellota_manager.OnBellotaCollected += IncreaseFillAmount;
    }

    private void OnDestroy()
    {
        bellota_manager.OnBellotaCollected -= IncreaseFillAmount;
    }

    void Update()
    {
        // Suavizado del llenado
        currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, Time.deltaTime * FILL_SPEED);
        powerUpFillImage.fillAmount = currentFillAmount;

        // Reproducir el sonido cuando la barra est� visualmente llena
        if (!isReadySoundPlayed && currentFillAmount >= 0.99f)
        {
            powerUpButton.interactable = true;
            if (audioSource && readySound)
            {
                audioSource.PlayOneShot(readySound);
            }
            isReadySoundPlayed = true;
        }

        // Suavizado del vaciado
        if (isEmptying)
        {
            emptyLerpTime += Time.deltaTime / emptyTime;
            currentFillAmount = Mathf.Lerp(1f, 0f, emptyLerpTime);
            powerUpFillImage.fillAmount = currentFillAmount;

            // Si la barra est� vac�a, deshabilitar el bot�n
            if (currentFillAmount <= 0f)
            {
                powerUpButton.interactable = false;
                isEmptying = false;
                isReadySoundPlayed = false; // Reiniciar el estado del sonido
                emptyLerpTime = 0f; // Reiniciar el tiempo de Lerp
            }
        }

        // Detectar clic incluso si el bot�n est� deshabilitado
        if (!powerUpButton.interactable && Input.GetMouseButtonDown(0)) // Bot�n izquierdo del mouse
        {
            // Verificar si el clic fue sobre el bot�n
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Reproducir sonido de "deshabilitado"
                if (audioSource && disabledSound)
                {
                    audioSource.PlayOneShot(disabledSound);
                }
            }
        }
    }

    // M�todo que se invoca cuando se recolecta una bellota
    public void IncreaseFillAmount()
    {
        if (targetFillAmount < MAX_FILL_AMOUNT)
        {
            targetFillAmount += FILL_INCREMENT;
            targetFillAmount = Mathf.Clamp(targetFillAmount, 0f, MAX_FILL_AMOUNT);
        }
    }

    // M�todo para activar la habilidad cuando la barra se llena
    public void ActivatePowerUp()
    {
        if (powerUpButton.interactable) // Si el bot�n est� habilitado
        {
            if (audioSource && abilitySound)
            {
                audioSource.PlayOneShot(abilitySound);
            }

            isEmptying = true;
            emptyLerpTime = 0f; // Reiniciar el tiempo de Lerp
            targetFillAmount = 0f; // Reiniciar el valor objetivo para el vaciado
        }
        else // Si el bot�n est� deshabilitado
        {
            if (audioSource && disabledSound)
            {
                audioSource.PlayOneShot(disabledSound); // Reproducir sonido de deshabilitado
            }
        }
    }
}
