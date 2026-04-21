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

    public static event Action<CameraBehaviorZone> OnCameraFollowTriggerStay;
    private float _currentYOffset = 0;

    private float _currentXZLerp;
    private float _currentYLerp;

    private Vector3 _currentDialogueOffset;

    private void Start()
    {
        transform.position = PlayerFsm.Singleton.transform.position;
        transform.rotation = PlayerFsm.Singleton.transform.rotation;
        _currentXZLerp = 100f;
        _currentYLerp = YLerpRate;
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

    private void OnEnable()
    {
        TestCutsceneFsm.OnIntroCutsceneGondolaTeleported += OnWarp;
    }

    private void OnDisable()
    {
        TestCutsceneFsm.OnIntroCutsceneGondolaTeleported -= OnWarp;
    }

    private void OnWarp(Vector3 delta)
    {
        transform.position += delta;
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
        

        

        
        if (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Dead) || PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Dying1)) return;
        
        var pos = CameraFollowTarget.Singleton.transform.position;
        var yLerp = PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.CutsceneWary)
            ? YLerpRate * 4f
            : YLerpRate;

        var xzLerp = 100f;
        var newYOffset = 0f;
        
        if (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Slide) || PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.FallAfterSlideLateral))
        {
            newYOffset = -5f;
        }
        
        if (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.RopeSwing) || PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.RopeSwingHoming))
        {
            yLerp *= 0.15f;
            xzLerp *= 0.15f;
            newYOffset = -1f;
        }
        
        if (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.SurgeDash))
        {
            xzLerp *= Mathf.Lerp(0.15f, 1f, Mathf.InverseLerp(0f, 0.3f, PlayerFsm.Singleton.TimeInCurrentState()));
        }

        var playerYVelocity = PlayerFsm.Singleton.GetYVelocity();
        if (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Updraft))
        {
            newYOffset = Mathf.Lerp(-3f, 5f, Mathf.InverseLerp(0, 60f, playerYVelocity));
        }
        
        yLerp = Mathf.Lerp(yLerp, yLerp * 2.75f, Mathf.InverseLerp(-5f, -30f, playerYVelocity));
        _currentYOffset = Mathf.Lerp(_currentYOffset, newYOffset, Time.deltaTime * 2f);
        pos += Vector3.up * _currentYOffset;


        _currentXZLerp = Mathf.Lerp(_currentXZLerp, xzLerp, Time.deltaTime * 15f);
        _currentYLerp = Mathf.Lerp(_currentYLerp, yLerp, Time.deltaTime * 15f);



        var newX = Mathf.Lerp(transform.position.x, pos.x, Time.deltaTime * _currentXZLerp);
        var newY = Mathf.Lerp(transform.position.y, pos.y, Time.deltaTime * _currentYLerp);
        var newZ = Mathf.Lerp(transform.position.z, pos.z, Time.deltaTime * _currentXZLerp);
        transform.position = new Vector3(newX, newY, newZ);

        var desiredDialogueOffset = Vector3.zero;
        if (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Dialogue))
        {
            var controller = DialogueCanvas.Singleton.currentDialogueController;
            if (controller != null)
            {
                var lookAt = controller.LookAtOverride == null
                    ? controller.transform
                    : controller.LookAtOverride;

                desiredDialogueOffset = (lookAt.position - transform.position) * controller.cameraFollowOffsetLerp;
            }
        }
        
        print(desiredDialogueOffset);

        _currentDialogueOffset = Vector3.Lerp(_currentDialogueOffset, desiredDialogueOffset, Time.deltaTime * 5f);
        transform.position += desiredDialogueOffset;
        
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
