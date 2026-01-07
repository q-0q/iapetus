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
    private float _timeSinceRecenter;
    private float _recenterTime;

    private CameraBehaviorZone _currentCameraBehaviorZone;
    private float _rampUpTime;

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
        _recenterTime = 1f;
        _rampUpTime = 6f;
    }

    private void Update()
    {

        HandleCameraBehaviorZone();
        
        var lookVector2 = _playerInput.actions["Look"].ReadValue<Vector2>();
        _timeSincePlayerLookInput += Time.deltaTime;
        _timeSinceRecenter += Time.deltaTime;
        if (_currentCameraBehaviorZone == null) _timeSinceRecenter = 0f;
        if (lookVector2.magnitude < 0.01f && _timeSincePlayerLookInput > _recenterTime) return;
        if (lookVector2.magnitude > 0.01f)
        {
            _timeSincePlayerLookInput = 0f;
            _timeSinceRecenter = 0f;
        }
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
        if (cameraBehaviorZone == null || _currentCameraBehaviorZone == null)
        {
            _currentCameraBehaviorZone = cameraBehaviorZone;
            return;
        }
        
        if (cameraBehaviorZone.priority > _currentCameraBehaviorZone.priority)
        {
            _timeSinceRecenter = 0f;
            _currentCameraBehaviorZone = cameraBehaviorZone;
        };
    }

    private void HandleCameraBehaviorZone()
    {
        if (_timeSincePlayerLookInput < _recenterTime) return;
        if (_currentCameraBehaviorZone == null) return;
        
        var newForward = _currentCameraBehaviorZone.GetCameraForward(PlayerFsm.Singleton.transform.position);
        var xAngle = Vector3.SignedAngle(Vector3.forward, newForward, transform.up);
        
        // converting to quats prevents wraparound issues
        var oldXQuat = Quaternion.Euler(0, _freeLook.m_XAxis.Value, 0);
        var newXQuat = Quaternion.Euler(0, xAngle, 0);
        
        
        var lerpStrength = Mathf.Lerp(1f, 4f, Mathf.InverseLerp(_recenterTime, _recenterTime + _rampUpTime, _timeSinceRecenter));
        
        _freeLook.m_XAxis.Value = Quaternion.Lerp(oldXQuat, newXQuat, Time.deltaTime * lerpStrength).eulerAngles.y;
        _freeLook.m_YAxis.Value = Mathf.Lerp(_freeLook.m_YAxis.Value, 0.7f, Time.deltaTime * lerpStrength);


    }
}
