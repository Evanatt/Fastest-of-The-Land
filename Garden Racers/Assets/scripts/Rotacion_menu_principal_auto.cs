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

    // Update is called once per frame
    void Update()
    {
        // Detectar si el bot�n izquierdo est� presionado
        if (Input.GetMouseButtonDown(0) && IsMouseOverTarget())
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // Si se est� arrastrando, rota la plataforma
        if (isDragging)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            float rotationAmount = -mouseDelta.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotationAmount);
            currentSpeed = rotationAmount; // Actualiza la velocidad actual de rotaci�n
            lastMousePosition = Input.mousePosition;
        }
        else
        {
            // Aplicar inercia cuando se suelta el bot�n
            transform.Rotate(Vector3.up, currentSpeed);
            currentSpeed *= deceleration; // Desaceleraci�n gradual de la rotaci�n
        }
    }

    // Verifica si el mouse est� sobre el �rea del auto y el personaje
    bool IsMouseOverTarget()
    {
        // Raycast desde la posici�n del mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // Verifica si el objeto que toca el raycast est� en el layer espec�fico (ej. "auto")
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

