using System;
using JetBrains.Annotations;
using UnityEngine;

public class TriggerProxy : MonoBehaviour
{
    public event Action<Collider> OnTriggerProxyStay;
    public event Action<Collider> OnTriggerProxyExit;

    void OnTriggerStay(Collider other) 
    {
        OnTriggerProxyStay?.Invoke(other);
    }
    
    void OnTriggerExit(Collider other) 
    {
        OnTriggerProxyExit?.Invoke(other);
    }
}