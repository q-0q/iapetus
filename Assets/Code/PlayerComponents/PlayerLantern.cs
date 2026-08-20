using System;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerLantern : MonoBehaviour
{
    private GameObject _child;
    private bool _playerHasLantern;
    private Vector3 _previousPlayerPosition;
    private Vector3 _currentSpeed;

    private void Awake()
    {
        _child = transform.Find("Child").gameObject;
        _child.SetActive(false);
        _playerHasLantern = SaveSystem.GetAllItems().Contains("Lantern");
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _previousPlayerPosition = PlayerFsm.Singleton.transform.position;
        _currentSpeed = Vector3.zero;
        transform.position = PlayerFsm.Singleton.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_playerHasLantern) return;
        // float rotationSpeed = 100f;

        var playerPosition = PlayerFsm.Singleton.transform.position;
        // var playerPositionDelta = playerPosition - _previousPlayerPosition;
        
        // _currentSpeed = Vector3.Lerp(_currentSpeed, playerPositionDelta, Time.deltaTime * 4f);
        
        float posLerpSpeed = 5f;
        transform.position = Vector3.Lerp(transform.position, playerPosition, posLerpSpeed * Time.deltaTime);
        // transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // transform.position += _currentSpeed;
        
        _child.SetActive(Shader.GetGlobalFloat("_CustomDarknessWeight") > 0.75f);
        _previousPlayerPosition = playerPosition;


    }
}
