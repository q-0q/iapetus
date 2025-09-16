using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MomentumUIValue : MonoBehaviour
{
    private TextMeshProUGUI _tmp;
    
    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out _tmp);
    }

    void UpdateText(float momentum)
    {
        _tmp.text = momentum.ToString().Substring(0, Math.Min(4, momentum.ToString().Length));
    }

    private void OnEnable()
    {
        PlayerFsm.OnPlayerMomentumUpdated += UpdateText;
    }

    private void OnDisable()
    {
        PlayerFsm.OnPlayerMomentumUpdated -= UpdateText;
    }
}
