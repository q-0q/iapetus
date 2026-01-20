using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TrialCanvas : MonoBehaviour
{

    [SerializeField] private Color _goldColor;

    [SerializeField] private TextMeshProUGUI _clearedTmp;
    [SerializeField] private TextMeshProUGUI _nameTmp;
    [SerializeField] private TextMeshProUGUI _playerTimeTmp;
    [SerializeField] private TextMeshProUGUI _newRecordTmp;
    [SerializeField] private TextMeshProUGUI _previousRecordTmp;
    [SerializeField] private TextMeshProUGUI _bestTmp;
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
    
    private void OnPlayerBeganTrial()
    {
        StartCoroutine(Close());
    }

    private IEnumerator Open(TrialCollectibleFsm trial, float playerTime)
    {
        var previousRecordTime = SaveSystem.GetTrialCompletion(trial.metaName, 0);
        _previousRecordTmp.text = previousRecordTime.ToString("F2");
        if (previousRecordTime > playerTime)
        {
            // new record
            _newRecordTmp.gameObject.SetActive(true);
            _previousRecordTmp.gameObject.SetActive(false);
            _bestTmp.gameObject.SetActive(false);
        }
        else
        {
            _newRecordTmp.gameObject.SetActive(false);
            _previousRecordTmp.gameObject.SetActive(true);
            _bestTmp.gameObject.SetActive(true);
        }
        
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
        _canvasGroup.alpha = 1;
        SaveSystem.WriteTrialCompletion(trial.metaName, playerTime, 0);

        duration = 4f;
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(Close());
        yield break;
    }
    
    private IEnumerator Close()
    {
        if (_canvasGroup.alpha < 0.1f) yield break;
        var duration = 0.45f;
        var t = 0f;
        while (t < duration)
        {
            _canvasGroup.alpha = Mathf.Lerp(1f, 0, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        _canvasGroup.alpha = 0;
        yield break;
    }

    private void OnEnable()
    {
        TrialCollectibleFsm.OnPlayerCompletedTrial += OnPlayerCompletedTrial;
        TrialCollectibleFsm.OnPlayerBeganTrial += OnPlayerBeganTrial;
        
    }

    private void OnDisable()
    {
        TrialCollectibleFsm.OnPlayerCompletedTrial -= OnPlayerCompletedTrial;
        TrialCollectibleFsm.OnPlayerBeganTrial -= OnPlayerBeganTrial;
    }
}
