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

    void LateUpdate()
    {
        // Calcular la posición objetivo de la cámara
        Vector3 targetPosition = player.position + player.transform.TransformVector(Offset);
        targetPosition -= player.transform.forward * Mathf.Abs(distance); // Mantener la distancia detrás del coche

        // Fijar la altura de la cámara en el eje Y
        targetPosition.y = player.position.y + height;

        // Suavizar la posición de la cámara
        transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);

        // Suavizar la rotación de la cámara para que siempre mire hacia el coche
        Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationDamping * Time.deltaTime);
    }
}




