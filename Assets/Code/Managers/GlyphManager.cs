using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class GlyphManager : MonoBehaviour
{
    
    public static readonly List<MajorLeylineNode> MajorLeylineNodes = new();
    public static GlyphManager Singleton;
    public string a = "summit-glyph";
    public string b = "test-b";
    public string c = "test-c";

    private void Awake()
    {
        Singleton = this;
        OnSaveDataUpdated(SaveSystem.LoadCachedSaveData());
    }

    private void OnEnable()
    {
        SaveSystem.OnSaveDataUpdated += OnSaveDataUpdated;
    }

    private void OnDisable()
    {
        SaveSystem.OnSaveDataUpdated -= OnSaveDataUpdated;
    }

    private void OnSaveDataUpdated(SaveSystem.SaveData saveData)
    {
        if (saveData.majorLeylineNodes == null) return;
        Shader.SetGlobalFloat("_GlyphMaskWeight_A", saveData.majorLeylineNodes.Contains(a) ? 1f : 0f);
        Shader.SetGlobalFloat("_GlyphMaskWeight_B", saveData.majorLeylineNodes.Contains(b) ? 1f : 0f);
        Shader.SetGlobalFloat("_GlyphMaskWeight_C", saveData.majorLeylineNodes.Contains(c) ? 1f : 0f);
    }

    public void ComputePlayerMapPosition()
    {
        if (MajorLeylineNodes.Count == 0) return;

        var playerPosition = PlayerFsm.Singleton.transform.position;
        
        var nearestDistance = Mathf.Infinity;
        var nearestNode = "";
        var nearestT = 0f;

        foreach (var majorLeylineNode in MajorLeylineNodes)
        {
            print(majorLeylineNode.metaName);
            var visualSpline = majorLeylineNode.GetVisualSplineContaier();
            SplineUtility.GetNearestPoint(visualSpline.Spline, visualSpline.transform.InverseTransformPoint(playerPosition), out var nearest, out var t);
            var vector3 = new Vector3(nearest.x, nearest.y, nearest.z);
            vector3 = visualSpline.transform.TransformPoint(vector3);
            var d = Vector3.SqrMagnitude(vector3 - playerPosition);

            if (d < nearestDistance)
            {
                nearestDistance = d;
                nearestNode = majorLeylineNode.metaName;
                nearestT = t;
            }
        }
        
        SaveSystem.WriteNearestMajorLeylineNode(nearestNode, nearestT);
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
