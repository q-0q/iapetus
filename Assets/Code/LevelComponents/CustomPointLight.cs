using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CustomPointLight : MonoBehaviour
{



    public Color Color;
    public float distanceLerpMin = 5;
    public float distanceLerpMax = 15f;
    public float distanceLerpPower = 0.5f;

    private void Start()
    {
    }

    private void OnEnable()
    {
        CustomPointLightManager.CustomPointLightRegistry.Add(this);
    }

    private void OnDisable()
    {
        CustomPointLightManager.CustomPointLightRegistry.Remove(this);
    }

    private void Update()
    {

    }
}
