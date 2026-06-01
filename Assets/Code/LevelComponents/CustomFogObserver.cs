using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomFogObserver : MonoBehaviour
{

    // public float Radius = 30f;
    // public float DesiredRadius = 30f;
    // public bool InvokeHalo = false;

    private void OnEnable()
    {
        CustomFogManager.CustomFogObserverRegistry.Add(this);
    }

    private void OnDisable()
    {
        CustomFogManager.CustomFogObserverRegistry.Remove(this);
    }
}
