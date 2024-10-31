using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotarSkybox : MonoBehaviour
{
    public float SkyRot;
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * SkyRot);
    }
}
