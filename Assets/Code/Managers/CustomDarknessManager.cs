using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CustomDarknessManager : MonoBehaviour
{
    public static CustomDarknessManager Singleton;

    private void Awake()
    {
        Singleton = this;
    }
    
    private CustomDarknessController _currentController;
    public static readonly List<CustomDarknessController> CustomDarknessControllerRegistry = new();
    public static readonly List<CustomDarknessObserver> CustomDarknessObserverRegistry = new();
    public const float LerpStrength = 1f;


    public void SetCurrentController(CustomDarknessController controller, bool snap = false)
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
        CustomDarknessController _currentHighestPriorityController = null;

        foreach (var collider in colliders)
        {
            if (collider == null) continue;
            collider.TryGetComponent(out CustomDarknessController controller);
            if (controller == null) continue;
            if (_currentHighestPriorityController != null && _currentHighestPriorityController.Priority >= controller.Priority) continue;
            _currentHighestPriorityController = controller;
            SetCurrentController(controller, snap);
        }
        
        if(_currentHighestPriorityController == null) SetCurrentController(null, snap);
    }

    // Update is called once per frame
    public void Update()
    {
        if (!Application.isPlaying)
        {
            ApplyEditorSettings();
            return;
        }
        
        UpdateObserversVectorArray();
        UpdateCurrentController(false);
        LerpAllValues(LerpStrength);
    }

    private void LerpAllValues(float strength)
    {
        LerpFloat("_CustomDarknessWeight", _currentController == null ? 0f : 1f, strength);
        if (_currentController != null) LerpColor("_CustomDarknessColor", _currentController.Color, strength);
    }

    private static void ApplyEditorSettings()
    {
        Shader.SetGlobalFloat("_CustomDarknessWeight", 0f);
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
    
    private static Vector4[] _observerPositions = new Vector4[64];
    private static readonly int PointsID = Shader.PropertyToID("_CustomDarknessObservers");
    private static readonly int CountID = Shader.PropertyToID("_CustomDarknessObserverCount");
    
    private void UpdateObserversVectorArray()
    {
        int count = Mathf.Min(CustomDarknessObserverRegistry.Count, 64);
        
        for (int i = 0; i < 64; i++)
        {
            if (i < count)
            {
                Vector3 pos = CustomDarknessObserverRegistry[i].transform.position;
                _observerPositions[i] = new Vector4(pos.x, pos.y, pos.z, CustomDarknessObserverRegistry[i].radiusMultiplier);
            }
            else
            {
                _observerPositions[i] = new Vector4(0,0,0, 0);
            }
        }
        
        Shader.SetGlobalVectorArray(PointsID, _observerPositions);
        Shader.SetGlobalInt(CountID, count);
    }
    

}
