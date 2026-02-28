using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODSceneRequestor : MonoBehaviour
{
    [Serializable]
    public class StartRequest
    {
        public FMODSceneManager.FMODSceneEvent FMODSceneEvent;
        public float minimumPlayerY = -1000f;

    }
    public List<StartRequest> StartEvents;
    public List<FMODSceneManager.FMODSceneEvent> StopEvents;
    void Update()
    {
        
        foreach (var start in StartEvents)
        {
            if (PlayerFsm.Singleton.transform.position.y < start.minimumPlayerY) continue;
            FMODSceneManager.Singleton.Play(start.FMODSceneEvent);
        }
        
        foreach (var stop in StopEvents)
        {
            FMODSceneManager.Singleton.Stop(stop);
        }
    }
}
