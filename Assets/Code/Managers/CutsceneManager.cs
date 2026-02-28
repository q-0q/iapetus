using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{

    public static CutsceneManager Singleton;

    private CutsceneFsm _activeCutscene;
    private bool _pseudoCutsceneActive;

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
    
    public void SetPseudoCutsceneActive()
    {
        _pseudoCutsceneActive = true;
    }
    
    public void ClearPseudoCutsceneActive()
    {
        _pseudoCutsceneActive = false;
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
}
