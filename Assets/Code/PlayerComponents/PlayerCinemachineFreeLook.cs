using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCinemachineFreeLook : MonoBehaviour
{
    private CinemachineFreeLook _freeLook;
    private PlayerInput _playerInput;
    private float _baseXSpeed;
    private float _baseYSpeed;
    private float _timeSincePlayerLookInput;
    private float _recenterTime;

    private CameraBehaviorZone _currentCameraBehaviorZone;

    void Awake()
    {
        TryGetComponent(out _freeLook);
        _baseXSpeed = _freeLook.m_XAxis.m_MaxSpeed;
        _baseYSpeed = _freeLook.m_YAxis.m_MaxSpeed;
    }

    private void Start()
    {
        PlayerFsm.Singleton.gameObject.TryGetComponent(out _playerInput);
        _timeSincePlayerLookInput = 0f;
        _recenterTime = 0.5f;
    }

    private void Update()
    {

        HandleCameraBehaviorZone();
        
        var lookVector2 = _playerInput.actions["Look"].ReadValue<Vector2>();
        _timeSincePlayerLookInput += Time.deltaTime;
        if (lookVector2.magnitude < 0.01f && _timeSincePlayerLookInput > _recenterTime) return;
        if (lookVector2.magnitude > 0.01f) _timeSincePlayerLookInput = 0f;
        _freeLook.m_XAxis.m_InputAxisValue = lookVector2.x;
        _freeLook.m_YAxis.m_InputAxisValue = lookVector2.y;
    }

    private void OnEnable()
    {
        MetaSaveSystem.OnMetaSaveDataUpdated += OnMetaSaveDataUpdated;
        CameraFollow.OnCameraFollowTriggerStay += OnCameraFollowTriggerStay;
    }

    private void OnDisable()
    {
        MetaSaveSystem.OnMetaSaveDataUpdated -= OnMetaSaveDataUpdated;
        CameraFollow.OnCameraFollowTriggerStay -= OnCameraFollowTriggerStay;
    }
    
    private void OnMetaSaveDataUpdated(MetaSaveSystem.MetaSaveData metaSaveData)
    {
        _freeLook.m_XAxis.m_MaxSpeed = _baseXSpeed * metaSaveData.cameraSensitivityModifier;
        _freeLook.m_YAxis.m_MaxSpeed = _baseYSpeed * metaSaveData.cameraSensitivityModifier;
    }
    
    private void OnCameraFollowTriggerStay(CameraBehaviorZone cameraBehaviorZone)
    {
        _currentCameraBehaviorZone = cameraBehaviorZone;
    }

    private void HandleCameraBehaviorZone()
    {
        if (_timeSincePlayerLookInput < _recenterTime) return;
        if (_currentCameraBehaviorZone == null) return;
        
        var newForward = _currentCameraBehaviorZone.GetCameraForward(PlayerFsm.Singleton.transform.position);
        var angle = Vector3.SignedAngle(Vector3.forward, newForward, transform.up);
        
        // converting to quats prevents wraparound issues
        var oldQuat = Quaternion.Euler(0, _freeLook.m_XAxis.Value, 0);
        var newQuat = Quaternion.Euler(0, angle, 0);
        _freeLook.m_XAxis.Value = Quaternion.Lerp(oldQuat, newQuat, Time.deltaTime * 10f).eulerAngles.y;
    }
}
