using System;
using UnityEngine;

public class MuralTerminalIndicator : MonoBehaviour
{

    public string node;

    private GameObject curvedStar;

    private GameObject halo;

    private void Awake()
    {
        curvedStar = transform.Find("CurvedStar").gameObject;
        halo = transform.Find("Halo").gameObject;
        SetVisibility();
    }

    private void OnEnable()
    {
        SaveSystem.OnSaveDataUpdated += OnSaveDataUpdated;
    }

    private void OnDisable()
    {
        SaveSystem.OnSaveDataUpdated -= OnSaveDataUpdated;
    }

    private void OnSaveDataUpdated(SaveSystem.SaveData obj)
    {
        SetVisibility();
    }

    private void SetVisibility()
    {
        var visible = SaveSystem.GetTerminalNode(node);
        curvedStar.SetActive(visible);
        halo.SetActive(visible);
    }
}
