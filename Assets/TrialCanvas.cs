using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrialCanvas : MonoBehaviour
{

    [SerializeField] private Color _goldColor;

    [SerializeField] private TextMeshProUGUI _clearedTmp;
    [SerializeField] private TextMeshProUGUI _nameTmp;
    [SerializeField] private TextMeshProUGUI _playerTimeTmp;
    [SerializeField] private TextMeshProUGUI _recordTmp;
    [SerializeField] private TextMeshProUGUI _goldTimeTmp;

    private CanvasGroup _canvasGroup;
    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out _canvasGroup);
        _canvasGroup.alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPlayerCompletedTrial(TrialCollectibleFsm trial, float playerTime)
    {
        StartCoroutine(Open(trial, playerTime));
    }

    private IEnumerator Open(TrialCollectibleFsm trial, float playerTime)
    {
        _nameTmp.text = trial.displayName;
        _playerTimeTmp.text = playerTime.ToString("F2");
        var duration = 0.25f;
        var t = 0f;
        while (t < duration)
        {
            _canvasGroup.alpha = t / duration;
            t += Time.deltaTime;
            yield return null;
        }
        yield break;
    }

    private void OnEnable()
    {
        TrialCollectibleFsm.OnPlayerCompletedTrial += OnPlayerCompletedTrial;
    }

    private void OnDisable()
    {
        TrialCollectibleFsm.OnPlayerCompletedTrial -= OnPlayerCompletedTrial;
    }
}
