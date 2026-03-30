using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    
    public static event Action OnSettingsMenuClosed;
    private Slider _cameraSensitivitySlider;
    // private Toggle _ambientParticlesToggle;
    private Toggle _autocamEnabledToggle;
    private Toggle _fpsToggle;
    private PlayerInput _playerInput;
    
    // Start is called before the first frame update
    void Awake()
    {
        TryGetComponent(out _playerInput);
        _cameraSensitivitySlider = transform.Find("Holder").Find("CameraSensitivitySlider").GetComponent<Slider>();
        // _ambientParticlesToggle = transform.Find("Holder").Find("AmbientParticlesToggle").GetComponent<Toggle>();
        _autocamEnabledToggle = transform.Find("Holder").Find("AutocamEnabledToggle").GetComponent<Toggle>();
        _fpsToggle = transform.Find("Holder").Find("FPSToggle").GetComponent<Toggle>();
    }

    // Update is called once per frame
    void Update()
    {
        if (NeedToSelect())
        {
            _cameraSensitivitySlider.Select();
        }
    }
    
    private bool NeedToSelect()
    {
        foreach (var selectable in GetComponentsInChildren<Selectable>())
        {
            if (EventSystem.current.currentSelectedGameObject == selectable.gameObject)
            {
                return false;
            };
        }
        
        return _playerInput.actions["Navigate"].ReadValue<Vector2>().magnitude > 0.1f;
    }

    private void OnEnable()
    {
        var metaSaveData = MetaSaveSystem.LoadCachedMetaSaveData();
        if (metaSaveData == null) return;
        _cameraSensitivitySlider.value = metaSaveData.cameraSensitivityModifier;
        // _ambientParticlesToggle.isOn = metaSaveData.enableAmbientParticles;
        _autocamEnabledToggle.isOn = metaSaveData.autoCamEnabled;
        _fpsToggle.isOn = metaSaveData.enableFpsDisplay;
    }

    public void OnBackClicked()
    {
        gameObject.SetActive(false);
        MetaSaveSystem.WriteCameraSensitivityModifier((int)_cameraSensitivitySlider.value);
        MetaSaveSystem.WriteEnableFpsDisplay(_fpsToggle.isOn);
        MetaSaveSystem.WriteEnableAutocam(_autocamEnabledToggle.isOn);
        OnSettingsMenuClosed?.Invoke();
    }

    public void OnApplyClicked()
    {
        // TODO: Batch these to save on IO
        MetaSaveSystem.WriteCameraSensitivityModifier((int)_cameraSensitivitySlider.value);
        MetaSaveSystem.WriteEnableFpsDisplay(_fpsToggle.isOn);
        MetaSaveSystem.WriteEnableAutocam(_autocamEnabledToggle.isOn);
        OnBackClicked();
    }
    
    public void OnResetClicked()
    {
        _cameraSensitivitySlider.value = 10;
        // _ambientParticlesToggle.isOn = true;
        _autocamEnabledToggle.isOn = false;
        _fpsToggle.isOn = true;
    }
    
}
