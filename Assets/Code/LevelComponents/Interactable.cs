using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    
    public string text = "Chat with guard";
    public float triggerRange = 10f;
    
    public event Action OnInteracted;
    public event Action OnHardInteracted;
    private Collider _collider;
    
    private void Awake()
    {
        TryGetComponent(out _collider);
    }

    public void TriggerInteraction()
    {
        OnInteracted?.Invoke();
    }
    
    public void TriggerHardInteraction()
    {
        OnHardInteracted?.Invoke();
    }

    public void SetEnabled(bool val)
    {
        _collider.enabled = val;
    }
}
