using UnityEngine;
using UnityEngine.UI;

public class FadeWithAnimator : MonoBehaviour
{
    public Image fadeImage; // La imagen de fade en el Canvas

    // Este método debe llamarse desde un evento de animación
    public void OnFadeStart()
    {
        fadeImage.raycastTarget = true; // Desactivar los botones mientras la imagen se desvanece
    }

    // Este método debe llamarse cuando la animación haya terminado (evento de animación)
    public void OnFadeEnd()
    {
        fadeImage.raycastTarget = false; // Reactivar los botones cuando la imagen desaparezca
    }

}

