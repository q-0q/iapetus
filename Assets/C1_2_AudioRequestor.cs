using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C1_2_AudioRequestor : MonoBehaviour
{
    void Start()
    {
        FMODSceneManager.Singleton.Stop(FMODSceneManager.FMODSceneEvent.WindAmbience);
        FMODSceneManager.Singleton.Play(FMODSceneManager.FMODSceneEvent.Ch1Music);
    }
}
