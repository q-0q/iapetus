using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachineFreeLookHelper : MonoBehaviour
{
    private CinemachineFreeLook _freeLook;
    private float _baseXSpeed;
    private float _baseYSpeed;

    void Awake()
    {
        TryGetComponent(out _freeLook);
        _baseXSpeed = _freeLook.m_XAxis.m_MaxSpeed;
        _baseYSpeed = _freeLook.m_YAxis.m_MaxSpeed;
    }

    private void OnEnable()
    {
        MetaSaveSystem.OnMetaSaveDataUpdated += OnMetaSaveDataUpdated;
    }

    private void OnDisable()
    {
        MetaSaveSystem.OnMetaSaveDataUpdated -= OnMetaSaveDataUpdated;
    }
    
    private void OnMetaSaveDataUpdated(MetaSaveSystem.MetaSaveData metaSaveData)
    {
        _freeLook.m_XAxis.m_MaxSpeed = _baseXSpeed * metaSaveData.cameraSensitivityModifier;
        _freeLook.m_YAxis.m_MaxSpeed = _baseYSpeed * metaSaveData.cameraSensitivityModifier;
    }
}
