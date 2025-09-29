using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HitstopManager : MonoBehaviour
{
    public static HitstopManager Singleton;
    
    private float _remainingHitstop;


    private void Awake()
    {
        Singleton = this;
    }

    public void StartHitstop(float amount)
    {
        _remainingHitstop = Mathf.Max(_remainingHitstop, amount);
    }

    public bool IsHitstopActive()
    {
        return _remainingHitstop > 0f;
    }

    // Update is called once per frame
    void Update()
    {
        _remainingHitstop -= Time.deltaTime;
    }
}
