using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour
{
    public bool jump = false;
    
    private void OnTriggerStay(Collider other)
    {
        other.transform.parent.TryGetComponent(out PlayerFsm playerFsm);
        if (playerFsm == null) return;
        playerFsm.InvokeBoost(jump);
    }
}
