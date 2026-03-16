using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CompletionCanvasTrigger : MonoBehaviour
{
    public static event Action OnCompletionCanvasTriggerEnter;
    public static event Action OnCompletionCanvasTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        OnCompletionCanvasTriggerEnter?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        OnCompletionCanvasTriggerExit?.Invoke();
    }
}
