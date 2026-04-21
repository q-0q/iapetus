using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeyLight : MonoBehaviour
{
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(_camera.transform.position - transform.position, Vector3.up);
    }
}
