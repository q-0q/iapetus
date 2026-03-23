using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Code.Fsm.TrialCollectibleFSM;
using Code.Misc;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCinemachineFreeLook : MonoBehaviour
{
    public static PlayerCinemachineFreeLook Singleton;
    private CinemachineFreeLook _freeLook;
    private PlayerInput _playerInput;
    private float _baseXSpeed;
    private float _baseYSpeed;
    private float _timeSincePlayerLookInput;
    private float _timeSinceRecenter;
    private float _recenterTime;

    private CameraBehaviorZone _currentCameraBehaviorZone;
    private float _rampUpTime;
    
    

    private bool _scriptActive;
    private Vector3 _scriptTargetDirection;
    private float _scriptDuration;
    private float _timeInScript;

    private bool _isAutocamEnabled;

    private bool _settingsMenuOpen;

    void Awake()
    {
        Singleton = this;
        TryGetComponent(out _freeLook);
        _isAutocamEnabled = true;
        _baseXSpeed = _freeLook.m_XAxis.m_MaxSpeed;
        _baseYSpeed = _freeLook.m_YAxis.m_MaxSpeed;
        _settingsMenuOpen = false;
        
        OnMetaSaveDataUpdated(MetaSaveSystem.LoadCachedMetaSaveData());
    }

    private void Start()
    {
        PlayerFsm.Singleton.gameObject.TryGetComponent(out _playerInput);
        _timeSincePlayerLookInput = 0f;
        _recenterTime = 1.5f;
        _rampUpTime = 8f;
        
        
        var xAngle = Vector3.SignedAngle(Vector3.forward, PlayerFsm.Singleton.transform.forward, Vector3.up);
        _freeLook.m_XAxis.Value = xAngle;
        
        var highestPriorityZone = CameraFollow.HighestPriorityZoneAtPosition(PlayerFsm.Singleton.transform.position);
        if (highestPriorityZone != null)
        {
            highestPriorityZone.GetCameraForward(PlayerFsm.Singleton.transform.position, out var y);
            _freeLook.m_YAxis.Value = y;
        }
        
        var saveData = SaveSystem.LoadCachedSaveData();
        var positionIdTransform = Util.FindGamePositionById(saveData.playerInGamePositionId, out var cameraRotationOffset);
        AddXAxisOffset(cameraRotationOffset);
    }

    private void Update()
    {
        if (_scriptActive) return;
        if (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.TrialTeleport)) return;
        if (CutsceneManager.Singleton.IsCutsceneCameraDisabled()) return;
        if (GameMenu.Singleton.IsMenuOpen())
        {
            _freeLook.m_XAxis.m_InputAxisValue = 0;
            _freeLook.m_YAxis.m_InputAxisValue = 0;
            return;
        }

        if (_currentCameraBehaviorZone != null)
        {
            if (!_currentCameraBehaviorZone.gameObject.activeInHierarchy) OnCameraFollowTriggerStay(null);
        }
        
        HandleCameraBehaviorZone();
        
        var lookVector2 = _playerInput.actions["Look"].ReadValue<Vector2>() * Time.deltaTime;
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
        TrialCollectibleKeyframe.OnTrialCollectibleCameraZoneUpdated += ForceRecenter;
        TestCutsceneFsm.OnIntroCutsceneGondolaTeleported += OnWarp;
    }

    private void OnDisable()
    {
        MetaSaveSystem.OnMetaSaveDataUpdated -= OnMetaSaveDataUpdated;
        CameraFollow.OnCameraFollowTriggerStay -= OnCameraFollowTriggerStay;
        TrialCollectibleKeyframe.OnTrialCollectibleCameraZoneUpdated -= ForceRecenter;
        TestCutsceneFsm.OnIntroCutsceneGondolaTeleported -= OnWarp;
    }
    
    private void OnMetaSaveDataUpdated(MetaSaveSystem.MetaSaveData metaSaveData)
    {
        _isAutocamEnabled = metaSaveData.autoCamEnabled;
        _freeLook.m_XAxis.m_MaxSpeed = _baseXSpeed * metaSaveData.cameraSensitivityModifier * 0.1f;
        _freeLook.m_YAxis.m_MaxSpeed = _baseYSpeed * metaSaveData.cameraSensitivityModifier * 0.1f;
    }

    private void OnWarp(Vector3 delta)
    {
        // StartCoroutine(Coroutine());
        CinemachineCore.Instance.OnTargetObjectWarped(_freeLook.Follow, delta);
        // IEnumerator Coroutine()
        // {
        //     for (int i = 0; i < 6; i++)
        //     {
        //         yield return null;
        //     }
        // }
    }

    // private void OnSettingsMenuOpened()
    // {
    //     _settingsMenuOpen = true;
    // }
    //
    // private void OnSettingsMenuClosed()
    // {
    //     _settingsMenuOpen = false;
    // }
    //
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
    
    
    public void AddXAxisOffset(float value)
    {
        _freeLook.m_XAxis.Value += value;
    }

    private void HandleCameraBehaviorZone()
    {
        
        if(!_isAutocamEnabled) return;
        if (_timeSincePlayerLookInput < _recenterTime) return;
        if (_currentCameraBehaviorZone == null) return;
        var newForward = _currentCameraBehaviorZone.GetCameraForward(PlayerFsm.Singleton.transform.position, out var y);

        var oldXQuat = Quaternion.Euler(0, _freeLook.m_XAxis.Value, 0);
        
        // if the player is facing away from the newForward and the camera is also pointed
        // roughly in that direction then we assume the player "knows what theyre doing" and we dont autocam
        // var playerDesiredAngleDelta = Vector3.Angle(PlayerFsm.Singleton.transform.forward, newForward);
        // var playerCurrentAngleDelta = Vector3.Angle(PlayerFsm.Singleton.transform.forward, oldXQuat * Vector3.forward);
        // if (playerDesiredAngleDelta > 100f && playerCurrentAngleDelta < 45f && PlayerFsm.Singleton.GetMomentum() > 2f) return;
        
        var xAngle = Vector3.SignedAngle(Vector3.forward, newForward, transform.up);
        
        // converting to quats prevents wraparound issues
        var newXQuat = Quaternion.Euler(0, xAngle, 0);
        
        
        var lerpStrength = Mathf.Lerp(1f, 4f, Mathf.InverseLerp(_recenterTime, _recenterTime + _rampUpTime, _timeSinceRecenter));
        
        _freeLook.m_XAxis.Value = Quaternion.Lerp(oldXQuat, newXQuat, Time.deltaTime * lerpStrength).eulerAngles.y;
        _freeLook.m_YAxis.Value = Mathf.Lerp(_freeLook.m_YAxis.Value, y, Time.deltaTime * lerpStrength);


    }

    private void ForceRecenter()
    {
        // _timeSinceRecenter = _recenterTime + _rampUpTime;
    }

    public void OnPlayerCinemachineFreeLookScript(Vector3 direction, float duration)
    {
        if (_scriptActive) return;
        StartCoroutine(InvokeScript(direction, duration));
        IEnumerator InvokeScript(Vector3 direction, float duration)
        {
            direction = new Vector3(direction.x, 0f, direction.z);
            var xAngle = Vector3.SignedAngle(Vector3.forward, direction, transform.up);
            var newXQuat = Quaternion.Euler(0, xAngle, 0);
            _scriptActive = true;
            
            float t = 0;
            while (t < duration)
            {
                var w = t / duration;
                var oldXQuat = Quaternion.Euler(0, _freeLook.m_XAxis.Value, 0);
                _freeLook.m_XAxis.Value = Quaternion.Slerp(oldXQuat, newXQuat, Time.deltaTime * 6f).eulerAngles.y;
                t += Time.deltaTime;
                yield return null;
            }
            _scriptActive = false;  
        }
        
    }

    public void SetAxes(float xValue, float yValue)
    {
        _freeLook.m_XAxis.Value = xValue;
        _freeLook.m_YAxis.Value = yValue;

    }

    
}
