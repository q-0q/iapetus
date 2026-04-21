using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogController : MonoBehaviour
{

    private float _initialFogStartDistance;
    private float _initialFogEndDistance;

    public float fogStartOffset;
    public float fogEndOffset;
    
    public Vector3 axis = Vector3.up;
    public float startCoordinate;
    public float endCoordinate;

    private float _weight = 0;

    private bool locked = false;
    private float lockedHeight = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        _initialFogStartDistance = RenderSettings.fogStartDistance;
        _initialFogEndDistance = RenderSettings.fogEndDistance;
        
        var height = Vector3.Dot(PlayerFsm.Singleton.transform.position, axis);
        _weight = Mathf.InverseLerp(startCoordinate, endCoordinate, height);
    }

    // Update is called once per frame
    void Update()
    {
        var height = locked ? lockedHeight : Vector3.Dot(PlayerFsm.Singleton.transform.position, axis);
        _weight = Mathf.Lerp(_weight, Mathf.InverseLerp(startCoordinate, endCoordinate, height), Time.deltaTime * 0.5f);

        RenderSettings.fogStartDistance = _initialFogStartDistance + Mathf.Lerp(0f, fogStartOffset, _weight);
        RenderSettings.fogEndDistance = _initialFogEndDistance + Mathf.Lerp(0f, fogEndOffset, _weight);
    }

    public void LockHeight(float y)
    {
        lockedHeight = y;
        locked = true;
    }

    public void Unlock()
    {
        locked = false;
    }
}
