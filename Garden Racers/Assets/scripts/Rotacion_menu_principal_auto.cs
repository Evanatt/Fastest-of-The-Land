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

    // Update is called once per frame
    void Update()
    {
        // Detectar si el botón izquierdo está presionado
        if (Input.GetMouseButtonDown(0) && IsMouseOverTarget())
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // Si se está arrastrando, rota la plataforma
        if (isDragging)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            float rotationAmount = -mouseDelta.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotationAmount);
            currentSpeed = rotationAmount; // Actualiza la velocidad actual de rotación
            lastMousePosition = Input.mousePosition;
        }
        else
        {
            // Aplicar inercia cuando se suelta el botón
            transform.Rotate(Vector3.up, currentSpeed);
            currentSpeed *= deceleration; // Desaceleración gradual de la rotación
        }
    }

    // Verifica si el mouse está sobre el área del auto y el personaje
    bool IsMouseOverTarget()
    {
        // Raycast desde la posición del mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // Verifica si el objeto que toca el raycast está en el layer específico (ej. "auto")
            return hit.collider.gameObject.layer == LayerMask.NameToLayer("auto");
        }
        return false;
    }
    /*private LayerMask targetLayer;
    public float Speed = 11f;
    private bool IsRotating = false;
    private float starmousePosition;
    private bool touchAnywhere;
    private Camera m_camera;
    void Start()
    {
        IsRotating = false;
        m_camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!touchAnywhere)
        {
            if (!IsRotating)
            {
                RaycastHit hit;
                Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out hit, 500, targetLayer))
                {
                    return;
                }
            }
        }
            if (Input.GetMouseButtonDown(0))
            {
                IsRotating = true;
                starmousePosition = Input.mousePosition.x;


            }
            else if (Input.GetMouseButtonDown(0))
            {
                IsRotating = false;
            }
            if (IsRotating)
            {
                float currentmouseposition = Input.mousePosition.x;
                float mousemovement = currentmouseposition - starmousePosition;
                transform.Rotate(Vector3.up, -mousemovement * Speed * Time.deltaTime);
                starmousePosition = currentmouseposition;
            }
        if (Input.GetMouseButtonUp(0))
            IsRotating = false;
    }
    */
}

