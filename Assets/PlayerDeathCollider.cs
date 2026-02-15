using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathCollider : MonoBehaviour
{
    // Start is called before the first frame update

    private void Awake()
    {
        var component = GetComponent<Renderer>();
        if (component is null) return;
        component.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerFsm.Singleton.InvokePlayerDeath();
    }
}
