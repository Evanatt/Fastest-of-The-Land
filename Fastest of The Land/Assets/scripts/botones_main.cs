using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class botones_main : MonoBehaviour
{
    public float loadingTime = 1f;
    private Animator animcanvas;

    public AudioSource botonossound;
    public AudioClip botonossound_Play_Clip;

    public GameObject SeleccionLandersCanvas;
    public GameObject comingsoon_aviso;

    private void Start()
    {
        animcanvas = GetComponent<Animator>();
    }
    public void jugar()
    {
        StartCoroutine(PlaySoundAndLoadScene());
    }
    public void selecciondepersonajes()
    {
        SeleccionLandersCanvas.SetActive(true);
        GetComponent<AudioSource>().Play();
    }
    public void OnMouseDown()
    {
        comingsoon_aviso.SetActive(true);
        GetComponent<AudioSource>().Play();

    }
    public void VolveralInicio()
    {
        SceneManager.LoadScene("menu_principal");
    }

    public void Exit()
    {
        Application.Quit();
    }
    private IEnumerator PlaySoundAndLoadScene()
    {
        animcanvas.SetTrigger("Start");
        // Reproducir el sonido del botón
        botonossound.PlayOneShot(botonossound_Play_Clip);

        // Esperar hasta que el sonido se termine de reproducir
        yield return new WaitForSeconds(botonossound_Play_Clip.length);

        // Cargar la escena después de reproducir el sonido
        SceneManager.LoadScene("nivel_garden_race");
    }
}
