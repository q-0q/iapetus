using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;


public class MinorLeyline : MonoBehaviour
{

    private Transform _nodeA;
    private Transform _nodeB;

    private SplineContainer _splineContainer;
    private float _length;
    
    private Material _material;

    private void Awake()
    {
        _nodeA = transform.Find("MinorLeylineNodeA");
        _nodeB = transform.Find("MinorLeylineNodeB");
        _splineContainer = GetComponentInChildren<SplineContainer>();
        _length = _splineContainer.Spline.GetLength();
        _material = _splineContainer.GetComponent<MeshRenderer>().material;
        var curveLength = _splineContainer.Spline.GetLength();
        _material.SetFloat("_SplineLength", curveLength);
        AlignNodes();
    }
    

    private void AlignNodes()
    {
        var splineContainer = GetComponentInChildren<SplineContainer>();
        var spline = splineContainer.Spline;
    
        var a = transform.Find("MinorLeylineNodeA");
        var b = transform.Find("MinorLeylineNodeB");
        
        a.rotation = GetWorldRotationAt(0f);
        a.position = splineContainer.transform.TransformPoint(spline.EvaluatePosition(0f));

        b.rotation = GetWorldRotationAt(1f) * Quaternion.Euler(0, 180f, 0);
        b.position = splineContainer.transform.TransformPoint(spline.EvaluatePosition(1f));
    }

    public Quaternion GetWorldRotationAt(float t)
    {
        var container = GetComponentInChildren<SplineContainer>();
        container.Evaluate(t, out float3 position, out float3 tangent, out float3 upVector);
        return Quaternion.LookRotation(tangent, upVector);
    }
    
    public Quaternion GetPlayerRotationAt(float t, bool direction, out float upMod)
    {
        upMod = 0;
        _splineContainer.Evaluate(t, out float3 position, out float3 tangent, out float3 upVector);
        var worldRotation = Quaternion.LookRotation(tangent, upVector);

        var v = tangent;
        var upAngle = Vector3.Angle(Vector3.up, direction ? tangent : -tangent );
        if (upAngle < 40f)
        {
            v = upVector;
            upMod = 1f;
        }
        
        return Quaternion.LookRotation(Vector3.ProjectOnPlane(v, Vector3.up), Vector3.up)
                                  * Quaternion.Euler(0, direction ? 0f : 180f, 0f);
    }
    
    
    public Vector3 GetUpVectorAt(float t)
    {
        _splineContainer.Evaluate(t, out float3 position, out float3 tangent, out float3 upVector);
        return upVector;
    }

    private void OnDrawGizmos()
    {
        AlignNodes();
    }

    public bool GetDirectionFromTrigger(Transform trigger)
    {
        return trigger.parent == _nodeA;
    }

    public Vector3 EvaluatePosition(float t)
    {
        return _splineContainer.transform.TransformPoint(_splineContainer.Spline.EvaluatePosition(t));
    }

    public void SetShaderWeight(float weight)
    {
        _material.SetFloat("_FillWeight", weight);
    }

    public float Length()
    {
        return _length;
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
