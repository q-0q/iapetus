using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Search;
using UnityEngine;

public class BellCanvas : MonoBehaviour
{
    private TextMeshProUGUI _tmp;
    private CanvasGroup _canvasGroup;
    private bool show;
    
    // Start is called before the first frame update
    void Start()
    {
        _tmp = GetComponentInChildren<TextMeshProUGUI>();
        TryGetComponent(out _canvasGroup);
        UpdateBellCount();
        show = true;
        UpdateBellCount();
    }

    // Update is called once per frame
    void Update()
    {
        _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, show ? 1f : 0, Time.deltaTime * 5f);
    }

    private void OnBellRung()
    {
        UpdateBellCount();
    }

    private void UpdateBellCount()
    {
        _tmp.text = SaveSystem.LoadSaveData(0).bells.Count.ToString();
    }

    private void OnEnable()
    {
        BellController.OnBellRing += OnBellRung;
    }

    private void OnDisable()
    {
        BellController.OnBellRing -= OnBellRung;
    }
}
