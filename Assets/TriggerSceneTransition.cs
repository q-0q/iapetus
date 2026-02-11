using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSceneTransition : MonoBehaviour
{
    public string scene;
    public Vector3 destinationPosition;
    public float destinationRotation;

    private void OnTriggerEnter(Collider other)
    {
        SaveSystem.WritePlayerInGamePosition(destinationPosition, destinationRotation, 0);
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
