using System;
using TMPro;
using UnityEngine;

public class FreezeCanvas : MonoBehaviour
{

    private TextMeshProUGUI tmp;

    private void Awake()
    {
        tmp = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tmp.text = PlayerFsm.Singleton.GetFreezeWeight().ToString();
    }
}
