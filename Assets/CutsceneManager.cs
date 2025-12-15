using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{

    public static CutsceneManager Singleton;

    private CutsceneFsm _activeCutscene;

    private void Awake()
    {
        Singleton = this;
        _activeCutscene = null;
    }

    public void SetActiveCutscene(CutsceneFsm activeCutscene)
    {
        _activeCutscene = activeCutscene;
    }

    public void ClearActiveCutscene()
    {
        _activeCutscene = null;
    }

    public bool IsCutsceneActive()
    {
        return _activeCutscene != null;
    }

    public bool IsCutscenePlayerDisabled()
    {
        if (_activeCutscene == null) return false;
        return _activeCutscene.StateMapConfig.CutscenePlayerDisabled.Get(_activeCutscene);
    }
}
