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

    private void Awake()
    {
    }

    private void Start()
    {
        reflectionAxisStore = GetNewForward();
    }

    private void Update()
    {
        if (!reflectionEnabled) return;
        if (PlayerFsm.Singleton.GetMomentum() < 10f) return;
        
        // var roundedPlayerForwardEuler = new Vector3(0,  Mathf.Round(PlayerFsm.Singleton.transform.rotation.y / 45f) * 45f, 0);
        // var roundedPlayerForward = Quaternion.Euler(roundedPlayerForwardEuler) * Vector3.forward;
        // Debug.DrawRay(PlayerFsm.Singleton.transform.position, roundedPlayerForward, Color.green);
        
        var newForward = GetNewForward();

        reflectionAxisStore = Quaternion.Lerp(reflectionAxisStore, newForward, Time.deltaTime * 10f);
    }

    private Quaternion GetNewForward()
    {
        var signedAngle = Vector3.SignedAngle(transform.forward, PlayerFsm.Singleton.transform.forward, Vector3.up);
        signedAngle = signedAngle > 0 ? 90f : -90f;

        var newForwardMinimum = Quaternion.Euler(0, -reflectionAngle, 0) * transform.rotation;
        var newForwardMAximum = Quaternion.Euler(0, reflectionAngle, 0) * transform.rotation;

        var newForward = Quaternion.Lerp(newForwardMinimum, newForwardMAximum,
            Mathf.InverseLerp(-90f, 90f, signedAngle));
        return newForward;
    }

    public Quaternion GetKeyframeRotation()
    {
        return reflectionEnabled ? reflectionAxisStore : transform.rotation;
    }
}
