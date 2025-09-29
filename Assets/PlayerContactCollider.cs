using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerContactCollider : MonoBehaviour
{

    public static event Action OnPlayerContactHitboxCollision;
    private void OnTriggerStay(Collider collision)
    {
        if (collision.isTrigger)
        {
            if (collision.transform.gameObject.layer != LayerMask.NameToLayer("EnemyHurtbox")) return;
            OnPlayerContactHitboxCollision?.Invoke();
        }
    }
}
