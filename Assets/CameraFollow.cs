using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    
    
    private void OnEnable()
    {
        PlayerFsm.OnPlayerPositionUpdated += UpdatePosition;
    }

    private void OnDisable()
    {
        PlayerFsm.OnPlayerPositionUpdated -= UpdatePosition;
    }
    
    void UpdatePosition(Vector3 pos)
    {
        transform.position = new Vector3(pos.x, transform.position.y, pos.z);
    }
}
