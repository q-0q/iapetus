using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceStartTrigger : MonoBehaviour
{
    public event Action OnTrigger;
    public event Action OnNotTrigger;
    private void OnTriggerStay(Collider other)
    {
        OnTrigger?.Invoke();
    }
    
    private void OnTriggerExit(Collider other)
    {
        OnNotTrigger?.Invoke();
    }
}
