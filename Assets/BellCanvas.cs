using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BellCanvas : MonoBehaviour
{
    private TextMeshProUGUI _tmp;
    
    // Start is called before the first frame update
    void Start()
    {
        _tmp = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnBellRung()
    {
        
    }

    private void OnEnable()
    {
        BellController.OnBellRing += OnBellRung;
    }

    private void OnDisable()
    {
        BellController.OnBellRing -= OnBellRung;
    }
}
