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
}
