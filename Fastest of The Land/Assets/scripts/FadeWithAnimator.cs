using UnityEngine;
using UnityEngine.UI;

public class FadeWithAnimator : MonoBehaviour
{
    public Image fadeImage; // La imagen de fade en el Canvas

    // Este m�todo debe llamarse desde un evento de animaci�n
    public void OnFadeStart()
    {
        fadeImage.raycastTarget = true; // Desactivar los botones mientras la imagen se desvanece
    }

    // Este m�todo debe llamarse cuando la animaci�n haya terminado (evento de animaci�n)
    public void OnFadeEnd()
    {
        fadeImage.raycastTarget = false; // Reactivar los botones cuando la imagen desaparezca
    }

}

