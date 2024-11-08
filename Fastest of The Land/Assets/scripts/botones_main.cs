using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class botones_main : MonoBehaviour
{
    public TMP_Text version;

    void Start()
    {
        version.text = Application.version;
    }
    public void jugar()
    {
        SceneManager.LoadScene("nivel_garden_race");
    }
    public void VolveralInicio()
    {
        SceneManager.LoadScene("menu_principal");
    }
    public void Exit()
    {
        Application.Quit();
    }
}
