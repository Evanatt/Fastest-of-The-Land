using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trailfade : MonoBehaviour
{
    private TrailRenderer trailRenderer;
    public float fadeTime = 1f; // Tiempo para desvanecer
    public float currentTime = 0f;

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    void Update()
    {
        // Verificar si el rastro está activo
        if (trailRenderer.time > 0)
        {
            currentTime += Time.deltaTime;

            // Calcular el porcentaje de desvanecimiento
            float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeTime);

            // Aplicar la nueva transparencia
            Color color = trailRenderer.material.color;
            color.a = alpha; // Cambia la transparencia
            trailRenderer.material.color = color;

            // Si el rastro ha alcanzado la transparencia total, detener el rastro
            if (alpha <= 0)
            {
                trailRenderer.time = 0; // Detener el rastro
            }
        }
    }

    public void ResetFade()
    {
        // Resetear el tiempo y la transparencia
        currentTime = 0f;
        trailRenderer.time = 1f; // Reiniciar el tiempo del rastro
        Color color = trailRenderer.material.color;
        color.a = 1f; // Volver a opaco
        trailRenderer.material.color = color;
    }
}
