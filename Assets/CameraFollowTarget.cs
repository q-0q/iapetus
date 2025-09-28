using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    public static CameraFollowTarget Singleton;

    private void Awake()
    {
        Singleton = this;
    }
}
