using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceStartTrigger : MonoBehaviour
{
    public event Action OnTrigger;
    private void OnTriggerStay(Collider other)
    {
        OnTrigger?.Invoke();
    }
}
