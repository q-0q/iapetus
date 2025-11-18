using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionTest : MonoBehaviour
{
    private Vector3 _basePosition;
    private Quaternion _baseRotation;

    private void Start()
    {
        _basePosition = transform.position;
        _baseRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        var f = Mathf.Sin(Time.time * 4f) * 1f;
        transform.position = _basePosition + new Vector3(f, f, f);
        transform.rotation = Quaternion.Euler(0f, f * 20f, 0f);
    }
}
