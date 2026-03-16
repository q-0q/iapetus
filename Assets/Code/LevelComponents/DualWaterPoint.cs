using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualWaterPoint : MonoBehaviour
{

    public float Radius = 30f;

    private void OnEnable()
    {
        DualWaterPointRegistry.DualWaterPoints.Add(this);
    }

    private void OnDisable()
    {
        DualWaterPointRegistry.DualWaterPoints.Remove(this);
    }
}
