using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerLine : MonoBehaviour
{
    private PowerConnector _powerConnectorA;
    private PowerConnector _powerConnectorB;
    private LineRenderer _lineRenderer;
    
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _powerConnectorA = transform.GetChild(0).GetComponent<PowerConnector>();
        _powerConnectorB = transform.GetChild(1).GetComponent<PowerConnector>();
        _powerConnectorA.AddInput(_powerConnectorB);
        _powerConnectorB.AddInput(_powerConnectorA);
    }
    
    void Update()
    {
        _lineRenderer.SetPosition(0, _powerConnectorA.transform.position);
        _lineRenderer.SetPosition(1, _powerConnectorB.transform.position);
        Color color = _powerConnectorA.IsPowered() || _powerConnectorB.IsPowered() ? Color.white : Color.gray;
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
    }
    
}
