using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class BellCanvas : MonoBehaviour
{
    private TextMeshProUGUI _tmp;
    private CanvasGroup _canvasGroup;
    private float _showTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        _tmp = GetComponentInChildren<TextMeshProUGUI>();
        TryGetComponent(out _canvasGroup);
        UpdateBellCount();
        _showTimer = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        if (SaveSystem.LoadSaveData(0).bellCount == 0)
        {
            _canvasGroup.alpha = 0;
            return;
        }
        _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, _showTimer < 0.5f ? 1f : 0, Time.deltaTime * 5f);
        _showTimer += Time.deltaTime;
        UpdateBellCount();
    }

    private void OnBellRung()
    {
        _canvasGroup.alpha = 1f;
    }

    private void UpdateBellCount()
    {
        _tmp.text = SaveSystem.LoadSaveData(0).bellCount.ToString();
    }

    public void ResetShowTimer()
    {
        _showTimer = 0;
    }

    private void OnEnable()
    {
        BellController.OnBellRing += OnBellRung;
        BellController.OnPlayerNearbyRungBell += ResetShowTimer;
        BellDoorController.OnPlayerNearbyUnopenedBellDoor += ResetShowTimer;
    }

    private void OnDisable()
    {
        BellController.OnBellRing -= OnBellRung;
        BellController.OnPlayerNearbyRungBell -= ResetShowTimer;
        BellDoorController.OnPlayerNearbyUnopenedBellDoor -= ResetShowTimer;
    }
}
