using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    
    public static event Action OnSettingsMenuClosed;
    public Slider cameraSensitivitySlider;
    public Slider foliageRenderDistanceSlider;
    public Toggle ambientParticlesToggle;
    public Toggle fpsToggle;
    private PlayerInput _playerInput;
    
    // Start is called before the first frame update
    void Awake()
    {
        TryGetComponent(out _playerInput);
    }

    // Update is called once per frame
    void Update()
    {
        if (NeedToSelect())
        {
            cameraSensitivitySlider.Select();
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
        cameraSensitivitySlider.value = metaSaveData.cameraSensitivityModifier;
        ambientParticlesToggle.isOn = metaSaveData.enableAmbientParticles;
        fpsToggle.isOn = metaSaveData.enableFpsDisplay;
        foliageRenderDistanceSlider.value = metaSaveData.foliageRenderDistanceLevel;
    }

    public void OnBackClicked()
    {
        gameObject.SetActive(false);
        MetaSaveSystem.WriteCameraSensitivityModifier((int)cameraSensitivitySlider.value);
        MetaSaveSystem.WriteEnableFpsDisplay(fpsToggle.isOn);
        MetaSaveSystem.WriteAmbientParticlesEnabled(ambientParticlesToggle.isOn);
        
        MetaSaveSystem.WriteFoliageRenderDistance((int)foliageRenderDistanceSlider.value);
        OnSettingsMenuClosed?.Invoke();
    }
    
    public void OnResetClicked()
    {
        cameraSensitivitySlider.value = 10;
        ambientParticlesToggle.isOn = true;
        fpsToggle.isOn = true;
    }
    
}
