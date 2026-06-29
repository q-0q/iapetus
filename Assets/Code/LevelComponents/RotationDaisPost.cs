using System;
using UnityEngine;

public class RotationDaisPost : MonoBehaviour
{
    
    private Material _material;
    private Light _light;
    private bool _active;

    private void Awake()
    {
        _material = GetComponent<Renderer>().material;
        _light = GetComponentInChildren<Light>();
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
        var c = _material.GetColor("_ColorC");
        var i = _light.intensity;

        var speed = 10f;
        _material.SetColor("_ColorC", Color.Lerp(c, _active ? new Color(0.5254902f, 0.6274511f, 1f) : new Color(0.2f, 0.2f, 0.2f), Time.deltaTime * speed));
        _light.intensity = Mathf.Lerp(i, _active ? 1.5f : 0, Time.deltaTime * speed);

    }
}
