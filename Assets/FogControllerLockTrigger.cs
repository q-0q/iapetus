using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogControllerLockTrigger : MonoBehaviour
{
    private FogController _fogController;
    public float LockHeight = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        _fogController = FindObjectOfType<FogController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _fogController.LockHeight(LockHeight);
    }

    private void OnTriggerExit(Collider other)
    {
        _fogController.Unlock();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
