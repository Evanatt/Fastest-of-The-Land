using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager_main_menu : MonoBehaviour
{
    public TextMeshProUGUI lapText;
    public TextMeshProUGUI positionText;

    private void Update()
    {
        // Verifica que el jugador exista
        if (PlayerController.Instance != null && RaceManager.Instance != null)
        {
            lapText.text = $"{PlayerController.Instance.lapCount}/{RaceManager.Instance.totalLaps}";
            positionText.text = $"{GetPlayerPosition()}";
        }
    }

    private int GetPlayerPosition()
    {
        if (PlayerController.Instance != null && RaceManager.Instance != null)
        {
            return RaceManager.Instance.players.IndexOf(PlayerController.Instance) + 1;
        }
        return -1; // Retorna -1 si el jugador no está en la lista
    }
}
