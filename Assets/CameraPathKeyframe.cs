using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPathKeyframe : MonoBehaviour
{
    [Range(0, 1f)] public float y = 0.7f;

    public bool reflectionEnabled;
    public float reflectionAngle;

    public float reflectionDeadzoneSize = 10f;
    private Quaternion reflectionAxisStore;
    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position, 1.5f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5f);
    }

    private void Start()
    {
        reflectionAxisStore = transform.rotation;
    }

    private void Update()
    {
        if (!reflectionEnabled) return;

        var signedAngle = Vector3.SignedAngle(transform.forward, PlayerFsm.Singleton.transform.forward, Vector3.up);

        var newForwardMinimum = Quaternion.Euler(0, -reflectionAngle + reflectionDeadzoneSize, 0) * transform.rotation;
        var newForwardMAximum = Quaternion.Euler(0, reflectionAngle - reflectionDeadzoneSize, 0) * transform.rotation;

        var newForward = Quaternion.Lerp(newForwardMinimum, newForwardMAximum,
            Mathf.InverseLerp(-reflectionAngle, reflectionAngle, signedAngle));
        
        reflectionAxisStore = Quaternion.Lerp(reflectionAxisStore, newForward, Time.deltaTime * 6f);
    }

    public Quaternion GetKeyframeRotation()
    {
        return reflectionEnabled ? reflectionAxisStore : transform.rotation;
    }
}
