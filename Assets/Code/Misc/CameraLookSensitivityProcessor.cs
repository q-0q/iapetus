using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad]
#endif
public class CameraLookSensitivityProcessor : InputProcessor<Vector2>
{
#if UNITY_EDITOR
    static CameraLookSensitivityProcessor()
    {
        Initialize();
    }
#endif
    
    private static float _sensitivityModifier;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        InputSystem.RegisterProcessor<CameraLookSensitivityProcessor>();
    }

    public static void SetSensitivityModifier(float value)
    {
        _sensitivityModifier = value;
    }
    
    public override Vector2 Process(Vector2 value, InputControl control)
    {
        Debug.Log(_sensitivityModifier);
        return value * _sensitivityModifier;
    }
}