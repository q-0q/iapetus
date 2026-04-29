using System;
using UnityEngine;
using UnityEngine.Splines;

public class FoliageMaskSpline : MonoBehaviour
{
    
    private SplineContainer _splineContainer;
    public float distance = 3f;
    public float falloff = 3f;

    private void Awake()
    {
        TryGetComponent(out _splineContainer);
    }

    public bool MaskFoliageInstance(Vector3 position)
    {
        SplineUtility.GetNearestPoint(_splineContainer.Spline, transform.InverseTransformPoint(position), out var nearest, out var _);
        var vector3 = new Vector3(nearest.x, nearest.y, nearest.z);
        vector3 = transform.TransformPoint(vector3);
        var d = Vector3.SqrMagnitude(vector3 - position);
        var p = Mathf.InverseLerp(distance + falloff, distance, d);
        return UnityEngine.Random.value < p;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnEnable() => FoliageChunkManager.MaskSplines.Add(this);
    void OnDisable() => FoliageChunkManager.MaskSplines.Remove(this);
}
