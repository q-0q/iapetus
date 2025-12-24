using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
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

    private void Start()
    {
        transform.position = PlayerFsm.Singleton.transform.position;
        transform.rotation = PlayerFsm.Singleton.transform.rotation;
        _freeLook = FindObjectOfType<CinemachineFreeLook>();
        _baseCenteringTime = _freeLook.m_RecenterToTargetHeading.m_RecenteringTime;
        _baseWaitTime = _freeLook.m_RecenterToTargetHeading.m_WaitTime;
    }

    private void OnTriggerStay(Collider other)
    {
        _freeLook.m_RecenterToTargetHeading.m_enabled = true;
        other.transform.TryGetComponent(out CameraBehaviorZone cameraBehaviorZone);
        if (cameraBehaviorZone.cameraBehavior == CameraBehaviorZone.CameraBehavior.LookAtPoint) transform.rotation =
            Quaternion.LookRotation((cameraBehaviorZone.InputVector3 + other.transform.position - transform.position) * (cameraBehaviorZone.invertDirection ? -1f : 1f),
                Vector3.up);
        if (cameraBehaviorZone.cameraBehavior == CameraBehaviorZone.CameraBehavior.LookInDirection) transform.rotation =
            Quaternion.LookRotation((cameraBehaviorZone.InputVector3 * (cameraBehaviorZone.invertDirection ? -1f : 1f)),
                Vector3.up);

        _freeLook.m_RecenterToTargetHeading.m_RecenteringTime =
            _baseCenteringTime * cameraBehaviorZone.centeringTimeModifier;
        
        _freeLook.m_RecenterToTargetHeading.m_WaitTime =
            _baseWaitTime * cameraBehaviorZone.waitTimeModifier;
    }
    
    private void OnTriggerExit(Collider other)
    {
        _freeLook.m_RecenterToTargetHeading.m_enabled = false;
        _freeLook.m_RecenterToTargetHeading.m_RecenteringTime = _baseCenteringTime;
        _freeLook.m_RecenterToTargetHeading.m_WaitTime = _baseWaitTime;
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
    }
}
