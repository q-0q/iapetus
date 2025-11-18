using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PixelRawImage : MonoBehaviour
{
    public static PixelRawImage Singleton;
    [FormerlySerializedAs("rawImage")] public RawImage RawImage;

    private void Awake()
    {
        Singleton = this;
        TryGetComponent(out RawImage);
    }
}
