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

    private CameraBehaviorZone _currentCameraBehaviorZone;
    private float _rampUpTime;
    
    

    private bool _scriptActive;
    private Vector3 _scriptTargetDirection;
    private float _scriptDuration;
    private float _timeInScript;

    private bool _isAutocamEnabled;

    private bool _settingsMenuOpen;

    private CinemachineBrain _brain;

    private float _baseFov;
    private bool _preventYRecenter;

    void Awake()
    {
        Singleton = this;
        TryGetComponent(out _freeLook);
        _isAutocamEnabled = true;
        _baseXSpeed = _freeLook.m_XAxis.m_MaxSpeed;
        _baseYSpeed = _freeLook.m_YAxis.m_MaxSpeed;
        _settingsMenuOpen = false;
        _brain = FindObjectOfType<CinemachineBrain>();
        _baseFov = _freeLook.m_Lens.FieldOfView;
        _freeLook.m_YAxis.Value = 0.55f;
        _freeLook.m_YAxis.m_InputAxisValue = 0f;
        _freeLook.m_XAxis.m_InputAxisValue = 0f;
        _preventYRecenter = false;
        
        OnMetaSaveDataUpdated(MetaSaveSystem.LoadCachedMetaSaveData());
    }

    private void Start()
    {
        PlayerFsm.Singleton.gameObject.TryGetComponent(out _playerInput);
        _timeSincePlayerLookInput = 0f;
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
        if (_scriptActive || _brain.ActiveVirtualCamera != _freeLook || GameMenu.Singleton.IsMenuOpen() || !InventoryMenuFsm.Singleton.Machine.IsInState(InventoryMenuFsm.InventoryMenuFsmState.Closed) || PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.TrialTeleport) || CutsceneManager.Singleton.IsCutsceneCameraDisabled() || PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.UseMap))
        {
            _freeLook.m_XAxis.m_InputAxisValue = 0;
            _freeLook.m_XAxis.Reset();
            _freeLook.m_YAxis.m_InputAxisValue = 0;
            _freeLook.m_YAxis.Reset();
            return;
        }

        if (_currentCameraBehaviorZone != null)
        {
            if (!_currentCameraBehaviorZone.gameObject.activeInHierarchy) OnCameraFollowTriggerStay(null);
        }
        
        HandleCameraBehaviorZone();
        
        var lookVector2 = _playerInput.actions["Look"].ReadValue<Vector2>() * Time.deltaTime;
        _timeSincePlayerLookInput += Time.deltaTime;

        if (_timeSincePlayerLookInput >= 2.5f && !_preventYRecenter && DialogueCanvas.Singleton.TimeSinceDialogueClosed > 2.5f)
        {
            var y = 0.55f;
            if (DialogueCanvas.Singleton.currentDialogueController != null)
            {
                y = DialogueCanvas.Singleton.currentDialogueController.CameraY;
            }
            
            _freeLook.m_YAxis.Value = Mathf.Lerp(_freeLook.m_YAxis.Value, y, Time.deltaTime * Mathf.Lerp(0.25f, 2f, Mathf.InverseLerp(2.5f, 3.5f, _timeSincePlayerLookInput)));
        }
        if (lookVector2.magnitude < 0.01f)
        {
            _freeLook.m_XAxis.m_InputAxisValue = 0;
            _freeLook.m_YAxis.m_InputAxisValue = 0;
            return;
        }
        _timeSincePlayerLookInput = 0f;

        if (InputTypeManager.Singleton.GetCurrentInputType() == InputTypeManager.InputType.Pad) lookVector2 *= 5f;
        _freeLook.m_XAxis.m_InputAxisValue = lookVector2.x;
        _freeLook.m_YAxis.m_InputAxisValue = lookVector2.y;
    }

    private void OnEnable()
    {
        MetaSaveSystem.OnMetaSaveDataUpdated += OnMetaSaveDataUpdated;
        CameraFollow.OnCameraFollowTriggerStay += OnCameraFollowTriggerStay;
        TestCutsceneFsm.OnIntroCutsceneGondolaTeleported += OnWarp;
        PlayerFsm.OnPlayerTeleported += OnWarp;
    }

    private void OnDisable()
    {
        MetaSaveSystem.OnMetaSaveDataUpdated -= OnMetaSaveDataUpdated;
        CameraFollow.OnCameraFollowTriggerStay -= OnCameraFollowTriggerStay;
        TestCutsceneFsm.OnIntroCutsceneGondolaTeleported -= OnWarp;
        PlayerFsm.OnPlayerTeleported -= OnWarp;
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
        
    }

    private void ForceRecenter()
    {
        // _timeSinceRecenter = _recenterTime + _rampUpTime;
    }

    public void OnPlayerCinemachineFreeLookScript(Vector3 direction, float duration, float y = 0.7f, float linger = 0f)
    {
        if (_scriptActive) return;
        StartCoroutine(InvokeScript(direction, duration));
        IEnumerator InvokeScript(Vector3 direction, float duration)
        {
            direction = new Vector3(direction.x, 0f, direction.z);
            var xAngle = Vector3.SignedAngle(Vector3.forward, direction, transform.up);
            var newXQuat = Quaternion.Euler(0, xAngle, 0);
            var oldXQuat = Quaternion.Euler(0, _freeLook.m_XAxis.Value, 0);

            var oldY = _freeLook.m_YAxis.Value;
            
            _scriptActive = true;
            
            float t = 0;
            while (t < duration)
            {
                var w = Util.SmoothLerp01( t / duration);
                _freeLook.m_XAxis.Value = Quaternion.Lerp(oldXQuat, newXQuat, w).eulerAngles.y;
                _freeLook.m_YAxis.Value = Mathf.Lerp(oldY, y, w);
                t += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(linger);
            
            _scriptActive = false;  
        }
    }

    public void SetAxes(float xValue, float yValue)
    {
        _freeLook.m_XAxis.Value = xValue;
        _freeLook.m_YAxis.Value = yValue;

    }

    public CinemachineFreeLook GetFreeLook()
    {
        return _freeLook;
    }

    public float GetBaseFov()
    {
        return _baseFov;
    }

    public IEnumerator PreventYRecenterForDuration(float duration)
    {
        _preventYRecenter = true;
        yield return new WaitForSeconds(duration);
        _preventYRecenter = false;
    }

    
}
