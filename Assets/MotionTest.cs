using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionTest : MonoBehaviour
{
    private Vector3 _base;

    private void Start()
    {
        _base = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var f = Mathf.Sin(Time.time * 4f) * 1f;
        transform.position = _base + new Vector3(f, f, f);
    }
}
