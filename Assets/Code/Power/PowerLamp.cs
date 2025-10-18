using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerLamp : MonoBehaviour
{
    private List<Light> _lights;
    private Material _material;
    private bool _lamp;
    private const float LerpStrength = 5f;
    private static readonly Color OnColor = new Color(1f, 1f, 1f, 1f);
    private static readonly Color OffColor = new Color(1f, 0f, 0f, 0.5f);

    private void Awake()
    {
        _lights = GetComponentsInChildren<Light>().ToList();
        _material = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        var color = Color.Lerp(_material.GetColor("_Color"), _lamp ? OnColor : OffColor, Time.deltaTime * LerpStrength);
        _material.color = color;
        // _material.SetColor("_BaseColor", color);
        foreach (var light in _lights)
        {
            light.intensity = Mathf.Lerp(light.intensity, _lamp ? 1f : 0f, Time.deltaTime * LerpStrength);
        }
    }

    public void SetLamp(bool val)
    {
        _lamp = val;
    }
}
