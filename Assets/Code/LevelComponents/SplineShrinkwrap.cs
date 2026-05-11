using System;
using UnityEngine;using UnityEngine.Splines;

public class SplineShrinkwrap : MonoBehaviour
{
    public SplineContainer target;
    public float pointsPerUnit = 5f;
    private SplineContainer _splineContainer;
    
    
    private void Awake()
    {
        TryGetComponent(out _splineContainer);
        var spline = _splineContainer.Spline;
        float length = spline.GetLength();
        // float pointDistance = 
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
