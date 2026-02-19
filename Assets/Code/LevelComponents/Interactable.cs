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
    public bool isEnabled = true;
    
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
        isEnabled = val;
    }
    
    void OnEnable() => InteractableRegistry.Interactables.Add(this);
    void OnDisable() => InteractableRegistry.Interactables.Remove(this);
}

public static class InteractableRegistry
{
    public static readonly List<Interactable> Interactables = new();
}
