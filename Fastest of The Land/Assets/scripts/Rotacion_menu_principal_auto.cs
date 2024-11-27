using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotacion_menu_principal_auto : MonoBehaviour
{
    public float rotationSpeed = 5f;  // Velocidad de rotación del mouse
    public float deceleration = 0.95f; // Factor de desaceleración de la inercia
    private bool isDragging = false;   // Verifica si se está haciendo clic en la zona
    private float currentSpeed = 0f;   // Velocidad actual de rotación
    private Vector3 lastMousePosition; // Última posición del mouse

    public AudioClip startRotationSound; // Sonido de arrastre
    public AudioClip endRotationSound;   // Sonido al terminar de desacelerar
    private AudioSource audioSource;
    private bool isPlayingDragSound = false; // Controla si el sonido de arrastre se está reproduciendo
    private bool hasPlayedEndSound = false;  // Controla si el sonido de fin ya se reprodujo

    public GameObject creditsCanvas; // Referencia al canvas de los créditos

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Si el canvas de créditos está activo, no permitir interacción
        if (creditsCanvas.activeSelf)
        {
            isDragging = false;
            currentSpeed = 0f; // Detener cualquier movimiento residual
            audioSource.Stop(); // Detener sonidos si están activos
            return;
        }

        // Detectar si el botón izquierdo está presionado y está sobre el collider
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

        // Si se está arrastrando y hay movimiento, reproducir sonido de arrastre
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
            currentSpeed = rotationAmount; // Actualiza la velocidad actual de rotación
            lastMousePosition = Input.mousePosition;
        }
        else
        {
            // Aplicar inercia cuando se suelta el botón
            transform.Rotate(Vector3.up, currentSpeed);
            currentSpeed *= deceleration; // Desaceleración gradual de la rotación

            // Reproducir sonido de fin una sola vez cuando la velocidad es casi cero
            if (Mathf.Abs(currentSpeed) < 0.01f && !hasPlayedEndSound)
            {
                audioSource.PlayOneShot(endRotationSound);
                hasPlayedEndSound = true;
            }
        }
    }

    // Verifica si el mouse está sobre el área del auto y el personaje
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
