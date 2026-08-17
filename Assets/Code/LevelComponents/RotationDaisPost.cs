using System;
using UnityEngine;

public class RotationDaisPost : MonoBehaviour
{
    
    private Material _material;
    private CustomPointLight _light;
    private bool _active;

    private Color _baseLightColor;

    private void Awake()
    {
        _material = GetComponent<Renderer>().material;
        _light = GetComponentInChildren<CustomPointLight>();
        _baseLightColor = _light.Color;
        _light.Color = Color.black;
        Deactivate();
    }

    public void Activate()
    {
        _active = true;
    }

    public void Deactivate()
    {
        _active = false;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var glow = _material.GetFloat("_GlowWeight");
        var color = _light.Color;

        var speed = 10f;
        _material.SetFloat("_GlowWeight", Mathf.Lerp(glow, _active ? 1f : 0f, Time.deltaTime * speed));
        _light.Color = Color.Lerp(color, _active ? _baseLightColor : Color.black, Time.deltaTime * speed);

    }
}
