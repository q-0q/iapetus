using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraBehaviorZone : MonoBehaviour
{

    public CameraBehavior cameraBehavior;
    public Vector3 InputVector3;
    
    public enum CameraBehavior
    {
        LookAtPoint,
        LookInDirection
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
        Gizmos.DrawLine(transform.position, transform.position + InputVector3);
        Gizmos.DrawSphere(transform.position + InputVector3, 1f);
    }
}
