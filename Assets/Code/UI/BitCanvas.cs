using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using TMPro;
using UnityEngine;

public class BitCanvas : MonoBehaviour
{
    private TextMeshProUGUI _tmp;
    private CanvasGroup _canvasGroup;
    private float _showTimer;

    
    // Start is called before the first frame update
    void Start()
    {
        _tmp = GetComponentInChildren<TextMeshProUGUI>();
        TryGetComponent(out _canvasGroup);
        UpdateBitCount();
        _showTimer = 100f;
        UpdateBitCount();
    }

    // Update is called once per frame
    void Update()
    {
        _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, _showTimer < 1.5f ? 1f : 0, Time.deltaTime * 5f);
        _showTimer += Time.deltaTime;
    }

    private void OnBitCountUpdated()
    {
        _canvasGroup.alpha = 1f;
        _showTimer = 0f;
        UpdateBitCount();
    }

    private void UpdateBitCount()
    {
        _tmp.text = SaveSystem.LoadCachedSaveData().bitCount.ToString();
    }

    private void OnEnable()
    {
        BitController.OnBitCountUpdated += OnBitCountUpdated;
        BitSystem.OnBitsDecremented += OnBitCountUpdated;
    }

    private void OnDisable()
    {
        BitController.OnBitCountUpdated -= OnBitCountUpdated;
        BitSystem.OnBitsDecremented -= OnBitCountUpdated;
    }
}
