using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class LemonCanvas : MonoBehaviour
{
    private TextMeshProUGUI _tmp;
    private CanvasGroup _canvasGroup;
    private float _showTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        _tmp = GetComponentInChildren<TextMeshProUGUI>();
        TryGetComponent(out _canvasGroup);
        UpdateLemonCount();
        _showTimer = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, _showTimer < 3.5f ? 1f : 0, Time.deltaTime * 5f);
        _showTimer += Time.deltaTime;
        UpdateLemonCount();
    }
    
    private void UpdateLemonCount()
    {
        _tmp.text = SaveSystem.LoadCachedSaveData().lemonCollections.Count.ToString();
    }

    public void ResetShowTimer()
    {
        _showTimer = 0;
    }

    private void OnEnable()
    {
        Lemon.OnLemonCollected += ResetShowTimer;
    }

    private void OnDisable()
    {
        Lemon.OnLemonCollected -= ResetShowTimer;
    }
}
