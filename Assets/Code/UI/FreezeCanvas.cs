using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FreezeCanvas : MonoBehaviour
{

    private Material _material;

    private void Awake()
    {
        _material = GetComponentInChildren<Image>().material;
        _material.SetFloat("_Weight_1", 0);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var w = PlayerFsm.Singleton.GetFreezeWeight();
        if (w > 0.01f) w = Mathf.Lerp(0.175f, 1f, w);
        _material.SetFloat("_Weight_1", Mathf.Lerp(_material.GetFloat("_Weight_1"), w, Time.deltaTime * 10f));
    }
}
