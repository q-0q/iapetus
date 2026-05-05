using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CustomFogManager : MonoBehaviour
{
    public static CustomFogManager Singleton;

    private void Awake()
    {
        Singleton = this;
    }

    private CustomFogController _currentController;
    public static readonly List<CustomFogController> CustomFogControllerRegistry = new();
    public const float LerpStrength = 1f;

    public void SetCurrentController(CustomFogController controller, bool snap = false)
    {
        _currentController = controller;
        if (snap) LerpAllValues(-1f);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerFsm.Singleton == null) return;


        _currentController = null;
        UpdateCurrentController(true);
    }

    private void UpdateCurrentController(bool snap)
    {
        if (PlayerFsm.Singleton == null) return;
        
        var colliders = Physics.OverlapSphere(PlayerFsm.Singleton.transform.position, 5f,
            LayerMask.GetMask("CustomFogController"), QueryTriggerInteraction.Collide);
        CustomFogController _currentHighestPriorityController = null;
        foreach (var collider in colliders)
        {
            collider.TryGetComponent(out CustomFogController controller);
            if (collider == null) continue;
            if (controller.Priority < 0) continue;
            if (_currentHighestPriorityController != null && _currentHighestPriorityController.Priority >= controller.Priority) continue;
            _currentHighestPriorityController = controller;
            SetCurrentController(controller, snap);
        }
    }

    // Update is called once per frame
    public void Update()
    {
        if (!Application.isPlaying)
        {
            ApplyEditorSettings();
            return;
        }
        UpdateCurrentController(false);
        if (_currentController == null) return;
        LerpAllValues(LerpStrength * _currentController.LerpStrengthMultiplier);
    }

    private void LerpAllValues(float strength)
    {
        if (_currentController == null) return;
            
        LerpColor("_CustomFogColor", _currentController.Color, strength);
            
        LerpFloat("_CustomFogYMin", _currentController.YMin, strength);
        LerpFloat("_CustomFogYMax", _currentController.YMax, strength);
        LerpFloat("_CustomFogYPower", _currentController.YPower, strength);
        LerpFloat("_CustomFogMinimumYFactor", _currentController.MinimumYFactor, strength);
        
        LerpFloat("_CustomFogYAddMin", _currentController.YAddMin, strength);
        LerpFloat("_CustomFogYAddMax", _currentController.YAddMax, strength);
        LerpFloat("_CustomFogYAddPower", _currentController.YAddPower, strength);
        LerpFloat("_CustomFogYAddDepthInversion", _currentController.YAddDepthInversion, strength);
        LerpFloat("_CustomFogYAddClamp", _currentController.YAddClamp, strength);
            
        LerpFloat("_CustomFogDepthMin", _currentController.DepthMin, strength);
        LerpFloat("_CustomFogDepthMax", _currentController.DepthMax, strength);
        LerpFloat("_CustomFogDepthPower", _currentController.DepthPower, strength);
            
        LerpFloat("_CustomFogNoiseSubtractionAmount", _currentController.NoiseSubtractionAmount, strength);
        LerpFloat("_CustomFogNoiseAScale", _currentController.NoiseAScale, strength);
        LerpFloat("_CustomFogNoiseBScale", _currentController.NoiseBScale, strength);
            
        LerpVector("_CustomFogNoiseAVelocity", _currentController.NoiseAVelocity, strength);
        LerpVector("_CustomFogNoiseBVelocity", _currentController.NoiseBVelocity, strength);
        
        LerpFloat("_CustomFogSkyboxLift", _currentController.SkyboxLift, strength);
    }

    private static void ApplyEditorSettings()
    {
        
        Shader.SetGlobalColor("_CustomFogColor", Color.darkGray);
        Shader.SetGlobalFloat("_CustomFogYMin", -10001f);
        Shader.SetGlobalFloat("_CustomFogYMax", -10000f);
        Shader.SetGlobalFloat("_CustomFogMinimumYFactor", 0);
        
        Shader.SetGlobalFloat("_CustomFogYAddMin", -10001f);
        Shader.SetGlobalFloat("_CustomFogYAddMax", -10000f);
        
        Shader.SetGlobalFloat("_CustomFogDepthMin", 10000f);
        Shader.SetGlobalFloat("_CustomFogDepthMax", 10001f);
    }

    private static void LerpFloat(string name, float value, float strength)
    {
        if (strength < 0)
        {
            Shader.SetGlobalFloat(name, value);
            return;
        }
        Shader.SetGlobalFloat(name, Mathf.Lerp(Shader.GetGlobalFloat(name), value, Time.deltaTime * strength));
    }
    
    private static void LerpColor(string name, Color value, float strength)
    {
        if (strength < 0)
        {
            Shader.SetGlobalColor(name, value);
            return;
        }
        Shader.SetGlobalColor(name, Color.Lerp(Shader.GetGlobalColor(name), value, Time.deltaTime * strength));
    }

    private static void LerpVector(string name, Vector3 value, float strength)
    {
        if (strength < 0)
        {
            Shader.SetGlobalVector(name, value);
            return;
        }
        Shader.SetGlobalVector(name, Vector3.Lerp(Shader.GetGlobalVector(name), value, Time.deltaTime * strength));
    }
    

}
