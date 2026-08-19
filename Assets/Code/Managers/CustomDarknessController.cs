using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CustomDarknessController : MonoBehaviour
{
    
    void OnEnable() => CustomDarknessManager.CustomDarknessControllerRegistry.Add(this);
    void OnDisable() => CustomDarknessManager.CustomDarknessControllerRegistry.Remove(this);



}
