using System;
using TMPro;
using UnityEngine;

public class AppPathDebug : MonoBehaviour
{
    private void Start()
    {
        GetComponent<TextMeshProUGUI>().text = MetaSaveSystem.GetPath();
    }
}
