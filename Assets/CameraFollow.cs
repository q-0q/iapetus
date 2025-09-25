using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    private float YLerpRate = 1.75f;
    
    private void OnEnable()
    {
        PlayerFsm.OnPlayerPositionUpdated += UpdatePosition;
    }

    private void OnDisable()
    {
        PlayerFsm.OnPlayerPositionUpdated -= UpdatePosition;
    }
    
    void UpdatePosition(Vector3 pos, bool grounded)
    {
        var newY = grounded ? Mathf.Lerp(transform.position.y, pos.y, Time.deltaTime * YLerpRate) : transform.position.y;
        transform.position = new Vector3(pos.x, newY, pos.z);
    }
}
