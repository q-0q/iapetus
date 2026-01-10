using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class FMODSceneManager : MonoBehaviour
{
    public enum FMODSceneEvent
    {
        Ch1Music,
        WindAmbience,
    }
    
    private static Dictionary<FMODSceneEvent, EventInstance> _eventInstances;
    
    private static FMODSceneManager _singleton;
    public static FMODSceneManager Singleton
    {
        get
        {
            if (_singleton == null)
            {
                var go = new GameObject("FMODSceneManager");
                _singleton = go.AddComponent<FMODSceneManager>();
            }
            return _singleton;
        }
    }

    void Awake()
    {
        if (_singleton != null && _singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        _singleton = this;
        DontDestroyOnLoad(gameObject);

        _eventInstances = new Dictionary<FMODSceneEvent, EventInstance>
        {
            [FMODSceneEvent.Ch1Music] = RuntimeManager.CreateInstance(EventReference.Find("event:/CH1_Music")),
            [FMODSceneEvent.WindAmbience] = RuntimeManager.CreateInstance(EventReference.Find("event:/WindAmbience"))
        };
    }

    public void Play(FMODSceneEvent fmodSceneEvent)
    {
        if (PlaybackState(_eventInstances[fmodSceneEvent]) == PLAYBACK_STATE.PLAYING) return;
        _eventInstances[fmodSceneEvent].start();
    }
    
    public void Stop(FMODSceneEvent fmodSceneEvent)
    {
        _eventInstances[fmodSceneEvent].stop(STOP_MODE.ALLOWFADEOUT);
    }
    
    
    
    FMOD.Studio.PLAYBACK_STATE PlaybackState(FMOD.Studio.EventInstance instance) 
    {
        FMOD.Studio.PLAYBACK_STATE pS;
        instance.getPlaybackState(out pS);
        return pS;
    }
}


