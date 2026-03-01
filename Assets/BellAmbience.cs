using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Random = UnityEngine.Random;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class BellAmbience : MonoBehaviour
{

    public EventReference EventReference;

    private EventInstance _eventInstance;

    private void OnEnable()
    {
        _eventInstance = FMODUnity.RuntimeManager.CreateInstance(EventReference);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(_eventInstance, gameObject);
     
    }

    private void OnDisable()
    {
        _eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }

    // Start is called before the first frame update
    void Start()
    {
        _eventInstance.start();
        _eventInstance.setTimelinePosition(Random.Range(0, 5000));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
