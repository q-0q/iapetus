using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        other.transform.parent.TryGetComponent(out PlayerFsm playerFsm);
        if (playerFsm == null) return;
        playerFsm.InvokeBoost();
    }
}
