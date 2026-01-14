using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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
        
        var neighbors =Physics.OverlapCapsule(transform.position, transform.position, 0.5f, LayerMask.GetMask("CameraBehaviorZone"));
        foreach (var neighbor in neighbors)
        {
            neighbor.TryGetComponent(out CameraBehaviorZone _cameraBehaviorZone);
            if (_cameraBehaviorZone == null) continue;
            OnCameraFollowTriggerStart?.Invoke(_cameraBehaviorZone);
            break;
        }
        
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
        pos = CameraFollowTarget.Singleton.transform.position;
        var yLerp = PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.CutsceneWary)
            ? YLerpRate * 4f
            : YLerpRate;

        if (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Updraft))
        {
            pos -= Vector3.up * 3f;
        }

        yLerp = Mathf.Lerp(yLerp, yLerp * 1.25f, Mathf.InverseLerp(-5f, -30f, PlayerFsm.Singleton.GetSummedYVelocity()));
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
        transform.position = Vector3.Lerp(_playerPos, _playerWeaponPos, Mathf.Lerp(0.0f, 0.65f, _biasTowardsWeapon));
        
        var neighbors = Physics.OverlapSphere(PlayerFsm.Singleton.transform.position, 0.5f, LayerMask.GetMask("CameraBehaviorZone"),
            QueryTriggerInteraction.Collide);
        foreach (var neighbor in neighbors)
        {
            OnCameraFollowTriggerStay?.Invoke(neighbor.GetComponent<CameraBehaviorZone>());
            break;
        }
        
    }
}
