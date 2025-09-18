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
        int val = (int)(momentum * 10f / PlayerFsm.MaxMomentum * 10f);
        _tmp.text = val.ToString();
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
