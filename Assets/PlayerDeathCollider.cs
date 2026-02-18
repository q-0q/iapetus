using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathCollider : MonoBehaviour
{
    // Start is called before the first frame update

    private void Awake()
    {
        TryGetComponent(out Renderer component);
        if (component == null) return;
        component.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerFsm.Singleton.InvokePlayerDeath();
    }
}
