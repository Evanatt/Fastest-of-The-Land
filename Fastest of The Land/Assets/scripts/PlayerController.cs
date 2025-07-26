using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance; // Propiedad estática para el Singleton

    public int currentCheckpointIndex = 0; // Índice del próximo checkpoint
    public int lapCount = 0; // Contador de vueltas

    private void Awake()
    {
        // Configura el Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Más de una instancia de PlayerController encontrada. Destruyendo esta.");
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            // Verifica si es el checkpoint correcto
            if (int.Parse(other.name.Replace("Checkpoint", "")) == currentCheckpointIndex + 1)
            {
                currentCheckpointIndex++;
                Debug.Log($"Checkpoint {currentCheckpointIndex} alcanzado.");
            }
        }

        if (other.CompareTag("FinishLine") && currentCheckpointIndex == RaceManager.Instance.totalCheckpoints)
        {
            // Completa una vuelta solo si todos los checkpoints fueron alcanzados
            lapCount++;
            currentCheckpointIndex = 0; // Reinicia los checkpoints
            Debug.Log($"Vuelta {lapCount} completada.");

            PlayerVoiceHandler voice = GetComponent<PlayerVoiceHandler>();
            if (voice != null)
                voice.PlayLapLine();

            // Notifica al RaceManager
            RaceManager.Instance.UpdateLap(this);
        }
    }
}