using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Landers en la carrera")]
    public GameObject[] landersCarrera; // Prefabs de los personajes con sus cámaras

    void Start()
    {
        int index = PlayerPrefs.GetInt("PersonajeSeleccionado", 0); // 0 como default
        ActivarSoloSeleccionado(index);
    }

    void ActivarSoloSeleccionado(int index)
    {
        for (int i = 0; i < landersCarrera.Length; i++)
        {
            landersCarrera[i].SetActive(i == index);
        }
    }
}
