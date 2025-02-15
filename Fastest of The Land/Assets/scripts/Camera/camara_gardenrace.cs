using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camara_gardenrace : MonoBehaviour
{
    public Transform player;
    public Rigidbody playerRB;
    public Vector3 Offset; // Offset para ajustar la posici�n de la c�mara
    public float speed = 5f; // Suavidad del movimiento de la c�mara
    public float height = 5f; // Altura fija de la c�mara
    public float distance = -5f; // Distancia fija detr�s del jugador
    public float rotationDamping = 2f; // Suavidad al rotar la c�mara
    public LayerMask collisionLayers; // Capas para detectar colisiones

    private Vector3 currentCameraPosition;

    void LateUpdate()
    {
        // Calcular la posici�n objetivo de la c�mara
        Vector3 targetPosition = player.position + player.transform.TransformVector(Offset);
        targetPosition -= player.transform.forward * Mathf.Abs(distance);

        // Fijar la altura de la c�mara en el eje Y
        targetPosition.y = player.position.y + height;

        // Realizar un raycast desde el jugador hacia la posici�n objetivo de la c�mara
        Vector3 desiredCameraPosition = targetPosition;
        Vector3 direction = desiredCameraPosition - player.position;
        float maxDistance = direction.magnitude;
        RaycastHit hit;

        if (Physics.Raycast(player.position, direction.normalized, out hit, maxDistance, collisionLayers))
        {
            // Si el raycast detecta una colisi�n, ajusta la posici�n de la c�mara
            desiredCameraPosition = hit.point - direction.normalized * 0.5f; // Ajuste para evitar que la c�mara toque el objeto
        }

        // Suavizar la posici�n de la c�mara
        transform.position = Vector3.Lerp(transform.position, desiredCameraPosition, speed * Time.deltaTime);

        // Suavizar la rotaci�n de la c�mara para que siempre mire hacia el coche
        Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationDamping * Time.deltaTime);
    }
}





