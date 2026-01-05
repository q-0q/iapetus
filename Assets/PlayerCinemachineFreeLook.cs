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
        
        var newForward = transform.forward;
        
        if (_currentCameraBehaviorZone.cameraBehavior == CameraBehaviorZone.CameraBehavior.LookAtPoint)
        {
            newForward = (_currentCameraBehaviorZone.InputVector3 + _currentCameraBehaviorZone.transform.position - PlayerFsm.Singleton.transform.position) * (_currentCameraBehaviorZone.invertDirection ? -1f : 1f);
        }
        
        if (_currentCameraBehaviorZone.cameraBehavior == CameraBehaviorZone.CameraBehavior.LookInDirection)
        {
            newForward = (_currentCameraBehaviorZone.InputVector3) * (_currentCameraBehaviorZone.invertDirection ? -1f : 1f);
        }
        
        var angle = Vector3.SignedAngle(Vector3.forward, newForward, Vector3.up);
        // _freeLook.m_XAxis.m_InputAxisValue = angle * 0.1f;
        
        _freeLook.m_XAxis.Value = Mathf.Lerp(_freeLook.m_XAxis.Value, angle, Time.deltaTime * 10f);
    }
}
