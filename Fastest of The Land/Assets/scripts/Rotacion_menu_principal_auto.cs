using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotacion_menu_principal_auto : MonoBehaviour
{
    public float rotationSpeed = 5f;  // Velocidad de rotaci�n del mouse
    public float deceleration = 0.95f; // Factor de desaceleraci�n de la inercia
    private bool isDragging = false;   // Verifica si se est� haciendo clic en la zona
    private float currentSpeed = 0f;   // Velocidad actual de rotaci�n
    private Vector3 lastMousePosition; // �ltima posici�n del mouse

    public AudioClip startRotationSound; // Sonido de arrastre
    public AudioClip endRotationSound;   // Sonido al terminar de desacelerar
    private AudioSource audioSource;
    private bool isPlayingDragSound = false; // Controla si el sonido de arrastre se est� reproduciendo
    private bool hasPlayedEndSound = false;  // Controla si el sonido de fin ya se reprodujo

    public GameObject creditsCanvas; // Referencia al canvas de los cr�ditos

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Si el canvas de cr�ditos est� activo, no permitir interacci�n
        if (creditsCanvas.activeSelf)
        {
            isDragging = false;
            currentSpeed = 0f; // Detener cualquier movimiento residual
            audioSource.Stop(); // Detener sonidos si est�n activos
            return;
        }

        // Detectar si el bot�n izquierdo est� presionado y est� sobre el collider
        if (Input.GetMouseButtonDown(0) && IsMouseOverTarget())
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
            hasPlayedEndSound = false; // Reiniciar el estado del sonido de fin
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            isPlayingDragSound = false; // Detener el sonido de arrastre
            audioSource.Stop();
        }

        // Si se est� arrastrando y hay movimiento, reproducir sonido de arrastre
        if (isDragging)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            float rotationAmount = -mouseDelta.x * rotationSpeed * Time.deltaTime;

            // Solo reproducir sonido si hay movimiento
            if (Mathf.Abs(rotationAmount) > 0.01f && !isPlayingDragSound)
            {
                audioSource.PlayOneShot(startRotationSound);
                isPlayingDragSound = true;
            }

            transform.Rotate(Vector3.up, rotationAmount);
            currentSpeed = rotationAmount; // Actualiza la velocidad actual de rotaci�n
            lastMousePosition = Input.mousePosition;
        }
        else
        {
            // Aplicar inercia cuando se suelta el bot�n
            transform.Rotate(Vector3.up, currentSpeed);
            currentSpeed *= deceleration; // Desaceleraci�n gradual de la rotaci�n

            // Reproducir sonido de fin una sola vez cuando la velocidad es casi cero
            if (Mathf.Abs(currentSpeed) < 0.01f && !hasPlayedEndSound)
            {
                audioSource.PlayOneShot(endRotationSound);
                hasPlayedEndSound = true;
            }
        }
    }

    // Verifica si el mouse est� sobre el �rea del auto y el personaje
    bool IsMouseOverTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.gameObject.layer == LayerMask.NameToLayer("auto");
        }
        return false;
    }
}
