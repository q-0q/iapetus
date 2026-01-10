using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelSceneTransition : MonoBehaviour
{
    public string scene;
    public Vector3 destinationPosition;
    public float destinationRotation;
    
    private void OnTriggerEnter(Collider other)
    {
        SceneLoader.Singleton.LoadScene(scene);
    }
}
