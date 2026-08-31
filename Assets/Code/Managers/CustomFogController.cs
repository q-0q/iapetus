using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CustomFogController : MonoBehaviour
{

    public int Priority = 1;

    
    public Color Color = Color.white;
    
    [Header("Depth")]
    public float DepthMin = 20f;
    public float DepthMax = 600f;
    public float DepthPower = 2f;
    public float MaxDepthFactor = 1.0f;
    
    [Header("Lower Blankets")]
    public float NearLowerBlanketMin = -50f;
    public float NearLowerBlanketMax = -5f;
    public float NearLowerBlanketPower = 2f;
    public float MaxNearLowerBlanketFactor = 0.5f;
    
    public float FarLowerBlanketMin = -50;
    public float FarLowerBlanketMax = 150f;
    public float FarLowerBlanketPower = 2f;
    public float MaxFarLowerBlanketFactor = 0.5f;
    
    public float LowerBlanketDistanceMin = 20f;
    public float LowerBlanketDistanceMax = 100f;
    public float LowerBlanketDistancePower = 2f;
    
    [Header("Upper Blanket")]
    public float UpperBlanketMin = 0f;
    public float UpperBlanketMax = 500f;
    public float UpperBlanketPower = 2f;
    public float MaxUpperBlanketFactor = 0.25f;
    

    
    public float SkyboxLift = 1500f;

    [Header("Vignette")]
    public float VignetteLerpMin;
    public float VignetteLerpMax;
    public float VignetteLerpPower = 1f;
    public float VignetteAlpha;
    public Color VignetteColor;

    
    public float LerpStrengthMultiplier = 1f;
    
    public bool force = false;

    private Collider _collider;

    private void Awake()
    {
        TryGetComponent(out _collider);
    }

    private void Start()
    {
        if (!force) return;
        CustomFogManager.Singleton.SetCurrentController(this, true);
    }

    void OnEnable() => CustomFogManager.CustomFogControllerRegistry.Add(this);
    void OnDisable() => CustomFogManager.CustomFogControllerRegistry.Remove(this);



}
