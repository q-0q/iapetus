using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{

    public static CutsceneManager Singleton;

    private GameObject _activeCutscene;

    private void Awake()
    {
        Singleton = this;
        _activeCutscene = null;
    }

    public void SetActiveCutscene(GameObject activeCutscene)
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
}
