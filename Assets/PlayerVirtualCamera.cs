using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerVirtualCamera : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private float _momentumStart = 0f;
    [SerializeField] private float _momentumEnd = 8f;
    [SerializeField] private float _minDistance = 15f;
    [SerializeField] private float _maxDistance = 25f;
    [SerializeField] private float _lerpSpeed = 15f;
    
    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out _virtualCamera);
    }

    void UpdateText(float momentum)
    {
        var val = Mathf.InverseLerp(_momentumStart, _momentumEnd, momentum);
        var desiredDistance = Mathf.Lerp(_minDistance, _maxDistance, val);
        var cinemachineFramingTransposer = ((CinemachineFramingTransposer)_virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body));
        var currentDistance =
            cinemachineFramingTransposer.m_CameraDistance;
        cinemachineFramingTransposer.m_CameraDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * _lerpSpeed);
    }

    private void OnEnable()
    {
        PlayerFsm.OnPlayerMomentumUpdated += UpdateText;
    }

    private void OnDisable()
    {
        PlayerFsm.OnPlayerMomentumUpdated -= UpdateText;
    }
}
