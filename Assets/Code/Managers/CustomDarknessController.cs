using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CustomDarknessController : MonoBehaviour
{
    public Color Color = Color.black;
    
    void OnEnable() => CustomDarknessManager.CustomDarknessControllerRegistry.Add(this);
    void OnDisable() => CustomDarknessManager.CustomDarknessControllerRegistry.Remove(this);



}
