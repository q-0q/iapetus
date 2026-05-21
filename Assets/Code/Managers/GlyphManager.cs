using System;
using UnityEngine;

public class GlyphManager : MonoBehaviour
{
    private static GlyphManager Singleton;
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
        Shader.SetGlobalFloat("_GlyphMaskWeight_A", saveData.majorLeylineNodes.Contains(a) ? 1f : 0f);
        Shader.SetGlobalFloat("_GlyphMaskWeight_B", saveData.majorLeylineNodes.Contains(b) ? 1f : 0f);
        Shader.SetGlobalFloat("_GlyphMaskWeight_C", saveData.majorLeylineNodes.Contains(c) ? 1f : 0f);
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
