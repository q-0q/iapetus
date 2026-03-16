using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuVersionNumber : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = Application.version;
    }
}
