using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

public class PrayerFlagRenderer : MonoBehaviour
{

    
    public Mesh Mesh;
    public Material Material;
    public float FlagSize;
    public float FlagSpacing;
    public float EndSpacing = 4f;

    private Matrix4x4[] matrices;
    private int _flagCount;
    RenderParams rp;

    private SplineContainer _splineContainer;
    private MaterialPropertyBlock _propBlock;
    private float[] _splineProgressArray;
    public List<Color> palette = new List<Color>();
    private Vector4[] _flagColors; // Use Vector4 for shader-friendly color data
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rp = new RenderParams(Material)
        {
            shadowCastingMode = ShadowCastingMode.On,
            receiveShadows = true
        };

        TryGetComponent(out _splineContainer);
        var curveLength = _splineContainer.Spline.GetLength();
        GetComponent<MeshRenderer>().material.SetFloat("_SplineLength", curveLength);
        _flagCount = Mathf.FloorToInt((curveLength - EndSpacing * 2f) / FlagSpacing);
        _splineProgressArray = new float[_flagCount];
        matrices = new Matrix4x4[_flagCount];
        _propBlock = new MaterialPropertyBlock();
        
        _flagColors = new Vector4[_flagCount];

        // Pick a random color for each flag once at the start
        for (int i = 0; i < _flagCount; i++)
        {
            Color randomColor = palette[UnityEngine.Random.Range(0, palette.Count)];
            _flagColors[i] = (Vector4)randomColor; 
        }
    }

    void Update()
    {
        var spline = _splineContainer.Spline;
        float len = spline.GetLength();

        for (int i = 0; i < _flagCount; i++)
        {
            float t = ((i * FlagSpacing + EndSpacing) / len);
            _splineProgressArray[i] = t;

            // 1. Evaluate the full transform (Position and Rotation) at progress 't'
            // This returns a float4x4 in Local Space
            spline.Evaluate(t, out float3 localPos, out float3 tangent, out float3 upVector);
        
            // 2. Create a Rotation from the Tangent and Up vector
            // math.lookRotation(forward, up)
            Quaternion localRotation = Quaternion.LookRotation((Vector3)tangent, (Vector3)upVector);
            
            // 3. Convert to World Space
            Vector3 worldPos = transform.TransformPoint((Vector3)localPos);
            // Multiply parent rotation by local spline rotation
            Quaternion worldRot = transform.rotation * (Quaternion)localRotation;

            _propBlock.SetVectorArray("_BaseColor", _flagColors);
            
            // 4. Update the matrix
            matrices[i] = Matrix4x4.TRS(worldPos, worldRot, Vector3.one);
        }
    
        _propBlock.SetFloatArray("_SplineProgress", _splineProgressArray);
        rp.matProps = _propBlock;
        Graphics.RenderMeshInstanced(rp, Mesh, 0, matrices, _flagCount);
    }
}
