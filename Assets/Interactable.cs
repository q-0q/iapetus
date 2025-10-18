using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    
    public string text = "Chat with guard";
    public float triggerRange = 2;
    
    public event Action OnInteracted;
    private Collider _collider;

    public Type type;
    
    public enum Type
    {
        Switch,
        Dialogue
    }

    private void Awake()
    {
        TryGetComponent(out _collider);
    }

    public void TriggerInteraction()
    {
        OnInteracted?.Invoke();
    }

    public void SetEnabled(bool val)
    {
        _collider.enabled = val;
    }
}
