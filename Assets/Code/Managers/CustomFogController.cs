using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CustomFogController : MonoBehaviour
{

    public int Priority = 1;
    public float LerpStrengthMultiplier = 1f;
    
    public Color Color = Color.white;
    
    public float YMin = -100f;
    public float YMax = 100f;
    public float YPower = 2f;
    public float MinimumYFactor = 0f;
    
    public float YAddMin = -100f;
    public float YAddMax = -40f;
    public float YAddPower = 2f;
    public float YAddDepthInversion = 0.5f;
    public float YAddClamp = 0.5f;
    
    public float DepthMin = 20f;
    public float DepthMax = 100f;
    public float DepthPower = 2f;
    public float DepthClamp = 1.0f;
    
    public float NoiseSubtractionAmount = 50f;
    public float NoiseAScale = 0.02f;
    public Vector3 NoiseAVelocity = new Vector3(2, 2, 2);
    public float NoiseBScale = 0.01f;
    public Vector3 NoiseBVelocity = new Vector3(-1, -1, -1);
    
    public float SkyboxLift = 1500f;

    public bool force = false;

    private Collider _collider;

    private void Awake()
    {
        TryGetComponent(out _collider);
    }

    private void Start()
    {
        if (!force) return;
        Shader.SetGlobalVector("_CameraFollowWorldPosition", Camera.main.transform.position);
        CustomFogManager.Singleton.SetCurrentController(this, true);
    }

    void OnEnable() => CustomFogManager.CustomFogControllerRegistry.Add(this);
    void OnDisable() => CustomFogManager.CustomFogControllerRegistry.Remove(this);



}
