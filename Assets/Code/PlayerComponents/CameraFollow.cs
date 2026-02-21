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
    private Vector3 _playerPos;
    private Vector3 _playerWeaponPos;
    private float _biasTowardsWeapon = 0.0f;
    private static CinemachineFreeLook _freeLook;
    private static float _baseWaitTime;
    private static float _baseCenteringTime;

    public static event Action<CameraBehaviorZone> OnCameraFollowTriggerStay;
    public static event Action<CameraBehaviorZone> OnCameraFollowTriggerStart;

    private void Start()
    {
        _playerPos = PlayerFsm.Singleton.transform.position;
        transform.position = PlayerFsm.Singleton.transform.position;
        transform.rotation = PlayerFsm.Singleton.transform.rotation;
        _freeLook = FindObjectOfType<CinemachineFreeLook>();
        _baseCenteringTime = _freeLook.m_RecenterToTargetHeading.m_RecenteringTime;
        _baseWaitTime = _freeLook.m_RecenterToTargetHeading.m_WaitTime;
        
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

    private void OnEnable()
    {
        PlayerFsm.OnPlayerPositionUpdated += UpdatePlayerPosition;
        PlayerWeaponFsm.OnPlayerWeaponPositionUpdated += UpdatePlayerWeaponPosition;
    }

    private void OnDisable()
    {
        PlayerFsm.OnPlayerPositionUpdated -= UpdatePlayerPosition;
        PlayerWeaponFsm.OnPlayerWeaponPositionUpdated -= UpdatePlayerWeaponPosition;
    }
    
    void UpdatePlayerPosition(Vector3 pos, bool grounded)
    {
        if (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Dead) || PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Dying1)) return;
        
        pos = CameraFollowTarget.Singleton.transform.position;
        var yLerp = PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.CutsceneWary)
            ? YLerpRate * 4f
            : YLerpRate;

        var playerYVelocity = PlayerFsm.Singleton.GetYVelocity();
        if (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Updraft) || PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.SlideLateral))
        {
            pos += Vector3.up * Mathf.Lerp(-3f, 5f, Mathf.InverseLerp(0, 60f, playerYVelocity));
        }

        yLerp = Mathf.Lerp(yLerp, yLerp * 1.25f, Mathf.InverseLerp(-5f, -30f, playerYVelocity));
        var newY = Mathf.Lerp(transform.position.y, pos.y, Time.deltaTime * yLerp);
        _playerPos = new Vector3(pos.x, newY, pos.z);
    }
    
    void UpdatePlayerWeaponPosition(Vector3 pos, bool active)
    {
        _biasTowardsWeapon = Mathf.Lerp(_biasTowardsWeapon, active ? 1.0f: 0.0f, Time.deltaTime * 10f);
        pos = new Vector3(pos.x, PlayerFsm.Singleton.transform.position.y, pos.z);
        _playerWeaponPos = Vector3.Lerp(_playerWeaponPos, pos, Time.deltaTime * 5f);
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
        
        
        
        transform.position = Vector3.Lerp(_playerPos, _playerWeaponPos, Mathf.Lerp(0.0f, 0.65f, _biasTowardsWeapon));
        
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
