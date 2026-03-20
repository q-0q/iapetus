using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = FMOD.Debug;

public class TriggerSceneTransition : MonoBehaviour
{
    public string scene;
    public float destinationRotation;
    
    public string DestinationId;

    private void OnTriggerEnter(Collider other)
    {
        SaveSystem.WritePlayerInGamePosition(Vector3.zero, DestinationId, destinationRotation);
        SceneLoader.Singleton.LoadScene(scene);
    }

    
    // private void Start()
    // {
    //     _initialFogEndDistance = RenderSettings.fogEndDistance;
    //     _initialFogStartDistance = RenderSettings.fogStartDistance;
    // }
    //
    // private void Update()
    // {
    //     var distance = Vector3.Distance(transform.position, PlayerFsm.Singleton.transform.position);
    //
    //     var fogDistanceOffset = Mathf.Lerp(30f, 0f,
    //         Mathf.InverseLerp(20f, 50f, distance));
    //
    //     RenderSettings.fogStartDistance = _initialFogStartDistance + fogDistanceOffset;
    //     RenderSettings.fogEndDistance = _initialFogEndDistance + fogDistanceOffset;
    // }
}
