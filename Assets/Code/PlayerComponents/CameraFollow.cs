using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    private float YLerpRate = 2.75f;
    private static CinemachineFreeLook _freeLook;

    public static event Action<CameraBehaviorZone> OnCameraFollowTriggerStay;
    public static event Action<CameraBehaviorZone> OnCameraFollowTriggerStart;
    private float _currentYOffset = 0;
    

    private void Start()
    {
        transform.position = PlayerFsm.Singleton.transform.position;
        transform.rotation = PlayerFsm.Singleton.transform.rotation;
        _freeLook = FindObjectOfType<CinemachineFreeLook>();
        
        var highestPriorityZone = HighestPriorityZoneAtPosition(PlayerFsm.Singleton.transform.position);
        OnCameraFollowTriggerStart?.Invoke(highestPriorityZone);
        
    }

    private void OnTriggerStay(Collider other)
    {
        // other.transform.TryGetComponent(out CameraBehaviorZone cameraBehaviorZone);
        // OnCameraFollowTriggerStay?.Invoke(cameraBehaviorZone);
        
    }
    
    private void OnTriggerExit(Collider other)
    {
        OnCameraFollowTriggerStay?.Invoke(null);
    }
    
    
    private void Update()
    {
        // if (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.TrialTeleport))
        // {
        //     if (PlayerFsm.Singleton.TimeInCurrentState() > 0.5f)
        //     {
        //         transform.position = Vector3.Lerp(transform.position, PlayerFsm.Singleton.GetTeleportDestination(),
        //             Time.deltaTime * 3f);
        //         return;
        //     }
        // }
        
        if (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.TrialTeleport))
        {
            transform.position = PlayerFsm.Singleton.transform.position;
            return;
        }
        

        
        if (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Dead) || PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Dying1)) return;
        
        var pos = CameraFollowTarget.Singleton.transform.position;
        var yLerp = PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.CutsceneWary)
            ? YLerpRate * 4f
            : YLerpRate;

        var xzLerp = 100f;
        var newYOffset = 0f;
        
        if (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Slide) || PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.FallAfterSlideLateral))
        {
            newYOffset = -8f;
        }

        var playerYVelocity = PlayerFsm.Singleton.GetYVelocity();
        if (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Updraft))
        {
            newYOffset = Mathf.Lerp(-3f, 5f, Mathf.InverseLerp(0, 60f, playerYVelocity));
        }
        
        yLerp = Mathf.Lerp(yLerp, yLerp * 1.75f, Mathf.InverseLerp(-5f, -30f, playerYVelocity));
        _currentYOffset = Mathf.Lerp(_currentYOffset, newYOffset, Time.deltaTime * 2f);
        pos += Vector3.up * _currentYOffset;


        var newX = Mathf.Lerp(transform.position.x, pos.x, Time.deltaTime * xzLerp);
        var newY = Mathf.Lerp(transform.position.y, pos.y, Time.deltaTime * yLerp);
        var newZ = Mathf.Lerp(transform.position.z, pos.z, Time.deltaTime * xzLerp);
        transform.position = new Vector3(newX, newY, newZ);


        
        var highestPriorityZone = HighestPriorityZoneAtPosition(PlayerFsm.Singleton.transform.position);
        OnCameraFollowTriggerStay?.Invoke(highestPriorityZone);
        
    }

    public static CameraBehaviorZone HighestPriorityZoneAtPosition(Vector3 position)
    {
        var neighbors = Physics.OverlapSphere(position, 0.5f, LayerMask.GetMask("CameraBehaviorZone"),
            QueryTriggerInteraction.Collide);

        CameraBehaviorZone highestPriorityZone = null;
        
        foreach (var neighbor in neighbors)
        {
            var cameraBehaviorZone = neighbor.GetComponent<CameraBehaviorZone>();
            if (highestPriorityZone == null)
            {
                highestPriorityZone = cameraBehaviorZone;
            } else if (highestPriorityZone.priority < cameraBehaviorZone.priority)
            {
                highestPriorityZone = cameraBehaviorZone;
            }
            
        }

        return highestPriorityZone;
    }
}
