using System;
using System.Collections.Generic;
using Code.Misc;
using UnityEngine;
using UnityEngine.Splines;

public class GlyphController : MonoBehaviour
{
    
    public static readonly List<TerminalNode> TerminalsInScene = new();
    private List<GameObject> _splines;
    
    
    public class TerminalData
    {
        public string previousNode = "";
        public List<string> loreDialogue = new List<string>() { };
        public string displayId;

        public int mapSplineId;
        public float mapSplineStartT;
        public float mapSplineEndT;

    }

    public static readonly Dictionary<string, TerminalData> TerminalRegistry = new()
    {
        
        { "bootstrap", new TerminalData()
            {
                displayId = "",
                previousNode = "",
                loreDialogue = new List<string>() { },
                
                mapSplineId = 0,
                mapSplineStartT = 0f,
                mapSplineEndT = 0.2f
            } 
        },
        
        
        { "tutorial-0", new TerminalData()
            {
                displayId = "00",
                previousNode = "bootstrap",
                loreDialogue = new List<string>() { "Signal here is drawn directly from source, making this the inception of the entire system.", "...As such, this node is a potential point of failure for entire network. Uptime is critical.", "Hardware health of this station should be monitored especially closely." },
                
                mapSplineId = 0,
                mapSplineStartT = 0.2f,
                mapSplineEndT = 0.4f
            } 
        },
        
        { "tutorial-1", new TerminalData()
            {
                displayId = "01",
                previousNode = "tutorial-0",
                loreDialogue = new List<string>() { "01 propagates signal into and through the master chambers.", "Signal density has to be within a very specific range to avoid hardware interference where the line passes indoors.", "That increased workload means this machine may require more regular defrags." },
                
                mapSplineId = 0,
                mapSplineStartT = 0.4f,
                mapSplineEndT = 0.6f
            } 
        },
        
        { "icy-canals-0", new TerminalData()
            {
                displayId = "02",
                previousNode = "tutorial-1",
                loreDialogue = new List<string>() {  },
                mapSplineId = 0,
                mapSplineStartT = 0.6f,
                mapSplineEndT = 0.8f
            } 
        },
        
        { "piton-climb", new TerminalData()
            {
                displayId = "03",
                previousNode = "icy-canals",
                loreDialogue = new List<string>() { }
            } 
        }
    };

    private void Awake()
    {
        OnSaveDataUpdated(SaveSystem.LoadCachedSaveData());
        var find = transform.Find("Canvas");
        
        var s = find.Find("Splines");
        _splines = new List<GameObject>();
        _splines.Add(s.Find("0").gameObject);
        // _splines.Add(s.Find("1").gameObject);
        // ...

        
        foreach (var spline in _splines)
        {
            
            var splineContainer = spline.GetComponent<SplineContainer>();
            var curveLength = splineContainer.Spline.GetLength();
            spline.GetComponent<Renderer>().material.SetFloat("_SplineLength", curveLength);
        }
        
        
        UpdateSplineRenderers();

    }

    private void UpdateSplineRenderers()
    {


        for (int i = 0; i < _splines.Count; i++)
        {
            var largestEndT = -1f;
            foreach (var (terminalName, terminalData) in TerminalRegistry)
            {
                if (terminalData.mapSplineId != i) continue;
                if (!SaveSystem.GetTerminalNode(terminalName)) continue;
                if (terminalData.mapSplineEndT < largestEndT) continue;
                largestEndT = terminalData.mapSplineEndT;
            }
            
            _splines[i].GetComponent<Renderer>().material.SetFloat("_FillWeight", largestEndT);
        }
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
    }
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<GameObject> GetSplines()
    {
        return _splines;
    }
}
