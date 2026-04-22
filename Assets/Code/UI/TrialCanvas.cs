using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TrialCanvas : MonoBehaviour
{

    [SerializeField] private Color _goldColor;
    private float _brightnessMod = 0.85f;

    [SerializeField] private TextMeshProUGUI _clearedTmp;
    [SerializeField] private TextMeshProUGUI _nameTmp;
    [SerializeField] private TextMeshProUGUI _playerTimeTmp;
    [SerializeField] private TextMeshProUGUI _newRecordTmp;
    [SerializeField] private TextMeshProUGUI _previousRecordTmp;
    [SerializeField] private TextMeshProUGUI _bestTmp;
    [SerializeField] private TextMeshProUGUI _goldTimeTmp;
    [SerializeField] private Image _goldSymbol;

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
        var playerAlreadyCompletedTrial = SaveSystem.GetTrialCompletion(trial.metaName, out var previousRecordTime);
        _previousRecordTmp.text = previousRecordTime.ToString("F2");
        _goldTimeTmp.text = trial.goldTime.ToString("F2");
        _goldTimeTmp.color = _goldColor * _brightnessMod;
        _goldSymbol.color = _goldColor * _brightnessMod;
        if (previousRecordTime > playerTime || !playerAlreadyCompletedTrial)
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

        if (playerTime < trial.goldTime)
        {
            _playerTimeTmp.color = _goldColor;
            _newRecordTmp.color = _goldColor;
            _clearedTmp.color = _goldColor;
            _clearedTmp.text = "Complete!";
        }
        else
        {
            _playerTimeTmp.color = Color.white;
            _newRecordTmp.color = Color.white;
            _clearedTmp.color = Color.white;
            _clearedTmp.text = "Cleared";
        }

        if (previousRecordTime < trial.goldTime)
        {
            _previousRecordTmp.color = _goldColor * _brightnessMod;
            _bestTmp.color = _goldColor * _brightnessMod;
        }
        else
        {
            _previousRecordTmp.color = Color.white * _brightnessMod;
            _bestTmp.color = Color.white * _brightnessMod;
        }

        if (playerTime < trial.goldTime || (previousRecordTime < trial.goldTime && playerAlreadyCompletedTrial))
        {
            _goldTimeTmp.text = "<s>" + trial.goldTime.ToString("F2") + "</s>";
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
        SaveSystem.WriteTrialCompletion(trial.metaName, playerTime, trial.goldTime);

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
        if (_canvasGroup.alpha < 0.99f) yield break;
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
