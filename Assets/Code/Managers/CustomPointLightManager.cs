using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CustomPointLightManager : MonoBehaviour
{
    public static CustomPointLightManager Singleton;

    private void Awake()
    {
        Singleton = this;
    }
    
    public static readonly List<CustomPointLight> CustomPointLightRegistry = new();
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    // Update is called once per frame
    public void Update()
    {
        UpdateObserversVectorArray();
    }

    
    
    private static Vector4[] _lightPositions = new Vector4[64];
    private static Vector4[] _lightLerps = new Vector4[64];
    private static Vector4[] _lightColors = new Vector4[64];
    
    private static readonly int CountID = Shader.PropertyToID("_CustomFogObserverCount");
    private static readonly int PositionsID = Shader.PropertyToID("_CustomPointLightPositions");
    private static readonly int LerpsID = Shader.PropertyToID("_CustomPointLightPositions");
    private static readonly int ColorsID = Shader.PropertyToID("_CustomPointLightColors");
    
    private void UpdateObserversVectorArray()
    {
        int count = Mathf.Min(CustomPointLightRegistry.Count, 64);
        
        for (int i = 0; i < 64; i++)
        {
            if (i < count)
            {
                var l = CustomPointLightRegistry[i];
                Vector3 pos = l.transform.position;
                _lightPositions[i] = new Vector4(pos.x, pos.y, pos.z);
                _lightLerps[i] = new Vector4(l.distanceLerpMin, l.distanceLerpMax, l.distanceLerpPower);
                _lightColors[i] = new Vector4(l.Color.r, l.Color.g, l.Color.b);
            }
            else
            {
                _lightPositions[i] = new Vector4(0,0,0, 0);
            }
        }

        // Send the data to all shaders globally
        Shader.SetGlobalInt(CountID, count);
        Shader.SetGlobalVectorArray(PositionsID, _lightPositions);
        Shader.SetGlobalVectorArray(LerpsID, _lightLerps);
        Shader.SetGlobalVectorArray(ColorsID, _lightColors);
    }
    

}
