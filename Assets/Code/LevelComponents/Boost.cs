using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour
{
    public bool jump = false;
    public float momentumWeight = 1.0f;
    
    private void OnTriggerStay(Collider other)
    {
        other.transform.parent.TryGetComponent(out PlayerFsm playerFsm);
        if (playerFsm == null) return;
        playerFsm.InvokeBoost(jump, momentumWeight);
    }
}
