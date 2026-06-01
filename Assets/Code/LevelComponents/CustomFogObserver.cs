using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomFogObserver : MonoBehaviour
{

    // public float Radius = 30f;
    // public float DesiredRadius = 30f;
    // public bool InvokeHalo = false;

    public bool isPlayer;

    private void Start()
    {
        if (!isPlayer) return;
        transform.SetParent(null);
    }

    private void OnEnable()
    {
        CustomFogManager.CustomFogObserverRegistry.Add(this);
    }

    private void OnDisable()
    {
        CustomFogManager.CustomFogObserverRegistry.Remove(this);
    }

    private void Update()
    {
        if (!isPlayer)return;
        
        // If you think this is hacky then you have forgotten that I am literally god
        transform.position = Shader.GetGlobalVector("_CameraFollowWorldPosition");
    }
}
