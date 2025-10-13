using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionCollider : MonoBehaviour
{
    public event Action OnInteracted;
    private Collider _collider;

    private void Awake()
    {
        TryGetComponent(out _collider);
    }

    public void InvokeOnInteracted()
    {
        OnInteracted?.Invoke();
    }

    public void SetEnabled(bool val)
    {
        _collider.enabled = val;
    }
}
