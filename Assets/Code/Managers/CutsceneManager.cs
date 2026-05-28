using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{

    public static CutsceneManager Singleton;

    private CutsceneFsm _activeCutscene;
    private bool _pseudoCutsceneActive;
    public bool _overwriteCameraFollowShaderPosition;
    public Transform _cameraFollowShaderTransform;

    public static event Action OnOverwriteCameraFollowEnded;

    private void Awake()
    {
        Singleton = this;
        _activeCutscene = null;
        _pseudoCutsceneActive = false;
    }

    public void SetActiveCutscene(CutsceneFsm activeCutscene)
    {
        _activeCutscene = activeCutscene;
    }
    
    public void SetPseudoCutsceneActive(bool overwriteCameraFollowShaderPosition = false, Transform cameraFollowShaderTransform = null)
    {
        _pseudoCutsceneActive = true;
        _overwriteCameraFollowShaderPosition = overwriteCameraFollowShaderPosition;
        _cameraFollowShaderTransform = cameraFollowShaderTransform;
        if (cameraFollowShaderTransform != null)
        {
            Shader.SetGlobalVector("_CameraFollowWorldPosition", cameraFollowShaderTransform.position);
        }
    }
    
    public void ClearPseudoCutsceneActive()
    {
        _pseudoCutsceneActive = false;
        if (_overwriteCameraFollowShaderPosition) OnOverwriteCameraFollowEnded?.Invoke();
        _overwriteCameraFollowShaderPosition = false;
    }


    public void ClearActiveCutscene()
    {
        _activeCutscene = null;
    }

    public bool IsCutsceneActive()
    {
        return _pseudoCutsceneActive || _activeCutscene != null;
    }

    public bool IsCutscenePlayerDisabled()
    {
        if (_pseudoCutsceneActive) return true;
        if (_activeCutscene == null) return false;
        return _activeCutscene.StateMapConfig.CutscenePlayerDisabled.Get(_activeCutscene);
    }
    
    public bool IsCutsceneCameraDisabled()
    {
        if (_pseudoCutsceneActive) return true;
        if (_activeCutscene == null) return false;
        return _activeCutscene.StateMapConfig.CutsceneCameraDisabled.Get(_activeCutscene);
    }
    
    public bool IsCutsceneJumpDisabled()
    {
        if (_pseudoCutsceneActive) return true;
        if (_activeCutscene == null) return false;
        return _activeCutscene.StateMapConfig.CutsceneJumpDisabled.Get(_activeCutscene);
    }
    
    public bool IsCutsceneHardLand()
    {
        if (_pseudoCutsceneActive) return true;
        if (_activeCutscene == null) return false;
        return _activeCutscene.StateMapConfig.CutsceneHardLand.Get(_activeCutscene);
    }

    public bool IsCutsceneOverwriteCameraFollowShaderPosition(out Transform transform)
    {
        transform = _cameraFollowShaderTransform;
        return _overwriteCameraFollowShaderPosition;
    }
    
}
