using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CustomDarknessController : MonoBehaviour
{
    public Color Color = Color.black;
    public float Priority = 1;
    
    void OnEnable() => CustomDarknessManager.CustomDarknessControllerRegistry.Add(this);
    void OnDisable() => CustomDarknessManager.CustomDarknessControllerRegistry.Remove(this);



}
