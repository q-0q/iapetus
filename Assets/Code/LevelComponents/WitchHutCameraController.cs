using System;
using Cinemachine;
using Code.Misc;
using UnityEngine;

public class WitchHutCameraController : MonoBehaviour
{
    private CinemachineTrackedDolly _dolly;
    public Transform center;

    private void Awake()
    {
        _dolly = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var w = Mathf.InverseLerp(18f, 7f, Vector3.Distance(PlayerFsm.Singleton.transform.position, center.position));
        w = Util.SmoothLerp01(w);
        _dolly.m_PathPosition = w;
    }
}
