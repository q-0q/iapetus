using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    private float YLerpRate = 2.75f;
    private Vector3 _playerPos;
    private Vector3 _playerWeaponPos;
    
    private void OnEnable()
    {
        PlayerFsm.OnPlayerPositionUpdated += UpdatePlayerPosition;
        PlayerWeaponFsm.OnPlayerWeaponPositionUpdated += UpdatePlayerWeaponPosition;
    }

    private void OnDisable()
    {
        PlayerFsm.OnPlayerPositionUpdated -= UpdatePlayerPosition;
        PlayerWeaponFsm.OnPlayerWeaponPositionUpdated -= UpdatePlayerWeaponPosition;
    }
    
    void UpdatePlayerPosition(Vector3 pos, bool grounded)
    {
        pos = CameraFollowTarget.Singleton.transform.position;
        var newY = true ? Mathf.Lerp(transform.position.y, pos.y, Time.deltaTime * YLerpRate) : transform.position.y;
        _playerPos = new Vector3(pos.x, newY, pos.z);
    }
    
    void UpdatePlayerWeaponPosition(Vector3 pos, bool active)
    {
        _playerWeaponPos = Vector3.Lerp(_playerWeaponPos, active ? pos : _playerPos, Time.deltaTime * 5f);
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(_playerPos, _playerWeaponPos, 0.35f);
    }
}
