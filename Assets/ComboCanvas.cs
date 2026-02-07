using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComboCanvas : MonoBehaviour
{
    private TextMeshProUGUI _tmp;
    private readonly Color _highColor = new Color(255f / 255f, 99 / 255f,  73f / 255f);
    private readonly Color _lowColor = new Color(255f / 255f, 189f / 255f, 234f / 255f);
    
    // Start is called before the first frame update
    void Start()
    {
        _tmp = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        var comboLength = PlayerFsm.Singleton.GetComboLength();
        if (comboLength < 2)
        {
            _tmp.text = "";
            return;
        }
           
        _tmp.text = comboLength.ToString();
        // _tmp.color = Color.red;
        _tmp.color = Color.Lerp(_lowColor, _highColor, Mathf.InverseLerp(0, 20, comboLength));
    }
}
