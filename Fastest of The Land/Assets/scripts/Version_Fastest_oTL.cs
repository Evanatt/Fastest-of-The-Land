using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Version_Fastest_oTL : MonoBehaviour
{
    public TMP_Text version;

    void Start()
    {
        version.text = Application.version;
    }
}
