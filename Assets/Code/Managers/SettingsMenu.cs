using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    
    public static event Action OnSettingsMenuClosed;
    private Slider _cameraSensitivitySlider;
    // private Toggle _ambientParticlesToggle;
    private Toggle _autocamEnabledToggle;
    private Toggle _fpsToggle;
    
    // Start is called before the first frame update
    void Awake()
    {
        _cameraSensitivitySlider = transform.Find("Holder").Find("CameraSensitivitySlider").GetComponent<Slider>();
        // _ambientParticlesToggle = transform.Find("Holder").Find("AmbientParticlesToggle").GetComponent<Toggle>();
        _autocamEnabledToggle = transform.Find("Holder").Find("AutocamEnabledToggle").GetComponent<Toggle>();
        _fpsToggle = transform.Find("Holder").Find("FPSToggle").GetComponent<Toggle>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
