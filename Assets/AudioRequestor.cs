using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODSceneRequestor : MonoBehaviour
{
    public List<FMODSceneManager.FMODSceneEvent> StartEvents;
    public List<FMODSceneManager.FMODSceneEvent> StopEvents;
    void Start()
    {
        
        foreach (var start in StartEvents)
        {
            FMODSceneManager.Singleton.Play(start);
        }
        
        foreach (var stop in StopEvents)
        {
            FMODSceneManager.Singleton.Stop(stop);
        }
    }
}
