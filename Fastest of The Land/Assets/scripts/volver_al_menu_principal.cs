using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class volver_al_menu_principal : MonoBehaviour
{
    public GameObject Canvas_creditos;
    public AudioSource botonossound;
    public AudioClip botonossound_Menu_Clip;
    public AudioClip botonossound_salir;

    public GameObject SeleccionLandersCanvas;

    public void volveralmenu()
    {
        botonossound.PlayOneShot(botonossound_salir);
        Canvas_creditos.SetActive(false);
        SeleccionLandersCanvas.SetActive(false);

    }

    public void AbrirCreditos()
    {
        botonossound.PlayOneShot(botonossound_Menu_Clip);
        Canvas_creditos.SetActive(true);
    }
}
