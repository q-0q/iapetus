using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWeaponCollider : MonoBehaviour
{

    public static event Action OnPlayerWeaponCollision;
    private void OnTriggerStay(Collider collision)
    {
        if (collision.isTrigger) return;
        OnPlayerWeaponCollision?.Invoke();
    }
}
