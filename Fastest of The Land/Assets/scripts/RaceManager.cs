using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    public int totalLaps = 3; // Total de vueltas necesarias para ganar
    public int totalCheckpoints = 5; // Total de checkpoints en la pista
    public List<PlayerController> players = new List<PlayerController>(); // Lista de jugadores

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateLap(PlayerController player)
    {
        if (player.lapCount >= totalLaps)
        {
            EndRace();
        }
    }

    private void EndRace()
    {
        // Ordena a los jugadores por vueltas completadas
        players.Sort((p1, p2) => p2.lapCount.CompareTo(p1.lapCount));

        // Muestra los resultados finales
        foreach (var player in players)
        {
            Debug.Log($"Jugador {players.IndexOf(player) + 1}: {player.lapCount} vueltas");
        }

        // Aquí puedes cargar una pantalla de resultados o mostrar un mensaje
    }
}