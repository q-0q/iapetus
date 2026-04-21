using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GustMask : MonoBehaviour
{
    void OnEnable() => GustMaskRegistry.Colliders.Add(GetComponent<Collider>());
    void OnDisable() => GustMaskRegistry.Colliders.Remove(GetComponent<Collider>());
}

public static class GustMaskRegistry
{
    public static readonly List<Collider> Colliders = new();
}