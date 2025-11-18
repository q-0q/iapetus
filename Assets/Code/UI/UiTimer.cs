using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class UiTimer : MonoBehaviour
{
    public float _timer;
    public bool _display;
    public bool _active;
    public static UiTimer Singleton;
    private TextMeshProUGUI _tmp;

    private void Awake()
    {
        Singleton = this;
    }


    
    
    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out _tmp);
    }

    // Update is called once per frame
    void Update()
    {
        if (_active) _timer += Time.deltaTime;
        _tmp.text = _timer.ToString("F2");
        _tmp.enabled = _display;

    }
}
