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
    
    // Start is called before the first frame update
    void Start()
    {
        _initialFogStartDistance = RenderSettings.fogStartDistance;
        _initialFogEndDistance = RenderSettings.fogEndDistance;
    }

    // Update is called once per frame
    void Update()
    {
        var height = Vector3.Dot(PlayerFsm.Singleton.transform.position, axis);
        var weight = Mathf.InverseLerp(startCoordinate, endCoordinate, height);

        RenderSettings.fogStartDistance = _initialFogStartDistance + Mathf.Lerp(0f, fogStartOffset, weight);
        RenderSettings.fogEndDistance = _initialFogEndDistance + Mathf.Lerp(0f, fogEndOffset, weight);
    }
}
