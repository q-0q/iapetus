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
    
    public float waitTimeModifier = 1.0f;
    public float centeringTimeModifier = 1.0f;
    public bool invertDirection = false;
    public int priority = 0;
    
    public enum CameraBehavior
    {
        LookAtPoint,
        LookInDirection
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Gizmos.DrawWireCube(transform.position, transform.lossyScale);
        Gizmos.DrawLine(transform.position, transform.position + InputVector3);
        Gizmos.DrawSphere(transform.position + InputVector3, 1f);
    }

    public virtual Vector3 GetCameraForward(Vector3 position, out float y)
    {
        y = 0.7f;
        
        switch (cameraBehavior)
        {
            case CameraBehavior.LookAtPoint:
                return (InputVector3 + transform.position - PlayerFsm.Singleton.transform.position) * (invertDirection ? -1f : 1f);
            case CameraBehavior.LookInDirection:
                return (InputVector3) * (invertDirection ? -1f : 1f);
            default:
                return Vector3.zero;
        }
    }
}
