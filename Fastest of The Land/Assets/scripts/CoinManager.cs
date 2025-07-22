using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    public int currentCoins = 0; // Monedas recolectadas en la partida actual
    public int totalCoins = 0;   // Monedas totales del jugador

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateUI(); // Actualiza la interfaz de usuario
    }

    public void SaveCoins()
    {
        totalCoins += currentCoins; // Suma las monedas recolectadas al total
        currentCoins = 0;           // Reinicia las monedas de la partida actual
    }

    private void UpdateUI()
    {
        // Llama a tu sistema de UI para actualizar el contador de monedas
        UIManager.Instance.UpdateCoinDisplay(currentCoins);
    }
}
