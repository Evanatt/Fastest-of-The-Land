using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.HDROutputUtils;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadingScreen_Fill_UI : MonoBehaviour
{
    public Image loadingBar;  // El Image configurado como Filled en el inspector
    public float loadingTime = 3f;  // Tiempo total para completar la carga (en segundos)

    private void Start()
    {
        // Inicia la rutina que llena la barra y cambia de escena
        StartCoroutine(FillLoadingBar());
    }

    private IEnumerator FillLoadingBar()
    {
        float elapsedTime = 0f;

        // Incrementa el fillAmount progresivamente hasta que pase el tiempo definido
        while (elapsedTime < loadingTime)
        {
            elapsedTime += Time.deltaTime;
            loadingBar.fillAmount = Mathf.Clamp01(elapsedTime / loadingTime);
            yield return null;  // Espera al siguiente frame
        }

        // Cuando el tiempo se complete, cambia a la escena con índice 2
        SceneManager.LoadScene(1);
    }
}
