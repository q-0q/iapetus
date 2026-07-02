using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class GlyphManager : MonoBehaviour
{
    
    public static readonly List<TerminalNode> MajorLeylineNodes = new();
    public static GlyphManager Singleton;
    public string a = "summit-glyph";
    public string b = "test-b";
    public string c = "test-c";

    
    public class TerminalData
    {
        public string previousNode = "";
        public List<string> loreDialogue = new List<string>() { };

    }

    public static readonly Dictionary<string, TerminalData> TerminalRegistry = new()
    {
        { "tutorial-0", new TerminalData()
            {
                previousNode = "",
                loreDialogue = new List<string>() { "First node. Leyline signal is drawn directly from source.", "...As such, node is a potential point of failure for entire network. Uptime is critical." }
            } 
        },
        
        { "tutorial-1", new TerminalData()
            {
                previousNode = "tutorial-0",
                loreDialogue = new List<string>() { "Lore test A.", "Lore test B!" }
            } 
        },
        
        { "icy-canals", new TerminalData()
            {
                previousNode = "tutorial-1",
                loreDialogue = new List<string>() {  }
            } 
        },
        
        { "piton-climb", new TerminalData()
            {
                previousNode = "icy-canals",
                loreDialogue = new List<string>() { }
            } 
        }
    };

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
        if (saveData.terminalNodes == null) return;
        Shader.SetGlobalFloat("_GlyphMaskWeight_A", saveData.terminalNodes.Contains(a) ? 1f : 0f);
        Shader.SetGlobalFloat("_GlyphMaskWeight_B", saveData.terminalNodes.Contains(b) ? 1f : 0f);
        Shader.SetGlobalFloat("_GlyphMaskWeight_C", saveData.terminalNodes.Contains(c) ? 1f : 0f);
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
                nearestT = t * majorLeylineNode.mapSplineTMultiplier;
            }
        }
        
        SaveSystem.WriteNearestTerminalNode(nearestNode, nearestT);
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
