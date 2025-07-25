using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorPersonaje : MonoBehaviour
{
    [Header("Landers en el menú")]
    public GameObject[] landersMenu; // Los modelos visibles en la selección
    private int personajeSeleccionado = 0;
    public GameObject SeleccionLandersCanvas;
    public AudioSource Landers_button_sound;
    public AudioClip Vorrix_Sound;
    public AudioClip Rubemori_Sound;
    public AudioClip Luz_Sound;
    void Start()
    {
        ActivarSoloSeleccionado();
    }

    public void SeleccionarPersonaje(int index)
    {
        personajeSeleccionado = index;
        PlayerPrefs.SetInt("PersonajeSeleccionado", personajeSeleccionado);
        ActivarSoloSeleccionado();
        switch (index)
        {
            case 0: // Vorrix
                Landers_button_sound.PlayOneShot(Vorrix_Sound);
                break;
            case 1: // Rubemori
                Landers_button_sound.PlayOneShot(Rubemori_Sound);
                break;
            case 2: // Luz
                Landers_button_sound.PlayOneShot(Luz_Sound);
                break;
        }
    }

    void ActivarSoloSeleccionado()
    {
        for (int i = 0; i < landersMenu.Length; i++)
        {
            landersMenu[i].SetActive(i == personajeSeleccionado);
            SeleccionLandersCanvas.SetActive(false);

        }
    }
}
