using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;


public class MinorLeyline : MonoBehaviour
{

    private Transform _nodeA;
    private Transform _nodeB;

    private void Awake()
    {
        _nodeA = transform.Find("MinorLeylineNodeA");
        _nodeB = transform.Find("MinorLeylineNodeB");
        AlignNodes();
    }
    

    private void AlignNodes()
    {
        var splineContainer = GetComponentInChildren<SplineContainer>();
        var spline = splineContainer.Spline;
    
        var a = transform.Find("MinorLeylineNodeA");
        var b = transform.Find("MinorLeylineNodeB");
        
        a.rotation = GetWorldRotationAt(splineContainer, 0f);
        a.position = splineContainer.transform.TransformPoint(spline.EvaluatePosition(0f));

        b.rotation = GetWorldRotationAt(splineContainer, 1f) * Quaternion.Euler(0, 180f, 0);
        b.position = splineContainer.transform.TransformPoint(spline.EvaluatePosition(1f));
    }

    private Quaternion GetWorldRotationAt(SplineContainer container, float t)
    {
        container.Evaluate(t, out float3 position, out float3 tangent, out float3 upVector);
        return Quaternion.LookRotation(tangent, upVector);
    }

    private void OnDrawGizmos()
    {
        AlignNodes();
    }

    // public bool TrySetPlayerDirection(Transform trigger)
    // {
    //     
    // }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
