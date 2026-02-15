using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPathKeyframe : MonoBehaviour
{
    [Range(0, 1f)] public float y = 0.7f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position, 1.5f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5f);
    }
}
