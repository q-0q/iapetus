using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DevelopmentBuildCanvas : MonoBehaviour
{
    private void Start()
    {
        GetComponent<TextMeshProUGUI>().text = "dev build " + Application.version;
    }
}
