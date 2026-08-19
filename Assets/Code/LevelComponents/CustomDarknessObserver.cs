using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CustomDarknessObserver : MonoBehaviour
{
    
    public float radiusMultiplier = 1.0f;

    private void OnEnable()
    {
        CustomDarknessManager.CustomDarknessObserverRegistry.Add(this);
    }

    private void OnDisable()
    {
        CustomDarknessManager.CustomDarknessObserverRegistry.Remove(this);
    }
    
}
