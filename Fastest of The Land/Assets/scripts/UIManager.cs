using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI coinText; // Referencia al texto del contador

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

    public void UpdateCoinDisplay(int coins)
    {
        coinText.text = $"{coins}";
    }
}