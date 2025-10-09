using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceTrigger : MonoBehaviour
{
    public static event Action<RaceTrigger> OnTrigger;
    public static event Action<RaceTrigger> OnNotTrigger;
    private void OnTriggerEnter(Collider other)
    {
        OnTrigger?.Invoke(this);
    }
    
    private void OnTriggerExit(Collider other)
    {
        OnNotTrigger?.Invoke(this);
    }
    
    public void MarkNext()
    {
        TryGetComponent(out MeshRenderer meshRenderer);
        meshRenderer.material.SetColor("_Color", Color.white);
        meshRenderer.material.SetFloat("_Alpha", 0.5f);
    }

    public void Hide()
    {
        TryGetComponent(out MeshRenderer meshRenderer);
        meshRenderer.material.SetFloat("_Alpha", 0f);
    }
}
