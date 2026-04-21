using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gust : MonoBehaviour
{
    void Start()
    {
        var trigger = GetComponentInChildren<ParticleSystem>().trigger;
        foreach (var collider in GustMaskRegistry.Colliders)
        {
            trigger.AddCollider(collider);
        }
    }
}
