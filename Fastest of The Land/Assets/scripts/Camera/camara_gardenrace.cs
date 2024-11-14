using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camara_gardenrace : MonoBehaviour
{
    public Transform player;
    public Rigidbody playerRB;
    public Vector3 Offset; // Offset para ajustar la posición de la cámara
    public float speed = 5f; // Suavidad del movimiento de la cámara
    public float height = 5f; // Altura fija de la cámara
    public float distance = -5f; // Distancia fija detrás del jugador
    public float rotationDamping = 2f; // Suavidad al rotar la cámara
    public LayerMask collisionLayers; // Capas para detectar colisiones

    private Vector3 currentCameraPosition;

    void LateUpdate()
    {
        // Calcular la posición objetivo de la cámara
        Vector3 targetPosition = player.position + player.transform.TransformVector(Offset);
        targetPosition -= player.transform.forward * Mathf.Abs(distance);

        // Fijar la altura de la cámara en el eje Y
        targetPosition.y = player.position.y + height;

        // Realizar un raycast desde el jugador hacia la posición objetivo de la cámara
        Vector3 desiredCameraPosition = targetPosition;
        Vector3 direction = desiredCameraPosition - player.position;
        float maxDistance = direction.magnitude;
        RaycastHit hit;

        if (Physics.Raycast(player.position, direction.normalized, out hit, maxDistance, collisionLayers))
        {
            // Si el raycast detecta una colisión, ajusta la posición de la cámara
            desiredCameraPosition = hit.point - direction.normalized * 0.5f; // Ajuste para evitar que la cámara toque el objeto
        }

        // Suavizar la posición de la cámara
        transform.position = Vector3.Lerp(transform.position, desiredCameraPosition, speed * Time.deltaTime);

        // Suavizar la rotación de la cámara para que siempre mire hacia el coche
        Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationDamping * Time.deltaTime);
    }
}





