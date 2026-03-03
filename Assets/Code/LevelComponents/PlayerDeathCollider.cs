using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathCollider : MonoBehaviour
{

    public bool requireGroundedState = true;
    // Start is called before the first frame update

    private void Awake()
    {
        TryGetComponent(out Renderer component);
        if (component == null) return;
        component.enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (requireGroundedState && (!PlayerFsm.Singleton.Machine.IsInState(GravityFsm.GravityFsmState.Grounded) || PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Jumpsquat))) return;
        PlayerFsm.Singleton.InvokePlayerDeath();
    }
}
