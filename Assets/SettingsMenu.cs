using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    
    public static event Action OnSettingsMenuClosed;
    private Slider _cameraSensitivitySlider;
    private Toggle _ambientParticlesToggle;
    private Toggle _fpsToggle;
    
    // Start is called before the first frame update
    void Awake()
    {
        _cameraSensitivitySlider = transform.Find("Holder").Find("CameraSensitivitySlider").GetComponent<Slider>();
        _ambientParticlesToggle = transform.Find("Holder").Find("AmbientParticlesToggle").GetComponent<Toggle>();
        _fpsToggle = transform.Find("Holder").Find("FPSToggle").GetComponent<Toggle>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        var metaSaveData = MetaSaveSystem.LoadMetaSaveData();
        if (metaSaveData == null) return;
        _cameraSensitivitySlider.normalizedValue =
            Mathf.InverseLerp(0.25f, 1.75f, metaSaveData.cameraSensitivityModifier);
        _ambientParticlesToggle.isOn = metaSaveData.enableAmbientParticles;
        _fpsToggle.isOn = metaSaveData.enableFpsDisplay;
    }

    public void OnBackClicked()
    {
        gameObject.SetActive(false);
        OnSettingsMenuClosed?.Invoke();
    }

    public void OnApplyClicked()
    {
        var cameraSensitivityModifier = Mathf.Lerp(0.25f, 1.75f, _cameraSensitivitySlider.normalizedValue);
        MetaSaveSystem.WriteMetaSaveData(0, cameraSensitivityModifier, _ambientParticlesToggle.isOn, _fpsToggle.isOn);
    }
    
    public void OnResetClicked()
    {
        _cameraSensitivitySlider.normalizedValue = 0.5f;
        _ambientParticlesToggle.isOn = true;
        _fpsToggle.isOn = true;
    }
    
}
